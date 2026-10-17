using Autofac;
using Fastdotnet.Plugin.Contracts;

namespace AiToolDemo;

/// <summary>
/// AI 能力演示插件：把插件内的能力暴露给 AI Agent（MCP）。
/// <para>
/// 关键点只有两步：
/// 1. 插件实现/提供一个 <see cref="IPluginToolProvider"/>；
/// 2. 在 <see cref="ConfigureServices"/> 中把它注册到插件自己的容器。
/// 宿主会自动发现并按权限暴露给 Agent，无需改动宿主代码。
/// </para>
/// </summary>
public class AiToolDemoPlugin : PluginBase
{
    public override string PluginId => "ai-tool-demo";

    public override string Name => "AI 能力演示插件";

    public override string Version => "1.0.0";

    public override void ConfigureServices(ContainerBuilder builder)
    {
        // 注册到插件自己的 LifetimeScope：宿主通过 scope.ResolveOptional<IPluginToolProvider>() 取用
        builder.RegisterType<DemoToolProvider>()
            .As<IPluginToolProvider>()
            .InstancePerLifetimeScope();
    }
}
