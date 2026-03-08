using Fastdotnet.Plugin.Contracts.Events;

namespace Fastdotnet.PluginA.Services;

/// <summary>
/// 商城订单服务（演示如何发布事件）
/// </summary>
public class MallOrderService
{
    private readonly IEventBus _eventBus;
    private readonly ILogger<MallOrderService> _logger;
    
    public MallOrderService(
        IEventBus eventBus,
        ILogger<MallOrderService> logger)
    {
        _eventBus = eventBus;
        _logger = logger;
    }
    
    /// <summary>
    /// 创建订单并发布事件
    /// </summary>
    public async Task<string> CreateOrderAsync(string userId, decimal totalAmount, int itemCount)
    {
        var orderId = $"ORD_{Guid.NewGuid():N}";
        
        _logger.LogInformation("【PluginA】创建订单：订单 ID={OrderId}, 用户={UserId}, 金额={Amount:C}", 
            orderId, userId, totalAmount);
        
        // TODO: 这里应该有实际的订单创建逻辑（保存到数据库等）
        //await Task.Delay(100); // 模拟数据库操作
        
        // 发布通用订单创建事件（供支付插件订阅）
        var orderCreatedEvent = new OrderCreatedEvent
        {
            OrderId = orderId,
            UserId = userId,
            Amount = totalAmount,
            Description = $"商城订单-{orderId}",  // 微信支付用
            Subject = $"商城订单-{orderId}",     // 支付宝用
            NotifyUrl = "https://localhost:5001/api/unified-pay/notify",
            ExtraData = new Dictionary<string, string>
            {
                ["ItemCount"] = itemCount.ToString(),
                ["CreateTime"] = DateTime.Now.ToString(),
                ["Status"] = "Pending"
            }
        };
        
        await _eventBus.PublishAsync(orderCreatedEvent);
        
        _logger.LogInformation("【PluginA】订单创建成功，已发布 OrderCreatedEvent 事件");
        
        return orderId;
    }
    
    /// <summary>
    /// 支付订单（演示发布支付请求事件）
    /// </summary>
    public async Task<bool> RequestPaymentAsync(string orderId, decimal amount, string paymentMethod = "Alipay")
    {
        _logger.LogInformation("【PluginA】发起支付请求：订单 ID={OrderId}, 金额={Amount:C}, 支付方式={Method}", 
            orderId, amount, paymentMethod);
        
        // 发布支付请求事件（支付插件会处理）
        var paymentRequestedEvent = new PaymentRequestedEvent
        {
            OrderId = orderId,
            Amount = amount,
            PaymentMethod = paymentMethod,
            NotifyUrl = "https://localhost:5001/api/pluginA/payment/notify", // TODO: 配置化
            Source = "PluginA",
            ExtraData = new Dictionary<string, string>
            {
                ["BusinessType"] = "MallOrder",
                ["UserId"] = "user123"
            }
        };
        
        await _eventBus.PublishAsync(paymentRequestedEvent);
        
        _logger.LogInformation("【PluginA】支付请求事件已发布");
        
        return true;
    }
}
