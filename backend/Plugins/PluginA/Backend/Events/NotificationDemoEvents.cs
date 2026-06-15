using Fastdotnet.Core.Notification;

namespace PluginA.Events;

/// <summary>
/// 库存扣减事件（Notification 演示）
/// 演示使用 [EventContract] + IFastdotnetEvent 替代旧的 DomainEvent
/// </summary>
[EventContract("com.fastdotnet.plugina.inventory.deducted.v1", Description = "PluginA 库存扣减事件")]
public record InventoryDeductedEvent(
    string ProductId,
    int Quantity,
    int RemainingStock,
    string? Warehouse = null
) : IFastdotnetEvent;

/// <summary>
/// 订单发货通知事件
/// </summary>
[EventContract("com.fastdotnet.plugina.order.shipped.v1", Description = "订单发货通知")]
public record OrderShippedEvent(
    string OrderId,
    string TrackingNumber,
    string Courier,
    DateTime ShippedAt
) : IFastdotnetEvent;

/// <summary>
/// 用户注册奖励事件
/// </summary>
[EventContract("com.fastdotnet.plugina.user.registered.v1", Description = "新用户注册奖励事件")]
public record UserRegisteredEvent(
    string UserId,
    string UserName,
    DateTime RegisteredAt,
    Dictionary<string, string>? Metadata = null
) : IFastdotnetEvent;
