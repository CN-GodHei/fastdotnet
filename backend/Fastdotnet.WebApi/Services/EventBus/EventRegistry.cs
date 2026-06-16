using System.Collections.Concurrent;
using System.Reflection;
using System.Text.Json.Serialization;
using Fastdotnet.Plugin.Contracts.Events;
using Fastdotnet.Core.Plugin;
using JsonSerializer = System.Text.Json.JsonSerializer;

namespace Fastdotnet.WebApi.Services.EventBus;

/// <summary>
/// 事件注册表实现
/// 扫描已加载程序集中带 [EventContract] 的类，生成事件目录
/// </summary>
public class EventRegistry : IEventRegistry
{
    private readonly ConcurrentDictionary<string, EventDefinition> _events = new();
    private readonly ConcurrentDictionary<string, PluginInfo> _plugins = new();
    private readonly ILogger<EventRegistry> _logger;

    public EventRegistry(ILogger<EventRegistry> logger)
    {
        _logger = logger;
    }

    /// <summary>
    /// 注册一个事件定义
    /// </summary>
    public void Register(EventDefinition definition)
    {
        _events[definition.EventKey] = definition;
        _logger.LogInformation("注册事件：{EventKey} ({Description})", definition.EventKey, definition.Description);
    }

    /// <summary>
    /// 获取所有已注册事件
    /// </summary>
    public IReadOnlyList<EventDefinition> GetAll()
    {
        return _events.Values.OrderBy(e => e.PluginName).ThenBy(e => e.EventName).ToList();
    }

    /// <summary>
    /// 按插件 ID 过滤
    /// </summary>
    public IReadOnlyList<EventDefinition> GetByPluginId(string pluginId)
    {
        return _events.Values
            .Where(e => e.PluginId == pluginId)
            .OrderBy(e => e.EventName)
            .ToList();
    }

    /// <summary>
    /// 按 EventKey 查找
    /// </summary>
    public EventDefinition? GetByKey(string eventKey)
    {
        _events.TryGetValue(eventKey, out var def);
        return def;
    }

    /// <summary>
    /// 取消注册指定插件的所有事件（插件卸载时调用）
    /// </summary>
    public void UnregisterByPluginId(string pluginId)
    {
        var keys = _events.Where(kv => kv.Value.PluginId == pluginId).Select(kv => kv.Key).ToList();
        foreach (var key in keys)
        {
            _events.TryRemove(key, out _);
        }
        _logger.LogInformation("已取消插件 {PluginId} 的所有事件注册", pluginId);
    }

    /// <summary>
    /// 注册插件信息（用于事件目录展示插件名称）
    /// </summary>
    public void RegisterPlugin(PluginInfo pluginInfo)
    {
        _plugins[pluginInfo.id] = pluginInfo;
    }

    /// <summary>
    /// 扫描程序集中带 [EventContract] 特性的类并自动注册
    /// </summary>
    public void ScanAssembly(Assembly assembly, PluginInfo? pluginInfo = null)
    {
        foreach (var type in assembly.GetExportedTypes())
        {
            if (type is { IsInterface: false, IsAbstract: false })
            {
                var attr = type.GetCustomAttribute<EventContractAttribute>();
                if (attr != null)
                {
                    // 尝试从 PluginInfoCache 获取插件信息（类库项目没有直接传入 pluginInfo）
                    if (pluginInfo == null)
                    {
                        var cached = PluginInfoCache.GetPluginInfoByAssembly(assembly.GetName().Name!);
                        if (cached != null) pluginInfo = cached;
                    }

                    var pluginId = pluginInfo?.id ?? assembly.GetName().Name ?? "unknown";
                    var pluginName = pluginInfo?.name ?? assembly.GetName().Name ?? "Unknown";

                    var def = new EventDefinition
                    {
                        PluginId = pluginId,
                        PluginName = pluginName,
                        EventKey = $"{pluginId}.{attr.Name}",
                        EventName = attr.Name,
                        Description = attr.Description,
                        Group = attr.Group,
                        Direction = attr.Direction,
                        ClrType = type.FullName ?? type.Name,
                        PayloadSchema = GeneratePayloadSchema(type)
                    };

                    Register(def);
                }
            }
        }
    }

    /// <summary>
    /// 扫描所有已加载的程序集
    /// </summary>
    public void ScanAllAssemblies()
    {
        var assemblies = AppDomain.CurrentDomain.GetAssemblies()
            .Where(a => !a.IsDynamic && !a.FullName!.StartsWith("System.") && !a.FullName.StartsWith("Microsoft."));

        foreach (var assembly in assemblies)
        {
            try
            {
                ScanAssembly(assembly);
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "扫描程序集 {Assembly} 时出错", assembly.GetName().Name);
            }
        }

        _logger.LogInformation("事件目录扫描完成，共发现 {Count} 个事件", _events.Count);
    }

    /// <summary>
    /// 从类型属性反射生成 Payload Schema
    /// </summary>
    private static Dictionary<string, string> GeneratePayloadSchema(Type eventType)
    {
        var schema = new Dictionary<string, string>();

        foreach (var prop in eventType.GetProperties(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly))
        {
            if (prop.GetIndexParameters().Length > 0) continue;
            schema[ToCamelCase(prop.Name)] = MapClrTypeToSchema(prop.PropertyType);
        }

        return schema;
    }

    private static string MapClrTypeToSchema(Type type)
    {
        if (type == typeof(string)) return "string";
        if (type == typeof(int) || type == typeof(long) || type == typeof(short)) return "integer";
        if (type == typeof(decimal) || type == typeof(double) || type == typeof(float)) return "number";
        if (type == typeof(bool)) return "boolean";
        if (type == typeof(DateTime) || type == typeof(DateTimeOffset)) return "datetime";
        if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Dictionary<,>)) return "object";
        if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(List<>)) return "array";
        if (type == typeof(Guid)) return "string";

        // 对于自定义类型，标注为 object
        if (!type.Namespace?.StartsWith("System") ?? false)
            return "object";

        return "string";
    }

    private static string ToCamelCase(string name)
    {
        if (string.IsNullOrEmpty(name)) return name;
        return char.ToLowerInvariant(name[0]) + name[1..];
    }

    // ========================================================================
    // AsyncAPI 2.6 规范生成
    // ========================================================================

    /// <summary>
    /// 生成 AsyncAPI 2.6 JSON 规范（用于 /api/eventcatalog/spec）
    /// </summary>
    public string GenerateAsyncApiSpec(string title = "Fastdotnet 插件事件目录",
        string version = "1.0.0")
    {
        var channels = new Dictionary<string, object>();

        foreach (var def in _events.Values.OrderBy(e => e.PluginId).ThenBy(e => e.EventName))
        {
            var channelName = def.EventKey;

            channels[channelName] = new
            {
                description = $"[{def.EventKey}] {def.Description}（插件: {def.PluginName}）",
                subscribe = new
                {
                    message = new
                    {
                        name = def.EventName.Replace('.', '_'),
                        title = def.Description,
                        summary = $"来源: {def.PluginName} | 分组: {def.Group} | 方向: {def.Direction}",
                        contentType = "application/json; charset=UTF-8",
                        payload = new
                        {
                            type = "object",
                            properties = def.PayloadSchema.ToDictionary(
                                kv => kv.Key,
                                kv => (object)new { type = kv.Value })
                        }
                    }
                }
            };
        }

        var spec = new Dictionary<string, object>
        {
            ["asyncapi"] = "2.6.0",
            ["info"] = new
            {
                title,
                version,
                description = "Fastdotnet 插件间事件通信目录。使用 AsyncAPI 2.6 规范描述所有已加载插件的可订阅事件。"
            },
            ["servers"] = new Dictionary<string, object>
            {
                ["plugin-event-bus"] = new
                {
                    url = "/",
                    protocol = "in-process",
                    description = "插件进程内事件总线"
                }
            },
            ["defaultContentType"] = "application/json",
            ["channels"] = channels
        };

        return JsonSerializer.Serialize(spec, new JsonSerializerOptions
        {
            WriteIndented = true,
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
        });
    }
}
