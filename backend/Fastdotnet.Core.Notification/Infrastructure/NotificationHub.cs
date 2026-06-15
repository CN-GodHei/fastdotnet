using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;

namespace Fastdotnet.Core.Notification.Infrastructure;

/// <summary>
/// 实时推送 SignalR Hub。前端连接此 Hub 接收实时事件通知。
/// </summary>
public class NotificationHub : Hub
{
    private readonly ILogger<NotificationHub> _logger;

    public NotificationHub(ILogger<NotificationHub> logger)
    {
        _logger = logger;
    }

    public override async Task OnConnectedAsync()
    {
        _logger.LogDebug("SignalR 客户端已连接: {ConnectionId}", Context.ConnectionId);
        await base.OnConnectedAsync();
    }

    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        _logger.LogDebug("SignalR 客户端已断开: {ConnectionId}", Context.ConnectionId);
        await base.OnDisconnectedAsync(exception);
    }
}

/// <summary>
/// SignalR 订阅器：实现 IEventSubscriber，将事件推送到所有连接的 SignalR 客户端。
/// </summary>
internal sealed class SignalRSubscriber : IEventSubscriber
{
    private readonly IHubContext<NotificationHub> _hubContext;
    private readonly ILogger<SignalRSubscriber> _logger;

    public SignalRSubscriber(IHubContext<NotificationHub> hubContext, ILogger<SignalRSubscriber> logger)
    {
        _hubContext = hubContext;
        _logger = logger;
    }

    public async Task HandleAsync(string eventType, string payload, CancellationToken cancellationToken = default)
    {
        try
        {
            await _hubContext.Clients.All.SendAsync("OnEvent", eventType, payload, cancellationToken);
            _logger.LogDebug("SignalR 推送事件: {EventType}", eventType);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "SignalR 推送失败: {EventType}", eventType);
            throw;
        }
    }
}
