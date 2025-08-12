using System;
using Autofac;
using System.Threading.Tasks;
using Fastdotnet.Core.Middleware;
using Microsoft.Extensions.DependencyInjection;
using PluginA.Middleware;
// This using statement is necessary to find the extension method 'GetService'.
using Microsoft.Extensions.DependencyInjection;
using Fastdotnet.Plugin.Contracts;
using PluginA.IService;
using PluginA.Services;

// The namespace for the WebApi project must be included to find the DynamicMiddlewareRegistry.

namespace PluginA
{
    public class PluginAImpl : IPlugin
    {
        public string Name => "PluginA";
        public string Version => "1.0.0";

        /// <summary>
        /// Called when the plugin is enabled. Use this to register services with the host.
        /// </summary>
        public Task InitializeAsync(IServiceProvider serviceProvider)
        {
            //Console.WriteLine($"[{Name}] Initializing and registering middleware...");
            
            // Get the central middleware registry from the host's services.
            var registry = serviceProvider.GetService<DynamicMiddlewareRegistry>();
            if (registry != null)
            {
                // Register this plugin's middleware type.
                registry.Register(typeof(PluginAMiddleware));
                //Console.WriteLine($"[{Name}] Middleware '{nameof(PluginAMiddleware)}' registered successfully.");
            }
            else
            {
                //Console.WriteLine($"[{Name}] ERROR: Could not find {nameof(DynamicMiddlewareRegistry)}. Middleware not registered.");
            }
            
            return Task.CompletedTask;
        }

        public Task StartAsync()
        {
            // Placeholder for start logic
            return Task.CompletedTask;
        }

        public Task StopAsync()
        {
            // Placeholder for stop logic
            return Task.CompletedTask;
        }

        /// <summary>
        /// Called when the plugin is disabled. Use this to clean up resources.
        /// </summary>
        public Task UnloadAsync(IServiceProvider serviceProvider)
        {
            //Console.WriteLine($"[{Name}] Unloading and unregistering middleware...");

            // Get the central middleware registry from the host's services.
            var registry = serviceProvider.GetService<DynamicMiddlewareRegistry>();
            // if (registry != null)
            // {
            //     // Unregister this plugin's middleware type to ensure clean removal.
            //     registry.Unregister(typeof(PluginAMiddleware));
            //     //Console.WriteLine($"[{Name}] Middleware '{nameof(PluginAMiddleware)}' unregistered successfully.");
            // }
            
            return Task.CompletedTask;
        }

        /// <summary>
        /// This method is called by the plugin system to register the plugin's own services
        /// into its dedicated DI scope.
        /// </summary>
        public void ConfigureServices(ContainerBuilder builder)
        {
            // This is where you would register services internal to the plugin.
            // For the middleware to be activated, it also needs to be registered here.
            builder.RegisterType<PluginAMiddleware>().AsSelf().InstancePerLifetimeScope();
            builder.RegisterType<PluginEntityService>().As<IPluginEntityService>().InstancePerLifetimeScope();
        }
    }
}