using System.Reflection;
using System.Text.Json;
using Fastdotnet.Core.Notification.Entities;
using Fastdotnet.Core.Notification.Infrastructure;
using SqlSugar;

namespace Fastdotnet.Core.Notification.Infrastructure;

/// <summary>
/// Scoped 事件发布器实现。将事件序列化后存入 EventContext，由拦截器统一落库。
/// </summary>
internal sealed class EventPublisher : IEventPublisher
{
    private readonly EventContext _context;
    private readonly JsonSerializerOptions _jsonOptions;

    public EventPublisher(EventContext context, JsonSerializerOptions? jsonOptions = null)
    {
        _context = context;
        _jsonOptions = jsonOptions ?? new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            WriteIndented = false
        };
    }

    public ValueTask PublishAsync(IFastdotnetEvent @event, CancellationToken cancellationToken = default)
    {
        if (@event == null)
            throw new ArgumentNullException(nameof(@event));

        _context.Add(@event);
        return ValueTask.CompletedTask;
    }

    /// <summary>
    /// 从 EventContext 提取事件并转换为 SysOutbox 实体列表
    /// </summary>
    internal static List<SysOutbox> DrainToOutboxEntries(EventContext context, string? tenantId, JsonSerializerOptions jsonOptions)
    {
        var entries = new List<SysOutbox>();
        foreach (var (evt, timestamp) in context.Drain())
        {
            var eventType = ResolveEventType(evt.GetType());
            var payload = JsonSerializer.Serialize(evt, evt.GetType(), jsonOptions);

            entries.Add(new SysOutbox
            {
                Id = Guid.NewGuid().ToString("N"),
                TenantId = tenantId,
                SourceModule = ResolveSourceModule(evt.GetType()),
                EventType = eventType,
                Payload = payload,
                Status = OutboxStatus.Pending,
                CreatedAt = timestamp
            });
        }
        return entries;
    }

    private static string ResolveEventType(Type eventType)
    {
        var attr = eventType.GetCustomAttribute<EventContractAttribute>();
        return attr?.EventType ?? eventType.FullName ?? eventType.Name;
    }

    private static string ResolveSourceModule(Type eventType)
    {
        var ns = eventType.Namespace ?? string.Empty;
        var parts = ns.Split('.');
        // 取命名空间第二段作为模块名，如 "MyPlugin.Events" -> "MyPlugin"
        return parts.Length >= 1 ? parts[0] : "Unknown";
    }
}
