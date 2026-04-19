using Autofac;
using Microsoft.Extensions.DependencyInjection;
using Fastdotnet.Core.Middleware;
using Fastdotnet.Plugin.Contracts.Metrics;
using Fastdotnet.Core.Extensibility.Users;
using Fastdotnet.Core.Plugin;
using Fastdotnet.Plugin.Contracts;

namespace Plugina
{
    /// <summary>
    /// 演示插件
    /// </summary>
    public class PluginaPlugin : PluginBase
    {
        public override string Name => "PluginA";
        public override string Version => "1.0.0";
        public override string PluginId => "11375910391972869";

        protected override async Task OnInitializeAsync(IServiceProvider serviceProvider)
        {
            // 获取插件信息
            var pluginInfo = PluginContext.GetCurrentPluginInfo();
            // 注册 HTTP 中间件（可选）
            var registry = serviceProvider.GetService<DynamicMiddlewareRegistry>();
            if (registry != null)
            {
                // registry.Register(typeof(YourMiddleware));
                //Console.WriteLine($"[{Name}] HTTP 中间件注册成功");
            }
            
        }

        protected override Task OnStartAsync()
        {
            // 插件启动逻辑
            return Task.CompletedTask;
        }

        protected override Task OnStopAsync()
        {
            // 插件停止逻辑
            return Task.CompletedTask;
        }

        protected override Task OnUnloadAsync(IServiceProvider serviceProvider)
        {
            // 插件卸载清理逻辑
            return Task.CompletedTask;
        }

        /// <summary>
        /// 注册插件服务到依赖注入容器
        /// </summary>
        public override void ConfigureServices(ContainerBuilder builder)
        {
            // 示例：注册服务
            // builder.RegisterType<YourService>().As<IYourService>().InstancePerLifetimeScope();
            
            // 示例：注册用户扩展处理器
            // builder.RegisterType<YourUserExtensionHandler>()
            //     .As<IFdAppUserExtensionHandler<YourUserExtension>>()
            //     .InstancePerLifetimeScope();
            
            // 示例：注册指标提供者
            // builder.RegisterType<YourMetricProvider>()
            //     .As<IMetricProvider>()
            //     .InstancePerLifetimeScope();
            
        }
    }
}
