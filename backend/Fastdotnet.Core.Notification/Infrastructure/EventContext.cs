using System.Collections.Concurrent;

namespace Fastdotnet.Core.Notification.Infrastructure;

/// <summary>
/// 基于 Scoped 生命周期的事件收集器。
/// 在一个工作单元内收集所有待发布事件，事务提交时统一落库。
/// </summary>
public class EventContext
{
    private readonly ConcurrentBag<(IFastdotnetEvent Event, DateTime Timestamp)> _events = new();

    /// <summary>
    /// 当前上下文中已收集的事件数量
    /// </summary>
    public int Count => _events.Count;

    /// <summary>
    /// 添加一个事件到当前收集器
    /// </summary>
    public void Add(IFastdotnetEvent @event)
    {
        _events.Add((@event, DateTime.Now));
    }

    /// <summary>
    /// 获取并清空所有已收集的事件
    /// </summary>
    public IReadOnlyList<(IFastdotnetEvent Event, DateTime Timestamp)> Drain()
    {
        var list = _events.ToArray();
        _events.Clear();
        return list;
    }
}
