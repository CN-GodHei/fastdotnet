using System.Reflection;
using Fastdotnet.Plugin.Contracts;
using Fastdotnet.Plugin.Shared.AdapterAOT;

namespace Fastdotnet.Mcp.Tools;

/// <summary>
/// Fastdotnet 内置能力：把框架自身的信息以 MCP 工具形式暴露给 AI Agent。
/// <para>
/// 这类能力不依赖插件授权，因此在任何环境都可用；
/// 插件能力与之并存，Agent 侧看到的都是统一的工具列表。
/// </para>
/// </summary>
public sealed class FastdotnetSystemToolProvider : IHostToolProvider
{
    /// <summary>查看插件清单所需的 RBAC 权限码</summary>
    public const string PluginInventoryPermission = "admin.permissions.view";

    private readonly IPluginLoadService _pluginLoadService;

    public FastdotnetSystemToolProvider(IPluginLoadService pluginLoadService)
    {
        _pluginLoadService = pluginLoadService;
    }

    public IEnumerable<PluginToolDescriptor> GetTools() =>
    [
        new PluginToolDescriptor
        {
            Name = "get_system_info",
            Title = "获取系统信息",
            Description =
                "获取当前 Fastdotnet 宿主的基本信息：框架版本、运行时版本、操作系统、已扫描插件数量、已启用插件数量。" +
                "当需要了解运行环境、排查部署问题或确认插件装载情况时使用。",
            Parameters = [],
            IsDestructive = false
        },

        new PluginToolDescriptor
        {
            Name = "list_plugins",
            Title = "列出插件清单",
            Description =
                "列出宿主 plugins 目录下扫描到的全部插件及其启用状态（插件 ID、名称、版本、是否已启用）。" +
                "当需要确认某个插件是否已安装/已启用时使用。需要管理员权限。",
            Parameters =
            [
                new PluginToolParameter
                {
                    Name = "onlyEnabled",
                    Type = ToolParameterType.Boolean,
                    Required = false,
                    Description = "为 true 时只返回已启用的插件，默认 false 返回全部",
                    Default = false
                }
            ],
            RequiredPermission = PluginInventoryPermission,
            IsDestructive = false
        }
    ];

    public async Task<ToolInvocationResult> InvokeAsync(
        string toolName,
        ToolInvocationContext context,
        CancellationToken cancellationToken = default)
    {
        return toolName switch
        {
            "get_system_info" => GetSystemInfo(),
            "list_plugins" => await ListPluginsAsync(context, cancellationToken),
            _ => ToolInvocationResult.Fail("TOOL_NOT_FOUND", $"未知工具：{toolName}")
        };
    }

    private static ToolInvocationResult GetSystemInfo()
    {
        var entryAssembly = Assembly.GetEntryAssembly();

        // 依次尝试 InformationalVersion（含语义化版本）→ FileVersion → AssemblyVersion
        var informational = entryAssembly?.GetCustomAttribute<AssemblyInformationalVersionAttribute>()?.InformationalVersion;
        var version = (informational?.Split('+')[0])
                      ?? entryAssembly?.GetCustomAttribute<AssemblyFileVersionAttribute>()?.Version
                      ?? entryAssembly?.GetName().Version?.ToString()
                      ?? "未知";

        var content =
            $"**Fastdotnet 宿主信息**\n" +
            $"- 框架版本：{version}\n" +
            $"- 运行时：{Environment.Version}\n" +
            $"- 操作系统：{Environment.OSVersion} ({(Environment.Is64BitProcess ? "64-bit" : "32-bit")})\n" +
            $"- 机器名：{Environment.MachineName}\n" +
            $"- 处理器数：{Environment.ProcessorCount}\n" +
            $"- 启动时间：{System.Diagnostics.Process.GetCurrentProcess().StartTime:yyyy-MM-dd HH:mm:ss}\n" +
            $"- 工作目录：{AppContext.BaseDirectory}";

        return ToolInvocationResult.Ok(content, new
        {
            version,
            runtime = Environment.Version.ToString(),
            os = Environment.OSVersion.ToString(),
            machineName = Environment.MachineName,
            processorCount = Environment.ProcessorCount,
            processStartTime = System.Diagnostics.Process.GetCurrentProcess().StartTime,
            baseDirectory = AppContext.BaseDirectory
        });
    }

    private async Task<ToolInvocationResult> ListPluginsAsync(
        ToolInvocationContext context,
        CancellationToken cancellationToken)
    {
        var onlyEnabled = false;
        if (context.Arguments.GetValueOrDefault("onlyEnabled") is { } raw)
        {
            bool.TryParse(raw.ToString(), out onlyEnabled);
        }

        var plugins = await _pluginLoadService.ScanPluginsAsync();
        var activeIds = _pluginLoadService.GetActivePlugins().ToHashSet(StringComparer.OrdinalIgnoreCase);

        var rows = plugins
            .Select(p => new
            {
                id = p.id,
                name = p.name,
                version = p.version,
                enabled = p.enabled || activeIds.Contains(p.id),
                active = activeIds.Contains(p.id),
                entryPoint = p.entryPoint
            })
            .Where(p => !onlyEnabled || p.enabled)
            .OrderBy(p => p.id)
            .ToList();

        if (rows.Count == 0)
        {
            return ToolInvocationResult.Ok("未扫描到任何插件。", new { count = 0, plugins = rows });
        }

        var lines = rows.Select(p =>
            $"- **{p.name}** (`{p.id}`) v{p.version} — {(p.active ? "已启用" : p.enabled ? "已启用(未加载)" : "未启用")}");
        var content = $"共 {rows.Count} 个插件：\n" + string.Join("\n", lines);

        return ToolInvocationResult.Ok(content, new { count = rows.Count, plugins = rows });
    }
}
