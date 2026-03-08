using Fastdotnet.Plugin.Contracts.Events;

namespace Fastdotnet.PluginA.Events;

/// <summary>
/// 商城订单创建事件（PluginA 业务事件示例）
/// </summary>
[Event(Name = "MallOrderCreated", Description = "商城订单创建", Group = "Mall")]
public class MallOrderCreatedEvent : DomainEvent
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
    /// 订单金额
    /// </summary>
    public decimal TotalAmount { get; set; }
    
    /// <summary>
    /// 商品数量
    /// </summary>
    public int ItemCount { get; set; }
    
    /// <summary>
    /// 扩展数据
    /// </summary>
    public Dictionary<string, object> OrderItems { get; set; } = new();
}
