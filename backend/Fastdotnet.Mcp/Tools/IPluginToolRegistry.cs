using Fastdotnet.Plugin.Contracts;

namespace Fastdotnet.Mcp.Tools;

/// <summary>
/// 插件工具注册表：聚合「当前已启用插件」声明的工具，并按调用者权限过滤。
/// </summary>
public interface IPluginToolRegistry
{
    /// <summary>
    /// 按调用者权限过滤后的可见工具。
    /// <b>每次请求调用</b>——因此插件装载/卸载后，工具集合立刻变化，无需重启宿主。
    /// </summary>
    IReadOnlyList<RegisteredPluginTool> GetVisibleTools(ToolCallerContext caller);

    /// <summary>按对外全限定名调用工具</summary>
    Task<ToolInvocationResult> InvokeAsync(
        string qualifiedName,
        IReadOnlyDictionary<string, object?> arguments,
        ToolCallerContext caller,
        CancellationToken cancellationToken = default);

    /// <summary>插件启停后触发（用于日志/指标；本设计不依赖它做客户端推送）</summary>
    event EventHandler? ToolsChanged;
}
