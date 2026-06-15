using Autofac;
using Microsoft.Extensions.DependencyInjection;
using Fastdotnet.Core.Middleware;
using Fastdotnet.Core.Notification;
using Fastdotnet.Core.Notification.Infrastructure;
using Fastdotnet.Plugin.Contracts.Metrics;
using Fastdotnet.Core.Extensibility.Users;
using Fastdotnet.Core.Plugin;
using Fastdotnet.Plugin.Contracts;
using Microsoft.Extensions.Logging;

namespace Plugina
{
    /// <summary>
    /// 演示插件 — 展示 Notification 推送基础设施的完整生命周期集成
    /// </summary>
    public class PluginaPlugin : PluginBase
    {
        public override string Name => "PluginA";
        public override string Version => "1.0.0";
        public override string PluginId => "11375910391972869";

        // 保存初始化时的 ServiceProvider，供后续生命周期使用
        private IServiceProvider? _serviceProvider;
        private PluginANotificationSubscriber? _subscriber;

        protected override async Task OnInitializeAsync(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;

            var pluginInfo = PluginContext.GetCurrentPluginInfo();

            var registry = serviceProvider.GetService<DynamicMiddlewareRegistry>();
            if (registry != null)
            {
                // registry.Register(typeof(YourMiddleware));
            }
        }

        protected override Task OnStartAsync()
        {
            // 插件启动：向全局 EventRouter 注册订阅者
            var router = _serviceProvider?.GetService<EventRouter>();
            if (router != null)
            {
                var loggerFactory = _serviceProvider!.GetRequiredService<ILoggerFactory>();
                var logger = loggerFactory.CreateLogger<PluginANotificationSubscriber>();
                _subscriber = new PluginANotificationSubscriber(logger);

                // 订阅本插件的所有事件（通配符）
                router.SubscribePattern("com.fastdotnet.plugina.*", _subscriber);

                Console.WriteLine($"[{Name}] Notification 订阅者已注册到 EventRouter");
            }

            return Task.CompletedTask;
        }

        protected override Task OnStopAsync()
        {
            return Task.CompletedTask;
        }

        protected override Task OnUnloadAsync(IServiceProvider serviceProvider)
        {
            // 插件卸载：从全局 EventRouter 移除本插件的所有订阅
            if (_subscriber != null)
            {
                var router = serviceProvider.GetService<EventRouter>();
                router?.UnsubscribeAll(_subscriber);

                Console.WriteLine($"[{Name}] Notification 订阅者已从 EventRouter 移除");
            }

            return Task.CompletedTask;
        }

        /// <summary>
        /// 注册插件服务到依赖注入容器
        /// </summary>
        public override void ConfigureServices(ContainerBuilder builder)
        {
            // Notification 演示：注册 NotificationDemoService
            builder.RegisterType<PluginA.Services.NotificationDemoService>()
                .InstancePerLifetimeScope();
        }
    }

    /// <summary>
    /// PluginA 专属的 Notification 订阅者演示。
    /// 插件加载时注册到全局 EventRouter，卸载时移除。
    /// </summary>
    internal sealed class PluginANotificationSubscriber : IEventSubscriber
    {
        private readonly ILogger<PluginANotificationSubscriber> _logger;

        public PluginANotificationSubscriber(ILogger<PluginANotificationSubscriber> logger)
        {
            _logger = logger;
        }

        public Task HandleAsync(string eventType, string payload, CancellationToken cancellationToken = default)
        {
            _logger.LogInformation(
                "[PluginA 订阅者] 收到事件: {EventType}, Payload: {Payload}",
                eventType,
                payload);

            // 实际场景中这里可以：
            // - 更新本地缓存
            // - 写入插件自己的数据库
            // - 发送邮件/短信/站内信
            // - 触发插件内部工作流

            return Task.CompletedTask;
        }
    }
}
