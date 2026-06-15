using Fastdotnet.Core.Notification.Infrastructure;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using System.Text.Json;

namespace Fastdotnet.Core.Notification;

/// <summary>
/// Fastdotnet.Notification 框架注册扩展
/// </summary>
public static class NotificationServiceCollectionExtensions
{
    /// <summary>
    /// 注册 Fastdotnet 推送基础设施核心服务
    /// </summary>
    public static IServiceCollection AddFastdotnetNotification(this IServiceCollection services, Action<NotificationOptions>? configure = null)
    {
        var options = new NotificationOptions();
        configure?.Invoke(options);

        // JSON 序列化选项
        services.TryAddSingleton(options.JsonSerializerOptions);

        // Scoped 事件上下文
        services.AddScoped<EventContext>();

        // Scoped 事件发布器
        services.AddScoped<IEventPublisher, EventPublisher>();

        // 事件路由器（单例）
        services.TryAddSingleton<EventRouter>();

        // Outbox 调度器配置
        services.TryAddSingleton(options.DispatcherOptions);

        // Outbox 后台调度器
        services.AddSingleton<OutboxDispatcher>();
        services.AddHostedService(sp => sp.GetRequiredService<OutboxDispatcher>());

        // SignalR Hub 推送
        services.AddSignalR();
        services.TryAddSingleton<IEventSubscriber, SignalRSubscriber>();

        return services;
    }

    /// <summary>
    /// 注册 Webhook 订阅者
    /// </summary>
    public static IServiceCollection AddWebhookSubscriber(this IServiceCollection services, string eventType, string webhookUrl, string secretKey)
    {
        services.AddHttpClient();
        services.AddSingleton<IEventSubscriber>(sp =>
        {
            var httpClient = sp.GetRequiredService<IHttpClientFactory>().CreateClient();
            return new WebhookSubscriber(
                httpClient,
                webhookUrl,
                secretKey,
                sp.GetRequiredService<Microsoft.Extensions.Logging.ILogger<WebhookSubscriber>>());
        });

        // 将 Webhook 订阅者注册到路由器
        services.AddSingleton<IEventSubscriptionRegistration>(sp =>
            new WebhookRegistration(eventType, sp.GetRequiredService<IEventSubscriber>()));

        return services;
    }
}

/// <summary>
/// 事件订阅注册器接口
/// </summary>
public interface IEventSubscriptionRegistration
{
    void Register(EventRouter router);
}

internal sealed class WebhookRegistration : IEventSubscriptionRegistration
{
    private readonly string _eventType;
    private readonly IEventSubscriber _subscriber;

    public WebhookRegistration(string eventType, IEventSubscriber subscriber)
    {
        _eventType = eventType;
        _subscriber = subscriber;
    }

    public void Register(EventRouter router)
    {
        router.Subscribe(_eventType, _subscriber);
    }
}

/// <summary>
/// Notification 模块配置选项
/// </summary>
public class NotificationOptions
{
    public JsonSerializerOptions JsonSerializerOptions { get; set; } = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        WriteIndented = false
    };

    public OutboxDispatcherOptions DispatcherOptions { get; set; } = new();
}
