using Fastdotnet.Plugin.Contracts.Events;

namespace PluginA.Services;

/// <summary>
/// 商城订单服务（演示两种事件发布方式）
/// </summary>
public class MallOrderService
{
    private readonly IEventBus _eventBus;
    private readonly IEventRegistry _eventRegistry;
    private readonly ILogger<MallOrderService> _logger;
    
    public MallOrderService(
        IEventBus eventBus,
        IEventRegistry eventRegistry,
        ILogger<MallOrderService> logger)
    {
        _eventBus = eventBus;
        _eventRegistry = eventRegistry;
        _logger = logger;
    }
    
    // ================================================================
    // 旧方式：强类型发布（依赖主框架 Plugin.Contracts 中的事件类型）
    // 缺点：事件定义在主框架中，加事件必须改主框架
    // ================================================================
    
    /// <summary>
    /// 创建订单并发布事件（旧方式）
    /// </summary>
    public async Task<string> CreateOrderAsync(string userId, decimal totalAmount, int itemCount)
    {
        var orderId = $"ORD_{Guid.NewGuid():N}";
        
        _logger.LogInformation("【PluginA】创建订单：订单 ID={OrderId}, 用户={UserId}, 金额={Amount:C}", 
            orderId, userId, totalAmount);
        
        var orderCreatedEvent = new OrderCreatedEvent
        {
            OrderId = orderId,
            UserId = userId,
            Amount = totalAmount,
            Description = $"商城订单-{orderId}",
            Subject = $"商城订单-{orderId}",
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
    /// 支付订单（旧方式）
    /// </summary>
    public async Task<bool> RequestPaymentAsync(string orderId, decimal amount, string paymentMethod = "Alipay")
    {
        _logger.LogInformation("【PluginA】发起支付请求：订单 ID={OrderId}, 金额={Amount:C}, 支付方式={Method}", 
            orderId, amount, paymentMethod);
        
        var paymentRequestedEvent = new PaymentRequestedEvent
        {
            OrderId = orderId,
            Amount = amount,
            PaymentMethod = paymentMethod,
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

    // ================================================================
    // 新方式：弱类型发布（跨插件通信，通过事件目录查询 eventKey）
    // 流程：访问 /api/eventcatalog/ui 查看事件目录 → 拿到完整 eventKey
    //       或通过 IEventRegistry 按事件名查询
    // eventKey 格式：{pluginId}.{eventName}，如 "11365281228127823.payment.completed"
    // ================================================================

    /// <summary>
    /// 【新方式】通过事件目录查询后发布订单创建事件
    /// 演示如何用 IEventRegistry 按事件名查找完整 eventKey
    /// </summary>
    public async Task<string> CreateOrderWeakTypedAsync(string userId, decimal totalAmount, int itemCount)
    {
        var orderId = $"ORD_{Guid.NewGuid():N}";

        // 方式一：硬编码（你从 /api/eventcatalog/ui 查到支付插件的 pluginId 后直接写死）
        // var eventKey = "11365281228127823.order.created";

        // 方式二：通过 IEventRegistry 按事件名动态查询
        var def = _eventRegistry.GetAll()
            .FirstOrDefault(d => d.EventName == "order.created");
        var eventKey = def?.EventKey ?? "order.created"; // fallback

        _logger.LogInformation("【PluginA·弱类型】发布跨插件事件：{EventKey}", eventKey);

        await _eventBus.PublishAsync(eventKey, new
        {
            OrderId = orderId,
            UserId = userId,
            Amount = totalAmount,
            Description = $"商城订单-{orderId}",
            Subject = $"商城订单-{orderId}",
            ItemCount = itemCount,
            CreateTime = DateTime.Now,
            Status = "Pending"
        });

        _logger.LogInformation("【PluginA·弱类型】事件 {EventKey} 已发布", eventKey);
        return orderId;
    }

    /// <summary>
    /// 【新方式】支付请求——硬编码完整 eventKey（从事件目录查到的）
    /// </summary>
    public async Task<bool> RequestPaymentWeakTypedAsync(string orderId, decimal amount, string paymentMethod = "Alipay")
    {
        // 直接使用完整 eventKey（从 /api/eventcatalog/ui 查到支付插件的完整 key）
        const string eventKey = "11365281228127823.payment.requested";

        _logger.LogInformation("【PluginA·弱类型】发布跨插件事件：{EventKey}", eventKey);

        await _eventBus.PublishAsync(eventKey, new
        {
            OrderId = orderId,
            Amount = amount,
            PaymentMethod = paymentMethod,
            BusinessType = "MallOrder",
            SourceName = "PluginA"
        });

        _logger.LogInformation("【PluginA·弱类型】事件 {EventKey} 已发布", eventKey);
        return true;
    }

    /// <summary>
    /// 【新方式】订阅支付完成事件（通配符匹配任意插件的 payment.completed）
    /// </summary>
    public IDisposable SubscribeToPaymentCompleted()
    {
        // 精确订阅某个插件的事件
        // var subscription = _eventBus.Subscribe("11365281228127823.payment.completed", ...);

        // 或通配符匹配所有插件的 payment.completed
        var subscription = _eventBus.Subscribe("*.payment.completed", async data =>
        {
            var json = (System.Text.Json.JsonElement)data;
            
            var orderId = json.TryGetProperty("orderId", out var o) ? o.GetString() : "?";
            var paidAmount = json.TryGetProperty("paidAmount", out var a) ? a.GetDecimal() : 0;
            
            _logger.LogInformation("【PluginA·弱类型】收到支付完成通知：订单={OrderId}, 金额={Amount:C}",
                orderId, paidAmount);
        });

        _logger.LogInformation("【PluginA·弱类型】已订阅 *.payment.completed");
        return subscription;
    }
}
