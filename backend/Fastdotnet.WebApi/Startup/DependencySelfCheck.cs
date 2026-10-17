using Fastdotnet.Core.IService;
using Fastdotnet.Mcp.Tools;
using Fastdotnet.Plugin.Shared.AdapterAOT;
using Fastdotnet.Service.IService;

namespace Fastdotnet.WebApi.Startup;

/// <summary>
/// 启动期依赖自检（fail-fast）。
/// <para>
/// 为什么需要它：容器注册错误（漏注册、生命周期不匹配、<c>null!</c> 掩盖的构造缺陷）
/// 原本要等到某个功能第一次用到时才暴露，甚至只在生产暴露。
/// 这里在启动时主动解析一遍关键服务，失败即**阻止启动**并把缺失项列清楚。
/// </para>
/// <para>
/// 注意：本项目用 Autofac 接管容器，MS DI 的 <c>ValidateOnBuild</c> 不生效，因此必须自建校验。
/// </para>
/// </summary>
public static class DependencySelfCheck
{
    /// <summary>
    /// 必须在启动时可解析的关键服务。
    /// 新增核心服务时请同步登记，避免"自检通过但服务不可用"。
    /// </summary>
    private static readonly Type[] CriticalServices =
    [
        // 身份与权限
        typeof(ICurrentUser),
        typeof(IPermissionService),
        // 插件体系
        typeof(IPluginLoadService),
        // MCP 暴露层
        typeof(IPluginToolRegistry),
        typeof(ToolCallerContextFactory),
    ];

    /// <summary>
    /// 执行自检。默认仅在 Development 环境启用，可用 <c>SelfCheck:Enabled</c> 覆盖。
    /// </summary>
    /// <exception cref="InvalidOperationException">存在无法解析的关键服务</exception>
    public static void Run(WebApplication app)
    {
        var enabled = app.Configuration.GetValue("SelfCheck:Enabled", app.Environment.IsDevelopment());
        if (!enabled) return;

        var logger = app.Services.GetRequiredService<ILoggerFactory>().CreateLogger("DependencySelfCheck");
        var failures = new List<string>();

        using var scope = app.Services.CreateScope();
        foreach (var serviceType in CriticalServices)
        {
            try
            {
                var instance = scope.ServiceProvider.GetService(serviceType);
                if (instance is null)
                {
                    failures.Add($"{serviceType.FullName}：未注册（Resolve 返回 null）");
                }
            }
            catch (Exception ex)
            {
                failures.Add($"{serviceType.FullName}：{ex.GetType().Name} - {ex.Message}");
            }
        }

        if (failures.Count > 0)
        {
            var message =
                $"启动自检失败：{failures.Count}/{CriticalServices.Length} 个关键服务无法从容器解析。" +
                Environment.NewLine + "  - " + string.Join(Environment.NewLine + "  - ", failures);

            logger.LogCritical("{Message}", message);

            // 直接抛出以阻止启动：这类问题在生产会表现为随机 500，越早失败越好
            throw new InvalidOperationException(message);
        }

        logger.LogInformation("依赖自检通过：{Count} 个关键服务均可正常解析", CriticalServices.Length);
    }
}
