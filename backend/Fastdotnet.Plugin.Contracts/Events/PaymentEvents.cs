namespace Fastdotnet.Plugin.Contracts.Events;

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
    public string NotifyUrl { get; set; } = string.Empty;
    
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
