using System.Reflection;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Fastdotnet.Core.Notification.Infrastructure;

/// <summary>
/// CloudEvents 1.0 规范信封。
/// 所有通过 SysOutbox 投递的事件载荷均以此格式包装。
/// </summary>
public sealed class CloudEventEnvelope
{
    [JsonPropertyName("specversion")]
    public string SpecVersion { get; set; } = "1.0";

    [JsonPropertyName("id")]
    public string Id { get; set; } = string.Empty;

    [JsonPropertyName("source")]
    public string Source { get; set; } = string.Empty;

    [JsonPropertyName("type")]
    public string Type { get; set; } = string.Empty;

    [JsonPropertyName("time")]
    public DateTimeOffset Time { get; set; }

    [JsonPropertyName("datacontenttype")]
    public string DataContentType { get; set; } = "application/json";

    [JsonPropertyName("data")]
    public JsonElement Data { get; set; }

    /// <summary>
    /// 从事件对象构建 CloudEvents 信封
    /// </summary>
    public static CloudEventEnvelope FromEvent<T>(T @event, string source, JsonSerializerOptions? options = null) where T : IFastdotnetEvent
    {
        options ??= new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase };
        var dataElement = JsonSerializer.SerializeToElement(@event, @event.GetType(), options);

        // 从 [EventContract] 特性读取事件类型
        var eventType = @event.GetType().GetCustomAttribute<EventContractAttribute>()?.EventType
                        ?? @event.GetType().FullName
                        ?? @event.GetType().Name;

        return new CloudEventEnvelope
        {
            Id = Guid.NewGuid().ToString(),
            Source = source,
            Type = eventType,
            Time = DateTimeOffset.UtcNow,
            Data = dataElement
        };
    }

    /// <summary>
    /// 序列化为 JSON 字符串
    /// </summary>
    public string Serialize(JsonSerializerOptions? options = null)
    {
        options ??= new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            WriteIndented = false
        };
        return JsonSerializer.Serialize(this, options);
    }
}
