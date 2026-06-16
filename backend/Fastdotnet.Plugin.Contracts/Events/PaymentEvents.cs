namespace Fastdotnet.Plugin.Contracts.Events;

/// <summary>
/// [已废弃] 订单创建事件
/// 请改用 Fastdotnet.Pay.Events.OrderCreatedEvent（位于支付插件中）
/// 跨插件订阅请使用弱类型 API: bus.Subscribe("{pluginId}.order.created", ...)
/// </summary>
[System.Obsolete("请改用 Fastdotnet.Pay.Events.OrderCreatedEvent")]
public class OrderCreatedEvent : DomainEvent
{
    public override string AggregateId => OrderId;
    public string OrderId { get; set; } = string.Empty;
    public string UserId { get; set; } = string.Empty;
    public decimal Amount { get; set; }
    public string Description { get; set; } = string.Empty;
    public string Subject { get; set; } = string.Empty;
    public Dictionary<string, string> ExtraData { get; set; } = new();
}

/// <summary>
/// [已废弃] 支付请求事件
/// 请改用 Fastdotnet.Pay.Events.PaymentRequestedEvent
/// </summary>
[System.Obsolete("请改用 Fastdotnet.Pay.Events.PaymentRequestedEvent")]
public class PaymentRequestedEvent : DomainEvent
{
    public override string AggregateId => OrderId;
    public string OrderId { get; set; } = string.Empty;
    public decimal Amount { get; set; }
    public string PaymentMethod { get; set; } = string.Empty;
    public Dictionary<string, string> ExtraData { get; set; } = new();
}

/// <summary>
/// [已废弃] 支付完成事件
/// 请改用 Fastdotnet.Pay.Events.PaymentCompletedEvent
/// </summary>
[System.Obsolete("请改用 Fastdotnet.Pay.Events.PaymentCompletedEvent")]
public class PaymentCompletedEvent : DomainEvent
{
    public override string AggregateId => OrderId;
    public string OrderId { get; set; } = string.Empty;
    public string TransactionNo { get; set; } = string.Empty;
    public decimal PaidAmount { get; set; }
    public DateTime PaidAt { get; set; }
    public string PaymentChannel { get; set; } = string.Empty;
    public Dictionary<string, string> ExtraData { get; set; } = new();
}

/// <summary>
/// [已废弃] 支付失败事件
/// 请改用 Fastdotnet.Pay.Events.PaymentFailedEvent
/// </summary>
[System.Obsolete("请改用 Fastdotnet.Pay.Events.PaymentFailedEvent")]
public class PaymentFailedEvent : DomainEvent
{
    public override string AggregateId => OrderId;
    public string OrderId { get; set; } = string.Empty;
    public string ErrorCode { get; set; } = string.Empty;
    public string ErrorMessage { get; set; } = string.Empty;
    public Dictionary<string, string> ExtraData { get; set; } = new();
}
