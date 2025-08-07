using System;
using Autofac;
using System.Threading.Tasks;
using Fastdotnet.Core.Plugin;

namespace PluginA
{
    public class PluginAImpl : IPlugin
    {
        public string Name => "PluginA";
        public string Version => "1.0.0";

        public Task InitializeAsync()
        {
            //Console.WriteLine($"[{Name}] Initializing...");
            return Task.CompletedTask;
        }

        public Task StartAsync()
        {
            //Console.WriteLine($"[{Name}] Starting...");
            return Task.CompletedTask;
        }

        public Task StopAsync()
        {
            //Console.WriteLine($"[{Name}] Stopping...");
            return Task.CompletedTask;
        }

        public Task UnloadAsync()
        {
            //Console.WriteLine($"[{Name}] Unloading...");
            return Task.CompletedTask;
        }

        public void ConfigureServices(ContainerBuilder builder)
        {
            // 可以在这里注册此插件特有的服务
            // 例如: builder.RegisterType<MyService>().As<IMyService>().SingleInstance();
        }
    }
}