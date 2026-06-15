using System.Threading.Channels;
using Fastdotnet.Core.Notification.Entities;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using SqlSugar;

namespace Fastdotnet.Core.Notification.Infrastructure;

/// <summary>
/// 后台发件箱调度器。
/// 双驱动：Channel 内存信号（毫秒级响应）+ 定时轮询（兜底）。
/// 通过 SqlSugar 乐观锁实现多节点分布式抢占。
/// 投递时将事件路由到 EventRouter → SignalR/Webhook 等订阅者。
/// </summary>
internal sealed class OutboxDispatcher : BackgroundService
{
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly ILogger<OutboxDispatcher> _logger;
    private readonly Channel<string> _signalChannel;
    private readonly string _instanceId;
    private readonly OutboxDispatcherOptions _options;

    public OutboxDispatcher(
        IServiceScopeFactory scopeFactory,
        ILogger<OutboxDispatcher> logger,
        OutboxDispatcherOptions? options = null)
    {
        _scopeFactory = scopeFactory;
        _logger = logger;
        _signalChannel = Channel.CreateUnbounded<string>(new UnboundedChannelOptions { SingleReader = true });
        _instanceId = Guid.NewGuid().ToString("N");
        _options = options ?? new OutboxDispatcherOptions();
    }

    /// <summary>
    /// 发送内存信号，唤醒调度器立即处理指定事件
    /// </summary>
    public bool TrySignal(string eventId)
    {
        return _signalChannel.Writer.TryWrite(eventId);
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("OutboxDispatcher 启动，实例ID: {InstanceId}", _instanceId);

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                using var cts = CancellationTokenSource.CreateLinkedTokenSource(stoppingToken);
                cts.CancelAfter(TimeSpan.FromSeconds(_options.PollingInterval));

                try
                {
                    await _signalChannel.Reader.ReadAsync(cts.Token);
                }
                catch (OperationCanceledException)
                {
                    // 超时，走定时轮询
                }

                await ProcessBatchAsync(stoppingToken);
            }
            catch (Exception ex) when (ex is not OperationCanceledException)
            {
                _logger.LogError(ex, "OutboxDispatcher 处理异常");
                await Task.Delay(1000, stoppingToken);
            }
        }

        _logger.LogInformation("OutboxDispatcher 已停止");
    }

    private async Task ProcessBatchAsync(CancellationToken ct)
    {
        using var scope = _scopeFactory.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<ISqlSugarClient>();
        var router = scope.ServiceProvider.GetRequiredService<EventRouter>();

        var lockDuration = DateTime.Now.AddSeconds(_options.LockDurationSeconds);

        // 原子抢占：UPDATE ... SET LockId = @instanceId WHERE ...
        var affected = await db.Updateable<SysOutbox>()
            .SetColumns(it => new SysOutbox
            {
                Status = OutboxStatus.Processing,
                LockId = _instanceId,
                LockExpiredTime = lockDuration
            })
            .Where(it =>
                (it.Status == OutboxStatus.Pending ||
                 (it.Status == OutboxStatus.Failed && it.NextAttemptTime <= DateTime.Now))
                && (it.LockExpiredTime == null || it.LockExpiredTime <= DateTime.Now))
            .ExecuteCommandAsync(ct);

        if (affected == 0) return;

        // 查询抢占到的记录
        var batch = await db.Queryable<SysOutbox>()
            .Where(it => it.LockId == _instanceId && it.Status == OutboxStatus.Processing)
            .ToListAsync(ct);

        var toProcess = batch.Take(_options.BatchSize).ToList();

        foreach (var entry in toProcess)
        {
            await ProcessSingleEntryAsync(db, router, entry, ct);
        }
    }

    private async Task ProcessSingleEntryAsync(ISqlSugarClient db, EventRouter router, SysOutbox entry, CancellationToken ct)
    {
        try
        {
            // 通过 EventRouter 投递到所有匹配的订阅者（SignalR / Webhook / 自定义订阅者）
            await router.RouteAsync(entry.EventType, entry.Payload, ct);

            // 标记为已发布
            await db.Updateable<SysOutbox>()
                .SetColumns(it => new SysOutbox
                {
                    Status = OutboxStatus.Published,
                    LockId = null,
                    LockExpiredTime = null
                })
                .Where(it => it.Id == entry.Id)
                .ExecuteCommandAsync(ct);

            _logger.LogDebug("事件已投递: {EventId} ({EventType})", entry.Id, entry.EventType);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "事件投递失败: {EventId}", entry.Id);

            var newRetryCount = entry.RetryCount + 1;
            if (newRetryCount >= _options.MaxRetryCount)
            {
                // 移入死信表
                await db.Insertable(new SysDeadLetter
                {
                    Id = entry.Id,
                    TenantId = entry.TenantId,
                    EventType = entry.EventType,
                    Payload = entry.Payload,
                    ErrorMessage = ex.ToString(),
                    FailedAt = DateTime.Now
                }).ExecuteCommandAsync(ct);

                await db.Deleteable<SysOutbox>().Where(it => it.Id == entry.Id).ExecuteCommandAsync(ct);
                _logger.LogWarning("事件移入死信: {EventId}", entry.Id);
            }
            else
            {
                var delaySeconds = Math.Pow(2, newRetryCount);
                await db.Updateable<SysOutbox>()
                    .SetColumns(it => new SysOutbox
                    {
                        Status = OutboxStatus.Failed,
                        RetryCount = newRetryCount,
                        NextAttemptTime = DateTime.Now.AddSeconds(delaySeconds),
                        LockId = null,
                        LockExpiredTime = null
                    })
                    .Where(it => it.Id == entry.Id)
                    .ExecuteCommandAsync(ct);
            }
        }
    }
}

/// <summary>
/// OutboxDispatcher 配置选项
/// </summary>
public class OutboxDispatcherOptions
{
    /// <summary>定时轮询间隔（秒），默认 5 秒</summary>
    public int PollingInterval { get; set; } = 5;

    /// <summary>每次抢占的最大批量数</summary>
    public int BatchSize { get; set; } = 50;

    /// <summary>分布式锁持续时间（秒）</summary>
    public int LockDurationSeconds { get; set; } = 30;

    /// <summary>最大重试次数，超过后移入死信</summary>
    public int MaxRetryCount { get; set; } = 10;
}
