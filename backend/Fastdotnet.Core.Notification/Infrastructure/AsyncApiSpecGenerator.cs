using System.Collections.Concurrent;
using System.Reflection;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Fastdotnet.Core.Notification.Infrastructure;

/// <summary>
/// AsyncAPI 2.6 契约规范生成器。
/// 采用注册表模式：插件加载时主动注册事件类型，`/_events/spec` 从注册表生成。
/// </summary>
public sealed class AsyncApiSpecGenerator
{
    private readonly string _title;
    private readonly ConcurrentDictionary<string, Type> _eventTypes = new();

    public AsyncApiSpecGenerator(
        string title = "Fastdotnet Events",
        string version = "1.0.0",
        string serverUrl = "/",
        string serverDescription = "Fastdotnet 推送服务")
    {
        _title = title;
        Version = version;
        _serverUrl = serverUrl;
        _serverDescription = serverDescription;
    }

    public string Version { get; }

    private readonly string _serverUrl;
    private readonly string _serverDescription;

    /// <summary>
    /// 插件加载时调用：注册一个事件类型。
    /// 线程安全，可并发调用。
    /// </summary>
    public void RegisterEventType(Type eventType)
    {
        if (!eventType.IsAssignableTo(typeof(IFastdotnetEvent)))
            return;

        var attr = eventType.GetCustomAttribute<EventContractAttribute>();
        var key = attr?.EventType ?? eventType.FullName ?? eventType.Name;
        _eventTypes.TryAdd(key, eventType);
    }

    /// <summary>
    /// 插件加载时调用：批量注册程序集中所有带 [EventContract] 的 IFastdotnetEvent 类型。
    /// </summary>
    public void RegisterAssembly(Assembly assembly)
    {
        foreach (var type in assembly.GetExportedTypes())
        {
            if (type.IsAssignableTo(typeof(IFastdotnetEvent))
                && type is { IsInterface: false, IsAbstract: false }
                && type.GetCustomAttribute<EventContractAttribute>() != null)
            {
                RegisterEventType(type);
            }
        }
    }

    /// <summary>
    /// 从已注册的事件类型生成 AsyncAPI 2.6 JSON 规范。
    /// 不依赖程序集扫描。
    /// </summary>
    public string GenerateSpec()
    {
        var channels = new Dictionary<string, object>();

        foreach (var (eventTypeKey, type) in _eventTypes.OrderBy(kv => kv.Key))
        {
            var attr = type.GetCustomAttribute<EventContractAttribute>();
            var channelName = eventTypeKey.Replace('.', '/');

            channels[channelName] = new
            {
                description = attr?.Description ?? $"{type.Name} 事件",
                subscribe = new
                {
                    message = new
                    {
                        name = type.Name,
                        title = attr?.Description ?? type.Name,
                        summary = attr?.Description,
                        contentType = "application/cloudevents+json; charset=UTF-8",
                        payload = GeneratePayloadSchema(type)
                    }
                }
            };
        }

        var spec = new Dictionary<string, object>
        {
            ["asyncapi"] = "2.6.0",
            ["info"] = new
            {
                title = _title,
                version = Version,
                description = "Fastdotnet 推送系统事件契约。使用 AsyncAPI 2.6.0 规范描述所有可订阅事件。"
            },
            ["servers"] = new Dictionary<string, object>
            {
                ["production"] = new
                {
                    url = _serverUrl,
                    protocol = "websocket",
                    description = _serverDescription
                }
            },
            ["defaultContentType"] = "application/cloudevents+json",
            ["channels"] = channels
        };

        return JsonSerializer.Serialize(spec, new JsonSerializerOptions
        {
            WriteIndented = true,
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
        });
    }

    private static object GeneratePayloadSchema(Type eventType)
    {
        var properties = new Dictionary<string, object>();
        var ctor = eventType.GetConstructors()
            .OrderByDescending(c => c.GetParameters().Length)
            .FirstOrDefault();

        if (ctor != null)
        {
            foreach (var param in ctor.GetParameters())
                properties[param.Name!] = MapTypeToSchema(param.ParameterType);
        }

        foreach (var prop in eventType.GetProperties(BindingFlags.Public | BindingFlags.Instance))
        {
            if (!properties.ContainsKey(ToCamelCase(prop.Name)))
                properties[ToCamelCase(prop.Name)] = MapTypeToSchema(prop.PropertyType);
        }

        return new { type = "object", properties };
    }

    private static object MapTypeToSchema(Type type)
    {
        var schema = new Dictionary<string, object>();

        if (type == typeof(string)) schema["type"] = "string";
        else if (type == typeof(int) || type == typeof(long)) schema["type"] = "integer";
        else if (type == typeof(decimal) || type == typeof(double) || type == typeof(float)) schema["type"] = "number";
        else if (type == typeof(bool)) schema["type"] = "boolean";
        else if (type == typeof(DateTime) || type == typeof(DateTimeOffset)) schema["type"] = "string";
        else if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Dictionary<,>))
        {
            schema["type"] = "object";
            schema["additionalProperties"] = true;
        }
        else if (type.Namespace?.StartsWith("System") != true)
        {
            schema["type"] = "object";
            var props = new Dictionary<string, object>();
            foreach (var prop in type.GetProperties(BindingFlags.Public | BindingFlags.Instance))
            {
                if (prop.GetIndexParameters().Length > 0) continue;
                props[ToCamelCase(prop.Name)] = MapTypeToSchema(prop.PropertyType);
            }
            schema["properties"] = props;
        }
        else
        {
            schema["type"] = "string";
        }

        return schema;
    }

    private static string ToCamelCase(string name)
    {
        if (string.IsNullOrEmpty(name)) return name;
        return char.ToLowerInvariant(name[0]) + name[1..];
    }
}
