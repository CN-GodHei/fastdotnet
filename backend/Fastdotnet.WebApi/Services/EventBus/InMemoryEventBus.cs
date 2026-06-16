using System.Collections.Concurrent;
using Fastdotnet.Plugin.Contracts.Events;
using JsonSerializer = System.Text.Json.JsonSerializer;

namespace Fastdotnet.WebApi.Services.EventBus;

/// <summary>
/// 内存事件总线实现
/// 支持两种模式：
///   强类型（进程内模块通信）—— 按 CLR 类型路由
///   弱类型（跨插件通信）—— 按 eventKey 字符串路由，payload 走 JSON 序列化
/// </summary>
public class InMemoryEventBus : IEventBus, IDisposable
{
    // ============ 强类型路由表: Type → List<IEventHandlerInternal> ============
    private readonly ConcurrentDictionary<Type, List<HandlerEntry>> _typedHandlers = new();

    // ============ 弱类型路由表: eventKey → List<Func<object, Task>> ============
    private readonly ConcurrentDictionary<string, List<WeakHandlerEntry>> _weakHandlers = new();

    private readonly ILogger<InMemoryEventBus> _logger;
    private readonly IServiceProvider _serviceProvider;

    public InMemoryEventBus(
        ILogger<InMemoryEventBus> logger,
        IServiceProvider serviceProvider)
    {
        _logger = logger;
        _serviceProvider = serviceProvider;
    }

    // ========================================================================
    // 强类型方法
    // ========================================================================

    /// <summary>
    /// 发布强类型事件
    /// </summary>
    public async Task PublishAsync<TEvent>(TEvent @event, CancellationToken cancellationToken = default)
        where TEvent : class
    {
        if (@event == null)
            throw new ArgumentNullException(nameof(@event));

        var eventType = typeof(TEvent);
        _logger.LogDebug("发布强类型事件：{EventType}", eventType.Name);

        if (_typedHandlers.TryGetValue(eventType, out var entries))
        {
            var tasks = entries.Select(async entry =>
            {
                try
                {
                    await entry.Handler.HandleAsync(@event, cancellationToken);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "强类型事件处理失败：{EventType} → {HandlerType}",
                        eventType.Name, entry.Handler.GetType().Name);
                }
            });

            await Task.WhenAll(tasks);
        }
    }

    /// <summary>
    /// 订阅强类型事件，返回 IDisposable 用于取消订阅
    /// </summary>
    public IDisposable Subscribe<TEvent>(IEventHandler<TEvent> handler) where TEvent : class
    {
        var eventType = typeof(TEvent);
        var entries = _typedHandlers.GetOrAdd(eventType, _ => new List<HandlerEntry>());

        IEventHandlerInternal internalHandler;
        if (handler is IEventHandlerInternal ih)
        {
            internalHandler = ih;
        }
        else
        {
            internalHandler = new EventHandlerAdapter<TEvent>(handler);
        }

        var entry = new HandlerEntry(internalHandler);
        lock (entries)
        {
            entries.Add(entry);
        }

        _logger.LogInformation("订阅强类型事件：{EventType} → {HandlerType}",
            eventType.Name, handler.GetType().Name);

        return new Subscription(() =>
        {
            lock (entries)
            {
                entries.Remove(entry);
            }
            _logger.LogInformation("取消订阅强类型事件：{EventType} → {HandlerType}",
                eventType.Name, handler.GetType().Name);
        });
    }

    /// <summary>
    /// 取消强类型事件订阅
    /// </summary>
    public void Unsubscribe<TEvent>(IEventHandler<TEvent> handler) where TEvent : class
    {
        var eventType = typeof(TEvent);
        if (_typedHandlers.TryGetValue(eventType, out var entries))
        {
            lock (entries)
            {
                entries.RemoveAll(e => e.Handler.GetType() == handler.GetType());
            }
        }
    }

    // ========================================================================
    // 弱类型方法（跨插件通信）
    // ========================================================================

    /// <summary>
    /// 发布弱类型事件（跨插件通信）
    /// eventKey 格式: {pluginId}.{eventName}
    /// 支持通配符匹配：订阅 "*.payment.completed" 可收到任意插件的 payment.completed 事件
    /// </summary>
    public async Task PublishAsync(string eventKey, object eventData, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(eventKey))
            throw new ArgumentNullException(nameof(eventKey));
        if (eventData == null)
            throw new ArgumentNullException(nameof(eventData));

        _logger.LogDebug("发布弱类型事件：{EventKey}", eventKey);

        // 精确匹配 + 通配符匹配
        var matchingHandlers = new List<WeakHandlerEntry>();

        foreach (var (key, entries) in _weakHandlers)
        {
            if (MatchEventKey(key, eventKey))
            {
                lock (entries)
                {
                    matchingHandlers.AddRange(entries);
                }
            }
        }

        if (matchingHandlers.Count == 0)
        {
            _logger.LogWarning("弱类型事件无订阅者：{EventKey}", eventKey);
            return;
        }

        // 将 eventData 序列化为 JsonElement，传递给各 handler
        var json = JsonSerializer.Serialize(eventData);
        var payload = JsonSerializer.Deserialize<JsonElement>(json);

        var tasks = matchingHandlers.Select(async entry =>
        {
            try
            {
                await entry.Handler(payload);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "弱类型事件处理失败：{EventKey}", eventKey);
            }
        });

        await Task.WhenAll(tasks);
    }

    /// <summary>
    /// 订阅弱类型事件，支持通配符 *
    /// 示例: "11365281228127823.payment.*" 匹配所有支付事件
    ///       "*.payment.completed" 匹配任意插件的 payment.completed 事件
    /// </summary>
    public IDisposable Subscribe(string eventKey, Func<object, Task> handler)
    {
        var entries = _weakHandlers.GetOrAdd(eventKey, _ => new List<WeakHandlerEntry>());
        var entry = new WeakHandlerEntry(handler);

        lock (entries)
        {
            entries.Add(entry);
        }

        _logger.LogInformation("订阅弱类型事件：{EventKey}", eventKey);

        return new Subscription(() =>
        {
            lock (entries)
            {
                entries.Remove(entry);
            }
            _logger.LogInformation("取消订阅弱类型事件：{EventKey}", eventKey);
        });
    }

    /// <summary>
    /// 取消指定 eventKey 的所有弱类型订阅
    /// </summary>
    public void UnsubscribeAll(string eventKey)
    {
        _weakHandlers.TryRemove(eventKey, out _);
        _logger.LogInformation("取消所有弱类型订阅：{EventKey}", eventKey);
    }

    // ========================================================================
    // 辅助方法
    // ========================================================================

    /// <summary>
    /// 简单的通配符匹配：* 匹配零个或多个任意字符
    /// </summary>
    private static bool MatchEventKey(string pattern, string actual)
    {
        if (pattern == actual) return true;
        if (pattern == "*") return true;

        // 支持 "*.suffix" 和 "prefix.*" 模式
        if (pattern.StartsWith("*."))
        {
            var suffix = pattern[2..];
            return actual.EndsWith(suffix, StringComparison.OrdinalIgnoreCase);
        }
        if (pattern.EndsWith(".*"))
        {
            var prefix = pattern[..^2];
            return actual.StartsWith(prefix, StringComparison.OrdinalIgnoreCase);
        }

        return false;
    }

    public void Dispose()
    {
        _typedHandlers.Clear();
        _weakHandlers.Clear();
    }

    // ========================================================================
    // 内部类型
    // ========================================================================

    private record HandlerEntry(IEventHandlerInternal Handler);
    private record WeakHandlerEntry(Func<object, Task> Handler);

    /// <summary>
    /// 订阅句柄，Dispose 时自动取消订阅
    /// </summary>
    private sealed class Subscription : IDisposable
    {
        private readonly Action _onDispose;
        public Subscription(Action onDispose) => _onDispose = onDispose;
        public void Dispose() => _onDispose();
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
