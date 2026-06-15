using System.Text.Json;
using Fastdotnet.Core.Notification.Entities;
using SqlSugar;

namespace Fastdotnet.Core.Notification.Infrastructure;

/// <summary>
/// Outbox 事务辅助器。提供扩展方法将 EventContext 中的事件在同一个事务内写入 SysOutbox。
/// 业务代码在 SqlSugar 事务内调用 FlushOutboxAsync 即可。
/// </summary>
public static class OutboxTransactionExtensions
{
    /// <summary>
    /// 在当前 SqlSugarClient 事务内，将 EventContext 中收集的事件批量写入 SysOutbox。
    /// 必须已经在事务中调用。
    /// </summary>
    public static async Task FlushOutboxAsync(
        this ISqlSugarClient db,
        EventContext context,
        string? tenantId = null,
        JsonSerializerOptions? jsonOptions = null,
        CancellationToken ct = default)
    {
        if (context == null || context.Count == 0)
            return;

        var entries = EventPublisher.DrainToOutboxEntries(context, tenantId,
            jsonOptions ?? new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase });

        if (entries.Count > 0)
        {
            await db.Insertable(entries).ExecuteCommandAsync(ct);
        }
    }
}
