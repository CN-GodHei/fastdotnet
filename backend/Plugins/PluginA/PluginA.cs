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
using PluginA.Initializers;
using Fastdotnet.PluginA.Services;
using Fastdotnet.PluginA.EventHandlers;
using Microsoft.AspNetCore.SignalR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Fastdotnet.Plugin.Contracts.Extensibility.Users;
using Fastdotnet.Core.Initializers;
using Fastdotnet.Plugin.Contracts.Metrics;

// The namespace for the WebApi project must be included to find the DynamicMiddlewareRegistry.

namespace PluginA
{
    public class PluginAImpl : IPlugin, IPluginSignalREndpoint
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
            builder.RegisterType<PluginAPermissionProvider>().As<IPermissionProvider>().InstancePerLifetimeScope();
            //builder.RegisterType<PluginEntityService>().As<IPluginEntityService>().InstancePerLifetimeScope();
            
            // ========== 事件总线演示：注册事件处理器 ==========
            builder.RegisterType<Fastdotnet.PluginA.EventHandlers.OrderPaidEventHandler>()
                .As<Fastdotnet.Plugin.Contracts.Events.IEventHandler<Fastdotnet.Plugin.Contracts.Events.PaymentCompletedEvent>>()
                .InstancePerLifetimeScope();
            builder.RegisterType<Fastdotnet.PluginA.EventHandlers.PluginInstalledEventHandler>()
                .As<Fastdotnet.Plugin.Contracts.Events.IEventHandler<Fastdotnet.Plugin.Contracts.Events.PluginInstalledEvent>>()
                .InstancePerLifetimeScope();
            
            // ========== 事件总线演示：注册商城订单服务（用于发布事件） ==========
            builder.RegisterType<Fastdotnet.PluginA.Services.MallOrderService>().AsSelf().InstancePerLifetimeScope();
            
            // 注册PluginAHub作为服务，以便其他组件可以使用它
            // 注意：这仅在插件内部需要直接使用Hub实例时才需要
            builder.RegisterType<Hubs.PluginAHub>().AsSelf().InstancePerLifetimeScope();
            
            // 注册插件的服务，用于向客户端发送消息
            builder.RegisterType<Services.PluginAMessageService>().As<IPluginAMessageService>().InstancePerLifetimeScope();
            
            // 注册用户扩展处理器，用于处理PluginA的用户扩展数据
            builder.RegisterType<PluginAUserExtensionHandler>().As<IFdAppUserExtensionHandler<Entities.PluginAUserExtension>>().InstancePerLifetimeScope();
            
            // 注册指标提供者
            builder.RegisterType<Metrics.PluginAMetricProvider>().As<IMetricProvider>().InstancePerLifetimeScope();
            
            // 注册初始化器，用于创建插件需要的数据库表
            //builder.RegisterType<PluginAUserExtensionInitializer>().As<IStartupTask>().InstancePerLifetimeScope();
        }

        /// <summary>
        /// 插件自主注册SignalR端点（文档用途，实际端点注册由主框架统一处理）
        /// </summary>
        /// <param name="endpoints">端点路由构建器</param>
        public void RegisterSignalREndpoints(IEndpointRouteBuilder endpoints)
        {
            // 注意：由于ASP.NET Core不支持动态注册SignalR端点，
            // 插件的SignalR功能通过主框架的通用Hub（UniversalHub）来实现。
            // 此方法仅作为文档用途，指示插件的SignalR端点路径。
            // 实际的端点路径：/api/signalr
            // 插件可以通过UniversalHub的插件特定方法来实现功能：
            // - SendPluginNotification(pluginId, message)
            // - JoinPluginRoom(pluginId, roomName)
            // - LeavePluginRoom(pluginId, roomName)
            // - SendToPluginRoom(pluginId, roomName, message)
            // - InvokePluginMethod(pluginId, methodName, args)
            
            Console.WriteLine($"[PluginA] SignalR端点信息：插件通过主框架的通用Hub实现SignalR功能");
        }
    }
}