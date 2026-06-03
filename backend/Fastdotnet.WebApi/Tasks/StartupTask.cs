using Fastdotnet.WebApi.Services;

namespace Fastdotnet.WebApi.Tasks
{
    public class StartupTask : IStartupTask
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly PluginSwaggerDocRegistry _swaggerDocRegistry;

        public StartupTask(IServiceProvider serviceProvider, PluginSwaggerDocRegistry swaggerDocRegistry)
        {
            _serviceProvider = serviceProvider;
            _swaggerDocRegistry = swaggerDocRegistry;
        }

        public async Task ExecuteAsync()
        {
            Console.WriteLine("开始初始化插件...");
            using (var scope = _serviceProvider.CreateScope())
            {
                try
                {
                    // ✅ 推荐：调用服务
                    var pluginLoader = scope.ServiceProvider.GetRequiredService<IPluginLoadService>();

                    await pluginLoader.StartInstalledPlugins();

                    // 同步已加载的插件到 Swagger 文档注册器
                    var loadedPlugins = pluginLoader.GetLoadedPlugins();
                    _swaggerDocRegistry.BulkRegister(loadedPlugins.Select(p => (p.id, p.name, p.description)));

                    Console.WriteLine($"插件初始化完成，已同步 {loadedPlugins.Count()} 个插件文档");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error in startup task: {ex.Message}");
                }
            }
        }
    }
}
