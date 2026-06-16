namespace Fastdotnet.Plugin.Contracts.Events;

/// <summary>
/// 事件契约特性——声明一个可被外部订阅/发现的插件事件
/// 类似 Swagger 的 [ApiController] + [Route]，标记后框架自动扫描并注册到事件目录
/// 
/// 注意：与 Fastdotnet.Core.Notification.EventContractAttribute 是不同命名空间的独立特性，
/// 后者用于框架对外推送（Webhook/SignalR），此特性用于插件间通信事件总线。
/// </summary>
[AttributeUsage(AttributeTargets.Class, Inherited = false)]
public class EventContractAttribute : Attribute
{
    /// <summary>
    /// 事件短名称（不含插件ID前缀），如 "payment.completed"
    /// 框架自动拼接为: {pluginId}.{eventName}
    /// </summary>
    public string Name { get; }

    /// <summary>
    /// 事件描述
    /// </summary>
    public string Description { get; set; } = string.Empty;

    /// <summary>
    /// 事件分组，用于分类管理
    /// </summary>
    public string Group { get; set; } = "General";

    /// <summary>
    /// 事件方向
    /// </summary>
    public EventDirection Direction { get; set; } = EventDirection.Published;

    /// <summary>
    /// 创建事件契约特性
    /// </summary>
    /// <param name="name">事件短名称，如 "payment.completed"</param>
    public EventContractAttribute(string name)
    {
        Name = name;
    }
}
