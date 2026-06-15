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

        // CloudEvents Source 前缀
        services.TryAddSingleton(options.Source);

        // Scoped 事件上下文
        services.AddScoped<EventContext>();

        // Scoped 事件发布器
        services.AddScoped<IEventPublisher, EventPublisher>();

        // 事件路由器（单例）
        services.TryAddSingleton<EventRouter>();

        // AsyncAPI 契约生成器（单例）
        services.TryAddSingleton(new AsyncApiSpecGenerator(
            title: "Fastdotnet Events",
            version: "1.0.0",
            serverUrl: "/",
            serverDescription: "Fastdotnet 推送服务"
        ));

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
    /// <summary>CloudEvents Source 标识</summary>
    public string Source { get; set; } = "fastdotnet://host";

    public JsonSerializerOptions JsonSerializerOptions { get; set; } = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        WriteIndented = false
    };

    public OutboxDispatcherOptions DispatcherOptions { get; set; } = new();
}
