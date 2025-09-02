using Fastdotnet.Core.Initializers;
using Fastdotnet.Plugin.Shared.AdapterAOT;
using Fastdotnet.WebApi.Controllers;
using System;
using System.Threading.Tasks;

namespace Fastdotnet.WebApi.Tasks
{
    public class StartupTask : IStartupTask
    {
        private readonly IServiceProvider _serviceProvider;

        public StartupTask(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public async Task ExecuteAsync()
        {
            Console.WriteLine("Executing StartupTask...");
            using (var scope = _serviceProvider.CreateScope())
            {
                try
                {
                    // ✅ 推荐：调用服务
                    var pluginLoader = scope.ServiceProvider.GetRequiredService<IPluginLoadService>();

                    pluginLoader.StartInstalledPlugins();

                    Console.WriteLine("Test methods executed successfully.");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error in startup task: {ex.Message}");
                }
            }
        }
    }
}
