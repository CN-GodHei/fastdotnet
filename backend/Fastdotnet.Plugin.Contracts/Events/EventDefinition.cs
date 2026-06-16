namespace Fastdotnet.Plugin.Contracts.Events;

/// <summary>
/// 事件定义模型——用于事件目录发现
/// 每个 [EventContract] 标记的类在插件加载时被扫描并注册为 EventDefinition
/// </summary>
public class EventDefinition
{
    /// <summary>
    /// 发布此事件的插件 ID
    /// </summary>
    public string PluginId { get; set; } = string.Empty;

    /// <summary>
    /// 发布此事件的插件名称
    /// </summary>
    public string PluginName { get; set; } = string.Empty;

    /// <summary>
    /// 完整事件 Key，格式: {pluginId}.{eventName}
    /// 如 "11365281228127823.payment.completed"
    /// </summary>
    public string EventKey { get; set; } = string.Empty;

    /// <summary>
    /// 事件短名称（不含插件ID前缀）
    /// 如 "payment.completed"
    /// </summary>
    public string EventName { get; set; } = string.Empty;

    /// <summary>
    /// 事件描述
    /// </summary>
    public string Description { get; set; } = string.Empty;

    /// <summary>
    /// 事件分组
    /// </summary>
    public string Group { get; set; } = "General";

    /// <summary>
    /// 事件方向
    /// </summary>
    public EventDirection Direction { get; set; } = EventDirection.Published;

    /// <summary>
    /// 事件对应的 CLR 类型全名
    /// </summary>
    public string ClrType { get; set; } = string.Empty;

    /// <summary>
    /// Payload 属性 Schema（属性名 → 类型名）
    /// 如 { "orderId": "string", "amount": "decimal" }
    /// </summary>
    public Dictionary<string, string> PayloadSchema { get; set; } = new();
}

/// <summary>
/// 事件方向
/// </summary>
public enum EventDirection
{
    /// <summary>
    /// 插件发布此事件（外部可订阅）
    /// </summary>
    Published,

    /// <summary>
    /// 插件订阅了外部事件（声明依赖）
    /// </summary>
    Subscribed
}

/// <summary>
/// 事件注册表接口——管理所有已加载插件的事件定义
/// 类似 Swagger 的 ApiExplorer，为事件目录提供数据源
/// </summary>
public interface IEventRegistry
{
    /// <summary>
    /// 注册一个事件定义（插件加载时调用）
    /// </summary>
    /// <param name="definition">事件定义</param>
    void Register(EventDefinition definition);

    /// <summary>
    /// 获取所有已注册的事件定义
    /// </summary>
    IReadOnlyList<EventDefinition> GetAll();

    /// <summary>
    /// 按插件 ID 过滤
    /// </summary>
    /// <param name="pluginId">插件 ID</param>
    IReadOnlyList<EventDefinition> GetByPluginId(string pluginId);

    /// <summary>
    /// 按完整 EventKey 查找
    /// </summary>
    /// <param name="eventKey">完整事件 Key</param>
    EventDefinition? GetByKey(string eventKey);

    /// <summary>
    /// 取消注册指定插件的所有事件（插件卸载时调用）
    /// </summary>
    /// <param name="pluginId">插件 ID</param>
    void UnregisterByPluginId(string pluginId);
}
