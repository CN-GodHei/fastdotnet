using System.Reflection;
using System.Text.Json;
using Fastdotnet.Core.Notification.Entities;

namespace Fastdotnet.Core.Notification.Infrastructure;

/// <summary>
/// Scoped 事件发布器实现。将事件包装为 CloudEvents 1.0 信封后存入 EventContext，由拦截器统一落库。
/// </summary>
internal sealed class EventPublisher : IEventPublisher
{
    private readonly EventContext _context;
    private readonly JsonSerializerOptions _jsonOptions;
    private readonly string _source;

    public EventPublisher(EventContext context, JsonSerializerOptions? jsonOptions = null, string source = "fastdotnet://host")
    {
        _context = context;
        _jsonOptions = jsonOptions ?? new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            WriteIndented = false
        };
        _source = source;
    }

    public ValueTask PublishAsync(IFastdotnetEvent @event, CancellationToken cancellationToken = default)
    {
        if (@event == null)
            throw new ArgumentNullException(nameof(@event));

        _context.Add(@event);
        return ValueTask.CompletedTask;
    }

    /// <summary>
    /// 从 EventContext 提取事件，包装为 CloudEvents 1.0 信封并转换为 SysOutbox 实体列表
    /// </summary>
    internal static List<SysOutbox> DrainToOutboxEntries(EventContext context, string? tenantId, JsonSerializerOptions jsonOptions, string source = "fastdotnet://host")
    {
        var entries = new List<SysOutbox>();
        foreach (var (evt, timestamp) in context.Drain())
        {
            var eventType = ResolveEventType(evt.GetType());

            // 使用 CloudEvents 1.0 信封包装
            var envelope = CloudEventEnvelope.FromEvent(evt, source, jsonOptions);
            envelope.Id = Guid.NewGuid().ToString("N"); // 覆盖为事件 ID
            var payload = envelope.Serialize(jsonOptions);

            entries.Add(new SysOutbox
            {
                Id = envelope.Id,
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
        return parts.Length >= 1 ? parts[0] : "Unknown";
    }
}
