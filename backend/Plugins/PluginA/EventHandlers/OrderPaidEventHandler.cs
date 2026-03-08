using Fastdotnet.Plugin.Contracts.Events;

namespace Fastdotnet.PluginA.EventHandlers;

/// <summary>
/// 支付完成事件处理器（PluginA 订阅支付插件事件）
/// </summary>
public class OrderPaidEventHandler : IEventHandler<PaymentCompletedEvent>
{
    private readonly ILogger<OrderPaidEventHandler> _logger;
    
    public OrderPaidEventHandler(ILogger<OrderPaidEventHandler> logger)
    {
        _logger = logger;
    }
    
    /// <summary>
    /// 处理支付完成事件
    /// </summary>
    public async Task HandleAsync(PaymentCompletedEvent @event, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("【PluginA】收到支付完成事件：订单 ID={OrderId}, 金额={Amount:C}", 
            @event.OrderId, @event.PaidAmount);
        
        // TODO: 这里可以添加业务逻辑，例如：
        // 1. 更新订单状态为已支付
        // 2. 扣减库存
        // 3. 发送通知给用户
        // 4. 记录日志
        
        await Task.CompletedTask;
        
        _logger.LogInformation("【PluginA】支付完成事件处理完成");
    }
}
