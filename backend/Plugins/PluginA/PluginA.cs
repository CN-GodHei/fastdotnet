using System;
using Autofac;
using System.Threading.Tasks;
using Fastdotnet.Core.Middleware;
using Microsoft.Extensions.DependencyInjection;
using PluginA.Middleware;
// This using statement is necessary to find the extension method 'GetService'.
using Microsoft.Extensions.DependencyInjection;
using PluginA.IService;
using PluginA.Initializers;
using Microsoft.AspNetCore.SignalR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Fastdotnet.Core.Initializers;
using Fastdotnet.Plugin.Contracts.Metrics;
using PluginA.EventHandlers;
using PluginA.Services;
using PluginA.Contexts;
using Fastdotnet.Core.Extensibility.Users;

// The namespace for the WebApi project must be included to find the DynamicMiddlewareRegistry.

namespace PluginA
{
    public class PluginAImpl : PluginBase, IPluginSignalREndpoint
    {
        public override string Name => "PluginA";
        public override string Version => "1.0.0";
        public override string PluginId => "11375910391972869";

        protected override async Task OnInitializeAsync(IServiceProvider serviceProvider)
        {
            //Console.WriteLine($"[{Name}] Initializing and registering middleware...");
            
            // ========== 注册 HTTP 中间件（IDynamicMiddleware） ==========
            var registry = serviceProvider.GetService<DynamicMiddlewareRegistry>();
            if (registry != null)
            {
                // 注册原有的 HTTP 中间件
                registry.Register(typeof(PluginAMiddleware));
                
                // 注册新的 HTTP 中间件（用于调用业务操作管道）
                registry.Register(typeof(HttpToBusinessOperationMiddleware));
                
                Console.WriteLine($"[{Name}] HTTP 中间件注册成功，共 {registry.GetMiddlewareTypes().Count()} 个");
            }
            
            // ========== 注册泛型管道中间件（IPluginPipeline<TContext>） ==========
            // 1. 获取业务操作管道的注册表
            var businessOpRegistry = serviceProvider.GetService<PluginPipelineRegistry<BusinessOperationContext>>();
            if (businessOpRegistry != null)
            {
                // 2. 注册三个中间件到管道注册表
                businessOpRegistry.Register(typeof(BusinessOperationLoggingMiddleware));
                businessOpRegistry.Register(typeof(BusinessOperationValidationMiddleware));
                businessOpRegistry.Register(typeof(BusinessOperationPerformanceMiddleware));
                
                Console.WriteLine($"[{Name}] 业务操作管道中间件注册成功，共 {businessOpRegistry.GetPipelineTypes().Count()} 个中间件");
            }
            else
            {
                Console.WriteLine($"[{Name}] 警告：未找到 PluginPipelineRegistry<BusinessOperationContext>，管道中间件未注册");
            }
            
            return;
        }

        protected override async Task OnStartAsync()
        {
            // Placeholder for start logic
            return;
        }

        protected override async Task OnStopAsync()
        {
            // Placeholder for stop logic
            return;
        }

        /// <summary>
        /// Called when the plugin is disabled. Use this to clean up resources.
        /// </summary>
        protected override async Task OnUnloadAsync(IServiceProvider serviceProvider)
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
            
            return;
        }

        /// <summary>
        /// This method is called by the plugin system to register the plugin's own services
        /// into its dedicated DI scope.
        /// </summary>
        public override void ConfigureServices(ContainerBuilder builder)
        {
            // This is where you would register services internal to the plugin.
            // For the middleware to be activated, it also needs to be registered here.
            builder.RegisterType<PluginAMiddleware>().AsSelf().InstancePerLifetimeScope();
            builder.RegisterType<PluginAPermissionProvider>().As<IPermissionProvider>().InstancePerLifetimeScope();
            //builder.RegisterType<PluginEntityService>().As<IPluginEntityService>().InstancePerLifetimeScope();
        
            // ========== 事件总线演示：注册事件处理器 ==========
            builder.RegisterType<OrderPaidEventHandler>()
                .As<Fastdotnet.Plugin.Contracts.Events.IEventHandler<Fastdotnet.Plugin.Contracts.Events.PaymentCompletedEvent>>()
                .InstancePerLifetimeScope();
            builder.RegisterType<PluginInstalledEventHandler>()
                .As<Fastdotnet.Plugin.Contracts.Events.IEventHandler<Fastdotnet.Plugin.Contracts.Events.PluginInstalledEvent>>()
                .InstancePerLifetimeScope();
        
            // ========== 事件总线演示：注册商城订单服务（用于发布事件） ==========
            builder.RegisterType<MallOrderService>().AsSelf().InstancePerLifetimeScope();
                    
            // 注册 PluginAHub 作为服务，以便其他组件可以使用它
            // 注意：这仅在插件内部需要直接使用 Hub 实例时才需要
            builder.RegisterType<Hubs.PluginAHub>().AsSelf().InstancePerLifetimeScope();
                    
            // 注册插件的服务，用于向客户端发送消息
            builder.RegisterType<Services.PluginAMessageService>().As<IPluginAMessageService>().InstancePerLifetimeScope();
                    
            // 注册用户扩展处理器，用于处理 PluginA 的用户扩展数据
            builder.RegisterType<PluginAUserExtensionHandler>().As<IFdAppUserExtensionHandler<Entities.PluginAUserExtension>>().InstancePerLifetimeScope();
                    
            // 注册指标提供者
            builder.RegisterType<Metrics.PluginAMetricProvider>().As<IMetricProvider>().InstancePerLifetimeScope();
                    
            // 注册初始化器，用于创建插件需要的数据库表
            //builder.RegisterType<PluginAUserExtensionInitializer>().As<IStartupTask>().InstancePerLifetimeScope();
                    
            // ========== 注册泛型管道相关服务 ==========
            // 1. 注册业务操作上下文管道注册表（单例）
            builder.RegisterType<PluginPipelineRegistry<BusinessOperationContext>>()
                .AsSelf()
                .SingleInstance();
                        
            // 2. 注册业务操作上下文管道调度器（单例）
            builder.RegisterType<PluginPipelineDispatcher<BusinessOperationContext>>()
                .AsSelf()
                .SingleInstance();
                        
            // 3. 注册三个中间件（瞬态，每次执行都会创建新实例）
            builder.RegisterType<BusinessOperationLoggingMiddleware>()
                .AsSelf()
                .InstancePerLifetimeScope();
            builder.RegisterType<BusinessOperationValidationMiddleware>()
                .AsSelf()
                .InstancePerLifetimeScope();
            builder.RegisterType<BusinessOperationPerformanceMiddleware>()
                .AsSelf()
                .InstancePerLifetimeScope();
                        
            // 4. 注册业务操作执行器（用于演示和使用管道）
            builder.RegisterType<BusinessOperationExecutor>()
                .AsSelf()
                .InstancePerLifetimeScope();
                        
            // 5. 注册 HTTP 到业务操作的桥接中间件（瞬态）
            builder.RegisterType<HttpToBusinessOperationMiddleware>()
                .AsSelf()
                .InstancePerLifetimeScope();
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