namespace Fastdotnet.Plugin.Contracts.Events;

/// <summary>
/// 事件特性标记（用于自动发现和文档生成）
/// </summary>
[AttributeUsage(AttributeTargets.Class, Inherited = false)]
public class EventAttribute : Attribute
{
    /// <summary>
    /// 事件名称
    /// </summary>
    public string Name { get; set; } = string.Empty;
    
    /// <summary>
    /// 事件描述
    /// </summary>
    public string Description { get; set; } = string.Empty;
    
    /// <summary>
    /// 是否持久化（保存到事件存储）
    /// </summary>
    public bool Persistent { get; set; }
    
    /// <summary>
    /// 事件分组（用于分类管理）
    /// </summary>
    public string Group { get; set; } = "General";
}
