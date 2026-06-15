namespace Fastdotnet.Core.Notification;

/// <summary>
/// 事件契约特性，用于标记事件类并指定唯一的事件类型标识
/// </summary>
[AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = false)]
public sealed class EventContractAttribute : Attribute
{
    /// <summary>
    /// 事件唯一标识键（推荐反向域名格式，如 "com.fastdotnet.order.created.v1"）
    /// </summary>
    public string EventType { get; }

    /// <summary>
    /// 事件描述
    /// </summary>
    public string? Description { get; set; }

    public EventContractAttribute(string eventType)
    {
        if (string.IsNullOrWhiteSpace(eventType))
            throw new ArgumentException("事件类型不能为空", nameof(eventType));
        EventType = eventType;
    }
}

/// <summary>
/// Fastdotnet 事件标记接口，所有事件记录类型必须实现此接口
/// </summary>
public interface IFastdotnetEvent { }
