using Autofac;
using Fastdotnet.Plugin.Contracts;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Threading.Tasks;

namespace Fastdotnet.Plugin.Marketplace
{
    public class PluginMarketplace : IPlugin
    {
        public string Name => "Plugin.Marketplace";
        public string Version => "1.0.0";

        public Task InitializeAsync(IServiceProvider serviceProvider)
        {
            // TODO: 在此执行插件初始化逻辑，例如加载配置
            return Task.CompletedTask;
        }

        public Task StartAsync()
        {
            // TODO: 插件启动逻辑
            return Task.CompletedTask;
        }

        public Task StopAsync()
        {
            // TODO: 插件停止逻辑
            return Task.CompletedTask;
        }

        public Task UnloadAsync(IServiceProvider serviceProvider)
        {
            // TODO: 在此执行插件卸载和资源清理逻辑
            return Task.CompletedTask;
        }

        public void ConfigureServices(ContainerBuilder builder)
        {
            // 在此注册插件内部的服务
            builder.RegisterType<Services.LicenseService>().As<Services.ILicenseService>().InstancePerLifetimeScope();
        }
    }
}
