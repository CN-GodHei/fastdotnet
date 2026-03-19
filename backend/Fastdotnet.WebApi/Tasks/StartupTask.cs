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
            Console.WriteLine("开始初始化插件...");
            using (var scope = _serviceProvider.CreateScope())
            {
                try
                {
                    // ✅ 推荐：调用服务
                    var pluginLoader = scope.ServiceProvider.GetRequiredService<IPluginLoadService>();

                    await pluginLoader.StartInstalledPlugins();

                    Console.WriteLine("插件初始化完成");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error in startup task: {ex.Message}");
                }
            }
        }
    }
}
