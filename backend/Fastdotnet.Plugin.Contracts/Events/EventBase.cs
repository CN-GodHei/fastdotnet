namespace Fastdotnet.Plugin.Contracts.Events;

/// <summary>
/// 事件基类
/// </summary>
public abstract class EventBase
{
    /// <summary>
    /// 事件 ID（全局唯一）
    /// </summary>
    public string EventId { get; set; } = Guid.NewGuid().ToString("N");
    
    /// <summary>
    /// 发生时间（UTC）
    /// </summary>
    public DateTime OccurredOn { get; set; } = DateTime.Now;
    
    /// <summary>
    /// 事件来源（插件名称或系统模块）
    /// </summary>
    public string Source { get; set; } = string.Empty;
    
    /// <summary>
    /// 扩展数据（用于传递额外的业务数据）
    /// </summary>
    public Dictionary<string, object> Data { get; set; } = new();
}

/// <summary>
/// 领域事件基类（与聚合根相关的事件）
/// </summary>
public abstract class DomainEvent : EventBase
{
    /// <summary>
    /// 聚合根 ID
    /// </summary>
    public virtual string AggregateId { get; set; } = string.Empty;
}

/// <summary>
/// 集成事件基类（跨服务/跨插件通信的事件）
/// </summary>
public abstract class IntegrationEvent : EventBase
{
    /// <summary>
    /// 消息类型（用于路由）
    /// </summary>
    public string MessageType { get; set; } = string.Empty;
}
