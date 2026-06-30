using Fastdotnet.Plugin.Contracts.Events;

namespace PluginA.Events;

/// <summary>
/// 商城订单创建事件（PluginA 发布，支付插件等可订阅）
/// eventKey: {pluginId}.order.created
/// </summary>
[EventContract("order.created", Description = "商城订单创建", Group = "Order")]
public class OrderCreatedEvent : DomainEvent
{
    public override string AggregateId => OrderId;

    /// <summary>订单 ID</summary>
    public string OrderId { get; set; } = string.Empty;

    /// <summary>用户 ID</summary>
    public string UserId { get; set; } = string.Empty;

    /// <summary>订单金额（元）</summary>
    public decimal TotalAmount { get; set; }

    /// <summary>商品数量</summary>
    public int ItemCount { get; set; }
}

/// <summary>
/// 订单取消事件
/// eventKey: {pluginId}.order.cancelled
/// </summary>
[EventContract("order.cancelled", Description = "订单取消", Group = "Order")]
public class OrderCancelledEvent : DomainEvent
{
    public override string AggregateId => OrderId;

    /// <summary>订单 ID</summary>
    public string OrderId { get; set; } = string.Empty;

    /// <summary>取消原因</summary>
    public string Reason { get; set; } = string.Empty;

    /// <summary>取消时间</summary>
    public DateTime CancelledAt { get; set; } = DateTime.Now;
}

//[EventContract("inventory.deducted", Description = "库存扣减", Group = "Inventory")]
//public class InventoryDeductedEvent : EventBase
//{
//    /// <summary>商品 ID</summary>
//    public string ProductId { get; set; } = string.Empty;

//    /// <summary>扣减数量</summary>
//    public int Quantity { get; set; }

//    /// <summary>剩余库存</summary>
//    public int RemainingStock { get; set; }

//    /// <summary>关联订单 ID</summary>
//    public string OrderId { get; set; } = string.Empty;
//}

///// <summary>
///// 用户注册事件
///// eventKey: {pluginId}.user.registered
///// </summary>
//[EventContract("user.registered", Description = "新用户注册", Group = "User")]
//public class UserRegisteredEvent : EventBase
//{
//    /// <summary>用户 ID</summary>
//    public string UserId { get; set; } = string.Empty;

//    /// <summary>用户名</summary>
//    public string UserName { get; set; } = string.Empty;

//    /// <summary>注册时间</summary>
//    public DateTime RegisteredAt { get; set; } = DateTime.Now;
//}
