namespace Fastdotnet.Plugin.Contracts.Events;

/// <summary>
/// 插件安装事件
/// </summary>
[Event(Name = "PluginInstalled", Description = "插件安装", Group = "System", Persistent = true)]
public class PluginInstalledEvent : EventBase
{
    /// <summary>
    /// 插件 ID
    /// </summary>
    public string PluginId { get; set; } = string.Empty;
    
    /// <summary>
    /// 插件版本
    /// </summary>
    public string PluginVersion { get; set; } = string.Empty;
}

/// <summary>
/// 插件卸载事件
/// </summary>
[Event(Name = "PluginUninstalled", Description = "插件卸载", Group = "System", Persistent = true)]
public class PluginUninstalledEvent : EventBase
{
    /// <summary>
    /// 插件 ID
    /// </summary>
    public string PluginId { get; set; } = string.Empty;
}

/// <summary>
/// 配置变更事件
/// </summary>
[Event(Name = "ConfigurationChanged", Description = "配置变更", Group = "System", Persistent = true)]
public class ConfigurationChangedEvent : EventBase
{
    /// <summary>
    /// 配置键
    /// </summary>
    public string ConfigKey { get; set; } = string.Empty;
    
    /// <summary>
    /// 旧值
    /// </summary>
    public string OldValue { get; set; } = string.Empty;
    
    /// <summary>
    /// 新值
    /// </summary>
    public string NewValue { get; set; } = string.Empty;
}
