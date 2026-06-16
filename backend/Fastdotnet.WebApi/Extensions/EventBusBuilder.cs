using Fastdotnet.Plugin.Contracts.Events;
using Fastdotnet.WebApi.Services.EventBus;

namespace Fastdotnet.WebApi.Extensions;

/// <summary>
/// 事件总线构建器扩展方法
/// </summary>
public static class EventBusBuilder
{
    /// <summary>
    /// 添加事件总线服务（含事件注册表）
    /// </summary>
    public static IServiceCollection AddEventBus(this IServiceCollection services)
    {
        // 注册事件总线（单例）
        services.AddSingleton<IEventBus, InMemoryEventBus>();

        // 注册事件注册表（单例，作为 IEventRegistry 和 EventRegistry）
        services.AddSingleton<EventRegistry>();
        services.AddSingleton<IEventRegistry>(sp => sp.GetRequiredService<EventRegistry>());

        return services;
    }

    /// <summary>
    /// 初始化事件总线（自动订阅所有强类型处理器 + 扫描 EventContract）
    /// </summary>
    public static async Task InitializeEventBusAsync(this IServiceProvider serviceProvider)
    {
        var eventBus = serviceProvider.GetRequiredService<IEventBus>();
        var logger = serviceProvider.GetRequiredService<ILogger<InMemoryEventBus>>();
        var registry = serviceProvider.GetRequiredService<EventRegistry>();

        logger.LogInformation("开始初始化事件总线...");

        // ========== 强类型 Handler 自动订阅 ==========
        var handlers = serviceProvider.GetServices<object>()
            .OfType<IEventHandlerInternal>();

        var handlerList = handlers.ToList();
        logger.LogInformation("发现 {Count} 个事件处理器", handlerList.Count);

        foreach (var handler in handlerList)
        {
            var handlerType = handler.GetType();

            // 获取处理器实现的所有 IEventHandler<TEvent> 接口
            var interfaces = handlerType.GetInterfaces()
                .Where(i => i.IsGenericType &&
                           i.GetGenericTypeDefinition() == typeof(IEventHandler<>));

            foreach (var @interface in interfaces)
            {
                var eventType = @interface.GetGenericArguments()[0];

                // 调用 Subscribe<TEvent>(handler)
                var method = eventBus.GetType()
                    .GetMethod(nameof(IEventBus.Subscribe))
                    ?.MakeGenericMethod(eventType);

                method?.Invoke(eventBus, new object[] { handler });
            }
        }

        // ========== 扫描 EventContract 事件注册 ==========
        try
        {
            registry.ScanAllAssemblies();
        }
        catch (Exception ex)
        {
            logger.LogWarning(ex, "扫描 EventContract 事件时出错（非致命）");
        }

        logger.LogInformation("事件总线初始化完成，共订阅 {Count} 个处理器，发现 {EventCount} 个已注册事件",
            handlerList.Count, registry.GetAll().Count);
    }
}
