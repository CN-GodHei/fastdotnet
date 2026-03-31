namespace Fastdotnet.Plugin.Contracts.Events;

/// <summary>
/// 订单创建事件（通用标准事件，所有业务插件使用此事件）
/// </summary>
[Event(Name = "OrderCreated", Description = "订单创建", Group = "Order", Persistent = true)]
public class OrderCreatedEvent : DomainEvent
{
    public override string AggregateId => OrderId;
    
    /// <summary>
    /// 订单 ID
    /// </summary>
    public string OrderId { get; set; } = string.Empty;
    
    /// <summary>
    /// 用户 ID
    /// </summary>
    public string UserId { get; set; } = string.Empty;
    
    /// <summary>
    /// 订单金额（元）
    /// </summary>
    public decimal Amount { get; set; }
    
    /// <summary>
    /// 商品描述（微信支付用）
    /// </summary>
    public string Description { get; set; } = string.Empty;
    
    /// <summary>
    /// 商品标题（支付宝用）
    /// </summary>
    public string Subject { get; set; } = string.Empty;
    
    /// <summary>
    /// 异步通知地址
    /// </summary>
    //public string NotifyUrl { get; set; } = string.Empty;
    
    /// <summary>
    /// 扩展数据
    /// </summary>
    public Dictionary<string, string> ExtraData { get; set; } = new();
}

/// <summary>
/// 支付请求事件
/// </summary>
[Event(Name = "PaymentRequested", Description = "支付请求", Group = "Payment")]
public class PaymentRequestedEvent : DomainEvent
{
    public override string AggregateId => OrderId;
    
    /// <summary>
    /// 订单 ID
    /// </summary>
    public string OrderId { get; set; } = string.Empty;
    
    /// <summary>
    /// 支付金额
    /// </summary>
    public decimal Amount { get; set; }
    
    /// <summary>
    /// 支付方式（Alipay、WeChatPay）
    /// </summary>
    public string PaymentMethod { get; set; } = string.Empty;
    
    /// <summary>
    /// 异步通知地址
    /// </summary>
    //public string NotifyUrl { get; set; } = string.Empty;
    
    /// <summary>
    /// 扩展数据
    /// </summary>
    public Dictionary<string, string> ExtraData { get; set; } = new();
}

/// <summary>
/// 支付完成事件
/// </summary>
[Event(Name = "PaymentCompleted", Description = "支付完成", Group = "Payment", Persistent = true)]
public class PaymentCompletedEvent : DomainEvent
{
    public override string AggregateId => OrderId;
    
    /// <summary>
    /// 订单 ID
    /// </summary>
    public string OrderId { get; set; } = string.Empty;
    
    /// <summary>
    /// 支付流水号
    /// </summary>
    public string TransactionNo { get; set; } = string.Empty;
    
    /// <summary>
    /// 实付金额
    /// </summary>
    public decimal PaidAmount { get; set; }
    
    /// <summary>
    /// 支付时间
    /// </summary>
    public DateTime PaidAt { get; set; }
    
    /// <summary>
    /// 支付渠道（Alipay、WeChatPay）
    /// </summary>
    public string PaymentChannel { get; set; } = string.Empty;
    
    /// <summary>
    /// 扩展数据
    /// </summary>
    public Dictionary<string, string> ExtraData { get; set; } = new();
}

/// <summary>
/// 支付失败事件
/// </summary>
[Event(Name = "PaymentFailed", Description = "支付失败", Group = "Payment", Persistent = true)]
public class PaymentFailedEvent : DomainEvent
{
    public override string AggregateId => OrderId;
    
    /// <summary>
    /// 订单 ID
    /// </summary>
    public string OrderId { get; set; } = string.Empty;
    
    /// <summary>
    /// 错误代码
    /// </summary>
    public string ErrorCode { get; set; } = string.Empty;
    
    /// <summary>
    /// 错误消息
    /// </summary>
    public string ErrorMessage { get; set; } = string.Empty;
    
    /// <summary>
    /// 扩展数据
    /// </summary>
    public Dictionary<string, string> ExtraData { get; set; } = new();
}
