using Fastdotnet.PluginA.Services;

namespace Fastdotnet.PluginA.Controllers;

/// <summary>
/// 商城订单控制器（演示事件发布）
/// </summary>
[ApiController]
[Route("api/pluginA/[controller]")]
public class MallOrderController : ControllerBase
{
    private readonly MallOrderService _orderService;
    private readonly ILogger<MallOrderController> _logger;
    
    public MallOrderController(
        MallOrderService orderService,
        ILogger<MallOrderController> logger)
    {
        _orderService = orderService;
        _logger = logger;
    }
    
    /// <summary>
    /// 创建订单并发布事件
    /// </summary>
    [HttpPost("create")]
    public async Task<IActionResult> CreateOrder([FromBody] CreateOrderRequest request)
    {
        try
        {
            _logger.LogInformation("收到创建订单请求：用户={UserId}, 金额={Amount:C}", 
                request.UserId, request.TotalAmount);
            
            var orderId = await _orderService.CreateOrderAsync(
                request.UserId, 
                request.TotalAmount, 
                request.ItemCount);
            
            return Ok(new 
            {
                success = true,
                orderId = orderId,
                message = "订单创建成功，已发布 MallOrderCreatedEvent 事件"
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "创建订单失败");
            return BadRequest(new 
            {
                success = false,
                message = ex.Message
            });
        }
    }
    
    /// <summary>
    /// 发起支付请求并发布事件
    /// </summary>
    [HttpPost("{orderId}/payment")]
    public async Task<IActionResult> RequestPayment(string orderId, [FromQuery] decimal amount)
    {
        try
        {
            _logger.LogInformation("收到支付请求：订单 ID={OrderId}, 金额={Amount:C}", orderId, amount);
            
            var result = await _orderService.RequestPaymentAsync(orderId, amount);
            
            return Ok(new 
            {
                success = result,
                message = "支付请求已发送，等待支付插件处理 PaymentRequestedEvent 事件"
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "支付请求失败");
            return BadRequest(new 
            {
                success = false,
                message = ex.Message
            });
        }
    }
}

/// <summary>
/// 创建订单请求 DTO
/// </summary>
public class CreateOrderRequest
{
    /// <summary>
    /// 用户 ID
    /// </summary>
    public string UserId { get; set; } = string.Empty;
    
    /// <summary>
    /// 订单总金额
    /// </summary>
    public decimal TotalAmount { get; set; }
    
    /// <summary>
    /// 商品数量
    /// </summary>
    public int ItemCount { get; set; }
}
