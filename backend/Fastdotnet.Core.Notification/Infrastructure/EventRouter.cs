using System.Collections.Concurrent;
using System.Text.RegularExpressions;

namespace Fastdotnet.Core.Notification.Infrastructure;

/// <summary>
/// 内存事件路由器。支持精确匹配和通配符（*）匹配。
/// 线程安全，支持插件动态加载/卸载时的订阅管理。
/// </summary>
public class EventRouter
{
    private readonly ConcurrentDictionary<string, List<IEventSubscriber>> _exactSubscribers = new();
    private readonly ConcurrentBag<(Regex Pattern, IEventSubscriber Subscriber)> _wildcardSubscribers = new();

    /// <summary>
    /// 注册订阅者（精确事件类型）
    /// </summary>
    public void Subscribe(string eventType, IEventSubscriber subscriber)
    {
        _exactSubscribers.AddOrUpdate(
            eventType,
            _ => new List<IEventSubscriber> { subscriber },
            (_, list) => { lock (list) list.Add(subscriber); return list; });
    }

    /// <summary>
    /// 取消注册订阅者（精确事件类型）
    /// </summary>
    public void Unsubscribe(string eventType, IEventSubscriber subscriber)
    {
        if (_exactSubscribers.TryGetValue(eventType, out var list))
        {
            lock (list) list.Remove(subscriber);
            if (list.Count == 0)
                _exactSubscribers.TryRemove(eventType, out _);
        }
    }

    /// <summary>
    /// 注册通配符订阅者，如 "com.fastdotnet.*"
    /// </summary>
    public void SubscribePattern(string pattern, IEventSubscriber subscriber)
    {
        var regex = new Regex("^" + Regex.Escape(pattern).Replace("\\*", ".*") + "$",
            RegexOptions.Compiled | RegexOptions.IgnoreCase);
        _wildcardSubscribers.Add((regex, subscriber));
    }

    /// <summary>
    /// 取消注册通配符订阅者
    /// </summary>
    public void UnsubscribePattern(string pattern, IEventSubscriber subscriber)
    {
        var regex = new Regex("^" + Regex.Escape(pattern).Replace("\\*", ".*") + "$",
            RegexOptions.Compiled | RegexOptions.IgnoreCase);
        // ConcurrentBag 不支持按条件删除，重建
        var remaining = _wildcardSubscribers
            .Where(x => !(x.Pattern.ToString() == regex.ToString() && x.Subscriber == subscriber))
            .ToList();
        _wildcardSubscribers.Clear();
        foreach (var item in remaining)
            _wildcardSubscribers.Add(item);
    }

    /// <summary>
    /// 取消某个订阅者的所有订阅（插件卸载时使用）
    /// </summary>
    public void UnsubscribeAll(IEventSubscriber subscriber)
    {
        // 精确匹配
        foreach (var kvp in _exactSubscribers)
        {
            lock (kvp.Value) kvp.Value.Remove(subscriber);
        }
        // 通配符匹配
        var remaining = _wildcardSubscribers
            .Where(x => x.Subscriber != subscriber)
            .ToList();
        _wildcardSubscribers.Clear();
        foreach (var item in remaining)
            _wildcardSubscribers.Add(item);
    }

    /// <summary>
    /// 路由事件到所有匹配的订阅者
    /// </summary>
    public async Task RouteAsync(string eventType, string payload, CancellationToken ct = default)
    {
        var tasks = new List<Task>();

        // 精确匹配
        if (_exactSubscribers.TryGetValue(eventType, out var exactList))
        {
            List<IEventSubscriber> snapshot;
            lock (exactList) snapshot = exactList.ToList();
            foreach (var sub in snapshot)
                tasks.Add(SafeInvokeAsync(sub, eventType, payload, ct));
        }

        // 通配符匹配
        var wildcardSnapshot = _wildcardSubscribers.ToList();
        foreach (var (pattern, sub) in wildcardSnapshot)
        {
            if (pattern.IsMatch(eventType))
                tasks.Add(SafeInvokeAsync(sub, eventType, payload, ct));
        }

        if (tasks.Count > 0)
            await Task.WhenAll(tasks);
    }

    private static async Task SafeInvokeAsync(IEventSubscriber subscriber, string eventType, string payload, CancellationToken ct)
    {
        try
        {
            await subscriber.HandleAsync(eventType, payload, ct);
        }
        catch
        {
            // 单个订阅者失败不影响其他订阅者
        }
    }
}
