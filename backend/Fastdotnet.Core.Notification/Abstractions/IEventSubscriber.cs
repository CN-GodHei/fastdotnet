namespace Fastdotnet.Core.Notification;

/// <summary>
/// 事件订阅器接口。实现此接口以订阅特定事件类型。
/// </summary>
public interface IEventSubscriber
{
    /// <summary>
    /// 处理事件
    /// </summary>
    /// <param name="eventType">事件类型标识</param>
    /// <param name="payload">事件载荷 JSON</param>
    /// <param name="cancellationToken">取消令牌</param>
    Task HandleAsync(string eventType, string payload, CancellationToken cancellationToken = default);
}
