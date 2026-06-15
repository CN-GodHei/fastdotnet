using Fastdotnet.Core.Notification;
using Fastdotnet.Core.Notification.Infrastructure;
using PluginA.Events;
using SqlSugar;

namespace PluginA.Services;

/// <summary>
/// Notification 演示服务
/// 演示如何使用新的推送基础设施发布事件并在事务内落库
/// </summary>
public class NotificationDemoService
{
    private readonly ISqlSugarClient _db;
    private readonly IEventPublisher _publisher;
    private readonly EventContext _eventContext;
    private readonly ILogger<NotificationDemoService> _logger;

    public NotificationDemoService(
        ISqlSugarClient db,
        IEventPublisher publisher,
        EventContext eventContext,
        ILogger<NotificationDemoService> logger)
    {
        _db = db;
        _publisher = publisher;
        _eventContext = eventContext;
        _logger = logger;
    }

    /// <summary>
    /// 演示：扣减库存并在同一事务内发布事件
    /// </summary>
    public async Task<string> DeductInventoryAsync(string productId, int quantity)
    {
        _logger.LogInformation("扣减库存：Product={ProductId}, Qty={Quantity}", productId, quantity);

        await _db.Ado.UseTranAsync(async () =>
        {
            // 1. 正常业务操作（演示）
            _logger.LogInformation("  模拟数据库操作：UPDATE inventory SET stock = stock - {Qty}", quantity);
            await Task.Delay(50);

            // 2. 发布通知事件（延迟收集）
            var evt = new InventoryDeductedEvent(productId, quantity, 100 - quantity, "01仓库");
            await _publisher.PublishAsync(evt);

            // 3. 事务内落库（与业务数据同事务）
            await _db.FlushOutboxAsync(_eventContext, tenantId: null);
        });

        _logger.LogInformation("库存扣减完成，事件已入发件箱");
        return Guid.NewGuid().ToString("N");
    }

    /// <summary>
    /// 演示：发货通知
    /// </summary>
    public async Task ShipOrderAsync(string orderId, string courier)
    {
        await _db.Ado.UseTranAsync(async () =>
        {
            var tracking = $"SF{Guid.NewGuid():N}"[..12];
            var evt = new OrderShippedEvent(orderId, tracking, courier, DateTime.Now);

            await _publisher.PublishAsync(evt);
            await _db.FlushOutboxAsync(_eventContext);

            _logger.LogInformation("订单 {OrderId} 已发货，快递单号 {Tracking}", orderId, tracking);
        });
    }

    /// <summary>
    /// 演示：用户注册（多层事件发布）
    /// </summary>
    public async Task RegisterUserAsync(string userName)
    {
        await _db.Ado.UseTranAsync(async () =>
        {
            var userId = $"U{Guid.NewGuid():N}"[..10];

            // 模拟用户写入
            _logger.LogInformation("  模拟：INSERT INTO users VALUES ({UserId}, {Name})", userId, userName);

            // 发布注册事件
            var evt = new UserRegisteredEvent(userId, userName, DateTime.Now, new Dictionary<string, string>
            {
                ["Source"] = "PluginA",
                ["Channel"] = "App"
            });
            await _publisher.PublishAsync(evt);

            // 同时发布库存分配事件（一个事务发布多个事件）
            var inventoryEvt = new InventoryDeductedEvent("GIFT-001", 1, 999, "赠品池");
            await _publisher.PublishAsync(inventoryEvt);

            // 统一落库
            await _db.FlushOutboxAsync(_eventContext);
        });
    }
}
