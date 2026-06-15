namespace Fastdotnet.Core.Notification;

/// <summary>
/// 事件发布器接口，业务代码的唯一入口
/// </summary>
public interface IEventPublisher
{
    /// <summary>
    /// 发布一个事件到当前工作单元上下文（延迟持久化）
    /// </summary>
    /// <param name="event">事件实例</param>
    /// <param name="cancellationToken">取消令牌</param>
    ValueTask PublishAsync(IFastdotnetEvent @event, CancellationToken cancellationToken = default);
}
