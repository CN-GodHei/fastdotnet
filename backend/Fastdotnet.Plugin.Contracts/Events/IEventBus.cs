using System.Threading;

namespace Fastdotnet.Plugin.Contracts.Events;

/// <summary>
/// 事件总线接口（通用）
/// 支持强类型（进程内模块通信）和弱类型（跨插件通信）两种模式
/// 弱类型事件Key格式: {pluginId}.{eventName}，如 "11365281228127823.payment.completed"
/// </summary>
public interface IEventBus
{
    // ==================== 强类型方法（进程内模块通信）====================
    
    /// <summary>
    /// 发布强类型事件
    /// </summary>
    /// <typeparam name="TEvent">事件类型</typeparam>
    /// <param name="event">事件实例</param>
    /// <param name="cancellationToken">取消令牌</param>
    Task PublishAsync<TEvent>(TEvent @event, CancellationToken cancellationToken = default) 
        where TEvent : class;
    
    /// <summary>
    /// 订阅强类型事件
    /// </summary>
    /// <typeparam name="TEvent">事件类型</typeparam>
    /// <param name="handler">事件处理器</param>
    /// <returns>IDisposable，释放后自动取消订阅</returns>
    IDisposable Subscribe<TEvent>(IEventHandler<TEvent> handler) where TEvent : class;
    
    /// <summary>
    /// 取消强类型事件订阅
    /// </summary>
    /// <typeparam name="TEvent">事件类型</typeparam>
    /// <param name="handler">事件处理器</param>
    void Unsubscribe<TEvent>(IEventHandler<TEvent> handler) where TEvent : class;
    
    // ==================== 弱类型方法（跨插件通信）====================
    
    /// <summary>
    /// 发布弱类型事件（跨插件通信）
    /// </summary>
    /// <param name="eventKey">事件Key，格式: {pluginId}.{eventName}</param>
    /// <param name="eventData">事件数据（任意对象，将被JSON序列化）</param>
    /// <param name="cancellationToken">取消令牌</param>
    Task PublishAsync(string eventKey, object eventData, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// 订阅弱类型事件（跨插件通信）
    /// </summary>
    /// <param name="eventKey">事件Key，格式: {pluginId}.{eventName}，支持通配符 *</param>
    /// <param name="handler">事件处理函数，接收 JSON 反序列化后的 object</param>
    /// <returns>IDisposable，释放后自动取消订阅</returns>
    IDisposable Subscribe(string eventKey, Func<object, Task> handler);
    
    /// <summary>
    /// 取消指定 eventKey 的所有弱类型订阅
    /// </summary>
    /// <param name="eventKey">事件Key</param>
    void UnsubscribeAll(string eventKey);
}

/// <summary>
/// 事件处理器接口（强类型）
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

/// <summary>
/// 内部事件处理器接口（用于反射调用，支持非泛型场景）
/// 这是 EventBus 内部使用的适配器接口
/// </summary>
public interface IEventHandlerInternal
{
    /// <summary>
    /// 处理事件（非泛型版本）
    /// </summary>
    /// <param name="event">事件实例（object 类型）</param>
    /// <param name="cancellationToken">取消令牌</param>
    Task HandleAsync(object @event, CancellationToken cancellationToken = default);
}
