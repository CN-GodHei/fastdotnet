using Fastdotnet.Core.Initializers;
using Fastdotnet.WebApi.Controllers;
using System;
using System.Threading.Tasks;

namespace Fastdotnet.WebApi.Tasks
{
    public class TestStartupTask : IStartupTask
    {
        private readonly IServiceProvider _serviceProvider;

        public TestStartupTask(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public Task ExecuteAsync()
        {
            Console.WriteLine("Executing TestStartupTask...");
            using (var scope = _serviceProvider.CreateScope())
            {
                try
                {
                    var pluginController = scope.ServiceProvider.GetRequiredService<PluginController>();
                    pluginController.Test();
                    Console.WriteLine("Test method executed successfully from TestStartupTask.");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error executing Test method in TestStartupTask: {ex.Message}");
                }
            }
            return Task.CompletedTask;
        }
    }
}
