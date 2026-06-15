using System.Net.Http.Json;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Logging;

namespace Fastdotnet.Core.Notification.Infrastructure;

/// <summary>
/// Webhook 订阅器。将事件以 HTTP POST 方式投递到外部回调地址，
/// 附带 HMAC-SHA256 签名。
/// </summary>
public class WebhookSubscriber : IEventSubscriber
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<WebhookSubscriber> _logger;
    private readonly string _webhookUrl;
    private readonly string _secretKey;

    public WebhookSubscriber(
        HttpClient httpClient,
        string webhookUrl,
        string secretKey,
        ILogger<WebhookSubscriber> logger)
    {
        _httpClient = httpClient;
        _webhookUrl = webhookUrl;
        _secretKey = secretKey;
        _logger = logger;
    }

    public async Task HandleAsync(string eventType, string payload, CancellationToken cancellationToken = default)
    {
        var timestamp = DateTimeOffset.UtcNow.ToUnixTimeSeconds().ToString();
        var signature = ComputeSignature(timestamp, payload, _secretKey);

        using var request = new HttpRequestMessage(HttpMethod.Post, _webhookUrl);
        request.Headers.Add("X-Fastdotnet-Signature", $"t={timestamp},v={signature}");
        request.Headers.Add("X-Event-Type", eventType);
        request.Content = new StringContent(payload, Encoding.UTF8, "application/json");

        var response = await _httpClient.SendAsync(request, cancellationToken);
        response.EnsureSuccessStatusCode();

        _logger.LogDebug("Webhook 投递成功: {Url} ({EventType})", _webhookUrl, eventType);
    }

    private static string ComputeSignature(string timestamp, string payload, string secret)
    {
        var data = $"{timestamp}.{payload}";
        using var hmac = new HMACSHA256(Encoding.UTF8.GetBytes(secret));
        var hash = hmac.ComputeHash(Encoding.UTF8.GetBytes(data));
        return Convert.ToHexStringLower(hash);
    }
}
