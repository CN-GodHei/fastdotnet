using Fastdotnet.Plugin.Contracts.Events;

namespace Fastdotnet.WebApi.Services.EventBus;

/// <summary>
/// 内存事件总线实现
/// </summary>
public class InMemoryEventBus : IEventBus, IDisposable
{
    private readonly ConcurrentDictionary<Type, List<IEventHandlerInternal>> _handlers = new();
    
    private readonly ILogger<InMemoryEventBus> _logger;
    private readonly IServiceProvider _serviceProvider;
    private readonly SemaphoreSlim _semaphore = new(1, 1);
    
    public InMemoryEventBus(
        ILogger<InMemoryEventBus> logger,
        IServiceProvider serviceProvider)
    {
        _logger = logger;
        _serviceProvider = serviceProvider;
    }
    
    /// <summary>
    /// 发布事件
    /// </summary>
    public async Task PublishAsync<TEvent>(TEvent @event, CancellationToken cancellationToken = default) 
        where TEvent : class
    {
        if (@event == null) 
            throw new ArgumentNullException(nameof(@event));
        
        var eventType = typeof(TEvent);
        
        _logger.LogDebug("发布事件：{EventType} (ID: {EventId})", eventType.Name, GetEventId(@event));
        
        if (_handlers.TryGetValue(eventType, out var handlers))
        {
            // 并行处理所有处理器
            var tasks = handlers.Select(async handler =>
            {
                try
                {
                    // ✅ 直接使用已保存的 handler 实例，不再重新创建
                    await handler.HandleAsync(@event, cancellationToken);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "事件处理失败：{EventType}", eventType.Name);
                    
                    // 可以添加重试逻辑、死信队列等
                    await HandleProcessingErrorAsync(@event, ex, cancellationToken);
                }
            });
            
            await Task.WhenAll(tasks);
        }
        else
        {
            _logger.LogError("没有订阅者：{EventType}", eventType.Name);
        }
    }
    
    /// <summary>
    /// 订阅事件
    /// </summary>
    public void Subscribe<TEvent>(IEventHandler<TEvent> handler) where TEvent : class
    {
        var handlers = _handlers.GetOrAdd(typeof(TEvent), _ => new List<IEventHandlerInternal>());
        
        lock (handlers)
        {
            if (!handlers.Any(h => ReferenceEquals(h, handler)))
            {
                // ✅ 如果 handler 实现了 IEventHandlerInternal，直接使用
                if (handler is IEventHandlerInternal internalHandler)
                {
                    handlers.Add(internalHandler);
                    _logger.LogInformation("订阅事件：{EventType} -> {HandlerType}", 
                        typeof(TEvent).Name, handler.GetType().Name);
                }
                else
                {
                    // ❌ 否则使用适配器包装（备用方案）
                    var adapter = new EventHandlerAdapter<TEvent>(handler);
                    handlers.Add(adapter);
                    _logger.LogInformation("订阅事件（使用适配器）: {EventType} -> {HandlerType}", 
                        typeof(TEvent).Name, handler.GetType().Name);
                }
            }
        }
    }
    
    /// <summary>
    /// 取消订阅
    /// </summary>
    public void Unsubscribe<TEvent>(IEventHandler<TEvent> handler) where TEvent : class
    {
        if (_handlers.TryGetValue(typeof(TEvent), out var handlers))
        {
            lock (handlers)
            {
                handlers.RemoveAll(h => h.GetType() == handler.GetType());
            }
        }
    }
    
    /// <summary>
    /// 处理错误
    /// </summary>
    private async Task HandleProcessingErrorAsync<TEvent>(
        TEvent @event, 
        Exception ex, 
        CancellationToken ct)
    {
        // 发布到死信队列（未来实现）
        await Task.CompletedTask;
    }
    
    /// <summary>
    /// 获取事件 ID（用于日志）
    /// </summary>
    private static string GetEventId(object @event)
    {
        return @event.GetType().GetProperty("EventId")?.GetValue(@event)?.ToString() 
               ?? @event.GetHashCode().ToString();
    }
    
    public void Dispose()
    {
        _semaphore.Dispose();
        _handlers.Clear();
    }
}

/// <summary>
/// 适配器模式 - 将泛型处理器适配为非泛型接口
/// </summary>
internal class EventHandlerAdapter<TEvent> : IEventHandlerInternal, IEventHandler<TEvent> 
    where TEvent : class
{
    private readonly IEventHandler<TEvent> _inner;
    
    public EventHandlerAdapter(IEventHandler<TEvent> inner)
    {
        _inner = inner;
    }
    
    public Task HandleAsync(TEvent @event, CancellationToken cancellationToken = default)
    {
        return _inner.HandleAsync(@event, cancellationToken);
    }
    
    Task IEventHandlerInternal.HandleAsync(object @event, CancellationToken cancellationToken = default)
    {
        return HandleAsync((TEvent)@event, cancellationToken);
    }
}
