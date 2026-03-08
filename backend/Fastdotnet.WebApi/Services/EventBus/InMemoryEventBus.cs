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
                    // 创建独立的作用域（支持依赖注入）
                    using var scope = _serviceProvider.CreateScope();
                    
                    // 从作用域中解析处理器
                    var scopedHandler = (IEventHandlerInternal)ActivatorUtilities
                        .CreateInstance(scope.ServiceProvider, handler.GetType());
                    
                    await scopedHandler.HandleAsync(@event, cancellationToken);
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
            _logger.LogDebug("没有订阅者：{EventType}", eventType.Name);
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
            if (!handlers.Any(h => h.GetType() == handler.GetType()))
            {
                handlers.Add((IEventHandlerInternal)handler);
                _logger.LogInformation("订阅事件：{EventType} -> {HandlerType}", 
                    typeof(TEvent).Name, handler.GetType().Name);
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
/// 内部接口，用于反射调用
/// </summary>
internal interface IEventHandlerInternal
{
    Task HandleAsync(object @event, CancellationToken cancellationToken = default);
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
