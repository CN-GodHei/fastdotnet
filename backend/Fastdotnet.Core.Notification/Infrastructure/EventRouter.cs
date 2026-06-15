using System.Collections.Concurrent;
using System.Text.RegularExpressions;

namespace Fastdotnet.Core.Notification.Infrastructure;

/// <summary>
/// 内存事件路由器。支持精确匹配和通配符（*）匹配。
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
            (_, list) => { list.Add(subscriber); return list; });
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
    /// 路由事件到所有匹配的订阅者
    /// </summary>
    public async Task RouteAsync(string eventType, string payload, CancellationToken ct = default)
    {
        var tasks = new List<Task>();

        // 精确匹配
        if (_exactSubscribers.TryGetValue(eventType, out var exactList))
        {
            foreach (var sub in exactList)
            {
                tasks.Add(SafeInvokeAsync(sub, eventType, payload, ct));
            }
        }

        // 通配符匹配
        foreach (var (pattern, sub) in _wildcardSubscribers)
        {
            if (pattern.IsMatch(eventType))
            {
                tasks.Add(SafeInvokeAsync(sub, eventType, payload, ct));
            }
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
