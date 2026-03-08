namespace Fastdotnet.Plugin.Contracts.Events;

/// <summary>
/// 事件总线接口（通用）
/// </summary>
public interface IEventBus
{
    /// <summary>
    /// 发布事件
    /// </summary>
    /// <typeparam name="TEvent">事件类型</typeparam>
    /// <param name="event">事件实例</param>
    /// <param name="cancellationToken">取消令牌</param>
    Task PublishAsync<TEvent>(TEvent @event, CancellationToken cancellationToken = default) 
        where TEvent : class;
    
    /// <summary>
    /// 订阅事件
    /// </summary>
    /// <typeparam name="TEvent">事件类型</typeparam>
    /// <param name="handler">事件处理器</param>
    void Subscribe<TEvent>(IEventHandler<TEvent> handler) where TEvent : class;
    
    /// <summary>
    /// 取消订阅
    /// </summary>
    /// <typeparam name="TEvent">事件类型</typeparam>
    /// <param name="handler">事件处理器</param>
    void Unsubscribe<TEvent>(IEventHandler<TEvent> handler) where TEvent : class;
}

/// <summary>
/// 事件处理器接口
/// </summary>
/// <typeparam name="TEvent">事件类型</typeparam>
public interface IEventHandler<in TEvent> where TEvent : class
{
    /// <summary>
    /// 处理事件
    /// </summary>
    /// <param name="event">事件实例</param>
    /// <param name="cancellationToken">取消令牌</param>
    Task HandleAsync(TEvent @event, CancellationToken cancellationToken = default);
}
