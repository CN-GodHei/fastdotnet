using Autofac;
using Fastdotnet.Plugin.Contracts;
using Fastdotnet.Plugin.Shared.AdapterAOT;
using Microsoft.Extensions.Logging;

namespace Fastdotnet.Mcp.Tools;

/// <summary>
/// 工具注册表：聚合「宿主内置能力」与「当前已启用插件」声明的工具，并按调用者权限过滤。
/// <para>
/// 不引入任何新的插件存储：工具声明**每次从提供者实例读取**，
/// 因此「库里的声明」和「实际运行的代码」永远不会不一致。
/// </para>
/// </summary>
public sealed class PluginToolRegistry : IPluginToolRegistry
{
    /// <summary>宿主内置能力使用的伪插件 ID</summary>
    public const string HostPluginId = "system";

    public const string HostPluginDisplayName = "Fastdotnet 内置能力";

    private readonly IPluginLoadService _pluginLoadService;
    private readonly IEnumerable<IHostToolProvider> _hostProviders;
    private readonly McpOptions _options;
    private readonly ILogger<PluginToolRegistry> _logger;

    public PluginToolRegistry(
        IPluginLoadService pluginLoadService,
        IEnumerable<IHostToolProvider> hostProviders,
        McpOptions options,
        ILogger<PluginToolRegistry> logger)
    {
        _pluginLoadService = pluginLoadService;
        _hostProviders = hostProviders;
        _options = options;
        _logger = logger;
    }

    public event EventHandler? ToolsChanged;

    /// <summary>插件启停后由宿主调用（P0 阶段仅用于触发事件，不影响 tools/list 的正确性）</summary>
    public void NotifyToolsChanged() => ToolsChanged?.Invoke(this, EventArgs.Empty);

    public IReadOnlyList<RegisteredPluginTool> GetVisibleTools(ToolCallerContext caller)
    {
        var result = new List<RegisteredPluginTool>();
        var usedNames = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

        // 1) 宿主内置能力（不依赖插件授权）
        foreach (var hostProvider in _hostProviders)
        {
            foreach (var descriptor in ReadTools(hostProvider, HostPluginId))
            {
                if (!caller.CanInvoke(descriptor)) continue;
                var qualifiedName = ReserveName(HostPluginId, descriptor.Name, usedNames);
                result.Add(new RegisteredPluginTool(qualifiedName, HostPluginId, HostPluginDisplayName, descriptor));
            }
        }

        // 2) 插件能力
        foreach (var pluginId in GetActivePluginIds())
        {
            if (!_pluginLoadService.TryGetPluginScope(pluginId, out var scope))
            {
                _logger.LogWarning("插件 {PluginId} 处于激活状态但无法获取其 LifetimeScope，已跳过", pluginId);
                continue;
            }

            IPluginToolProvider? provider;
            try
            {
                provider = scope.ResolveOptional<IPluginToolProvider>();
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "从插件 {PluginId} 解析 IPluginToolProvider 失败", pluginId);
                continue;
            }

            if (provider is null) continue;

            var displayName = GetPluginDisplayName(pluginId);

            foreach (var descriptor in ReadTools(provider, pluginId))
            {
                if (!caller.CanInvoke(descriptor)) continue;
                var qualifiedName = ReserveName(pluginId, descriptor.Name, usedNames);
                result.Add(new RegisteredPluginTool(qualifiedName, pluginId, displayName, descriptor));

                if (result.Count >= _options.MaxToolsPerList)
                {
                    _logger.LogWarning("工具数量达到上限 {Max}，其余工具未返回", _options.MaxToolsPerList);
                    return result;
                }
            }
        }

        return result;
    }

    public async Task<ToolInvocationResult> InvokeAsync(
        string qualifiedName,
        IReadOnlyDictionary<string, object?> arguments,
        ToolCallerContext caller,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(qualifiedName))
            return ToolInvocationResult.Fail("INVALID_ARGUMENT", "工具名不能为空");

        var tool = GetVisibleTools(caller)
            .FirstOrDefault(t => string.Equals(t.QualifiedName, qualifiedName, StringComparison.OrdinalIgnoreCase));

        if (tool is null)
            return ToolInvocationResult.Fail("TOOL_NOT_FOUND", $"工具不存在或当前用户无权访问：{qualifiedName}");

        // 二次鉴权：防止客户端缓存了工具列表后绕过可见性过滤
        if (!caller.CanInvoke(tool.Descriptor))
            return ToolInvocationResult.Fail("PERMISSION_DENIED", "无权限调用该工具");

        if (string.Equals(tool.PluginId, HostPluginId, StringComparison.OrdinalIgnoreCase))
        {
            var hostProvider = _hostProviders.FirstOrDefault(p =>
                ReadTools(p, HostPluginId).Any(d => string.Equals(d.Name, tool.Descriptor.Name, StringComparison.Ordinal)));

            if (hostProvider is null)
                return ToolInvocationResult.Fail("NO_TOOL_PROVIDER", "未找到对应的宿主内置能力提供者");

            return await InvokeProviderAsync(hostProvider, tool, arguments, caller, pluginScope: null, cancellationToken);
        }

        if (!_pluginLoadService.TryGetPluginScope(tool.PluginId, out var scope))
            return ToolInvocationResult.Fail("PLUGIN_NOT_LOADED", $"插件未加载或已卸载：{tool.PluginId}");

        var provider = scope.ResolveOptional<IPluginToolProvider>();
        if (provider is null)
            return ToolInvocationResult.Fail("NO_TOOL_PROVIDER", $"插件 {tool.PluginId} 未实现 IPluginToolProvider");

        return await InvokeProviderAsync(provider, tool, arguments, caller, scope, cancellationToken);
    }

    private async Task<ToolInvocationResult> InvokeProviderAsync(
        IPluginToolProvider provider,
        RegisteredPluginTool tool,
        IReadOnlyDictionary<string, object?> arguments,
        ToolCallerContext caller,
        ILifetimeScope? pluginScope,
        CancellationToken cancellationToken)
    {
        var context = new ToolInvocationContext
        {
            PluginId = tool.PluginId,
            ToolName = tool.Descriptor.Name,
            UserId = caller.UserId,
            UserName = caller.UserName,
            UserType = caller.UserType,
            IsSuperAdmin = caller.IsSuperAdmin,
            TenantId = caller.TenantId,
            RequestId = caller.RequestId,
            Arguments = arguments,
            PluginServices = pluginScope
        };

        var timeoutSeconds = tool.Descriptor.TimeoutSeconds is > 0
            ? tool.Descriptor.TimeoutSeconds!.Value
            : _options.DefaultTimeoutSeconds;

        using var timeoutCts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
        timeoutCts.CancelAfter(TimeSpan.FromSeconds(timeoutSeconds));

        try
        {
            var result = await provider.InvokeAsync(tool.Descriptor.Name, context, timeoutCts.Token);
            return Truncate(result);
        }
        catch (OperationCanceledException) when (!cancellationToken.IsCancellationRequested)
        {
            _logger.LogWarning("工具 {Tool} 调用超时（{Timeout}s）", tool.QualifiedName, timeoutSeconds);
            return ToolInvocationResult.Fail("TIMEOUT", $"工具调用超时（{timeoutSeconds}s）");
        }
        catch (Exception ex)
        {
            // 提供者异常不向 Agent 暴露细节，仅记录到宿主日志
            _logger.LogError(ex, "工具 {Tool} 调用抛出异常", tool.QualifiedName);
            return ToolInvocationResult.Fail("INTERNAL_ERROR", "工具执行失败，请联系管理员查看宿主日志");
        }
    }

    private IEnumerable<PluginToolDescriptor> ReadTools(IPluginToolProvider provider, string owner)
    {
        try
        {
            return (provider.GetTools() ?? Array.Empty<PluginToolDescriptor>())
                .Where(d => d is not null && !string.IsNullOrWhiteSpace(d.Name))
                .ToList();
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "{Owner} 的 GetTools() 抛出异常，已跳过该提供者", owner);
            return Array.Empty<PluginToolDescriptor>();
        }
    }

    private string ReserveName(string ownerId, string toolName, HashSet<string> usedNames)
    {
        var qualifiedName = PluginToolNaming.Qualify(ownerId, toolName);
        if (usedNames.Add(qualifiedName)) return qualifiedName;

        var suffix = 2;
        string candidate;
        do { candidate = $"{qualifiedName}_{suffix++}"; } while (!usedNames.Add(candidate));

        _logger.LogWarning("工具名冲突，{Original} 已重命名为 {Renamed}", qualifiedName, candidate);
        return candidate;
    }

    private IEnumerable<string> GetActivePluginIds()
    {
        try
        {
            return _pluginLoadService.GetActivePlugins().ToList();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "读取已激活插件列表失败，本次仅返回宿主内置能力");
            return Array.Empty<string>();
        }
    }

    /// <summary>按配置截断过大的返回文本，避免把宿主内存/上下文打爆</summary>
    private ToolInvocationResult Truncate(ToolInvocationResult result)
    {
        if (result?.Content is null || result.Content.Length <= _options.MaxResultBytes) return result!;

        var truncated = result.Content[.._options.MaxResultBytes] +
                        $"\n\n[结果已截断：原始长度 {result.Content.Length} 字符，上限 {_options.MaxResultBytes}]";
        return new ToolInvocationResult
        {
            Success = result.Success,
            Content = truncated,
            StructuredContent = result.StructuredContent,
            ErrorCode = result.ErrorCode,
            ErrorMessage = result.ErrorMessage
        };
    }

    private string GetPluginDisplayName(string pluginId)
    {
        try
        {
            return _pluginLoadService.GetLoadedPlugins()
                       .FirstOrDefault(p => string.Equals(p.id, pluginId, StringComparison.OrdinalIgnoreCase))
                       ?.name
                   ?? pluginId;
        }
        catch
        {
            return pluginId;
        }
    }
}
