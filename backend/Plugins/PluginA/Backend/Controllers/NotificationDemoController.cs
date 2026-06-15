using PluginA.Services;

namespace PluginA.Controllers;

/// <summary>
/// Notification 推送基础设施演示控制器
/// 演示 IEventPublisher + FlushOutboxAsync 的完整使用流程
/// </summary>
[ApiController]
[Route("api/notification-demo")]
public class NotificationDemoController : ControllerBase
{
    private readonly NotificationDemoService _service;
    private readonly ILogger<NotificationDemoController> _logger;

    public NotificationDemoController(
        NotificationDemoService service,
        ILogger<NotificationDemoController> logger)
    {
        _service = service;
        _logger = logger;
    }

    /// <summary>
    /// 扣减库存（演示单事件 + 事务内落库）
    /// </summary>
    [HttpPost("inventory/deduct")]
    public async Task<IActionResult> DeductInventory([FromBody] DeductInventoryRequest request)
    {
        var opId = await _service.DeductInventoryAsync(request.ProductId, request.Quantity);
        return Ok(new
        {
            success = true,
            operationId = opId,
            message = "库存扣减完成，事件已写入 SysOutbox 等待投递"
        });
    }

    /// <summary>
    /// 发货通知（演示 Webhook 场景）
    /// </summary>
    [HttpPost("order/{orderId}/ship")]
    public async Task<IActionResult> ShipOrder(string orderId, [FromQuery] string courier = "顺丰速运")
    {
        await _service.ShipOrderAsync(orderId, courier);
        return Ok(new
        {
            success = true,
            message = $"订单 {orderId} 已发货，事件已写入 SysOutbox，将通过 SignalR/Webhook 推送给订阅方"
        });
    }

    /// <summary>
    /// 注册用户（演示多事件 + 事务内落库）
    /// </summary>
    [HttpPost("user/register")]
    public async Task<IActionResult> RegisterUser([FromBody] RegisterUserRequest request)
    {
        await _service.RegisterUserAsync(request.UserName);
        return Ok(new
        {
            success = true,
            message = $"用户 {request.UserName} 注册完成，UserRegistered + InventoryDeducted 事件已入发件箱"
        });
    }
}

public class DeductInventoryRequest
{
    public string ProductId { get; set; } = string.Empty;
    public int Quantity { get; set; }
}

public class RegisterUserRequest
{
    public string UserName { get; set; } = string.Empty;
}
