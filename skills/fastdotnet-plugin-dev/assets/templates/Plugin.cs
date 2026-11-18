namespace __PLUGIN_NAME__;

/// <summary>
/// 插件入口。一个插件程序集只能有这一个 IPlugin/PluginBase 实现。
/// </summary>
public class __PLUGIN_NAME__Plugin : PluginBase
{
    public override string PluginId => "__PLUGIN_ID__";

    public override string Name => "__PLUGIN_NAME__";

    public override string Version => "1.0.0";

    protected override Task OnInitializeAsync(IServiceProvider serviceProvider)
    {
        var logger = serviceProvider.GetService<ILoggerFactory>()?.CreateLogger("__PLUGIN_NAME__");
        logger?.LogInformation("[{Plugin}] 初始化完成", Name);
        return Task.CompletedTask;
    }

    protected override Task OnStartAsync() => Task.CompletedTask;

    protected override Task OnStopAsync() => Task.CompletedTask;

    protected override Task OnUnloadAsync(IServiceProvider serviceProvider)
    {
        // 在这里反注册中间件、释放定时器、退订手写的事件订阅
        return Task.CompletedTask;
    }

    /// <summary>
    /// 注册插件自己的服务。控制器由宿主自动注册，不需要写在这里。
    /// </summary>
    public override void ConfigureServices(ContainerBuilder builder)
    {
        builder.RegisterType<MyThingService>().AsSelf().InstancePerLifetimeScope();
        builder.RegisterType<__PLUGIN_NAME__PermissionProvider>().As<IPermissionProvider>().InstancePerLifetimeScope();
        builder.RegisterType<__PLUGIN_NAME__ToolProvider>().As<IPluginToolProvider>().InstancePerLifetimeScope();
    }
}
