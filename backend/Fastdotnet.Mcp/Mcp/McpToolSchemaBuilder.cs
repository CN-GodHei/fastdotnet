using System.Text.Json;
using System.Text.Json.Nodes;
using Fastdotnet.Plugin.Contracts;
using ModelContextProtocol.Protocol;

namespace Fastdotnet.Mcp.Mcp;

/// <summary>
/// 把插件工具声明转换为 MCP 协议对象。
/// <para>
/// 选择「由契约手工构造 JSON Schema」而不是反射方法签名：
/// 契约即 Schema，插件作者可控，AI 生成插件时也最容易写对。
/// </para>
/// </summary>
internal static class McpToolSchemaBuilder
{
    /// <summary>稳定通道 1：列出当前可用的插件能力</summary>
    public const string CapabilitiesListToolName = "fd_capabilities_list";

    /// <summary>稳定通道 2：通用调用入口（应对客户端缓存 tools/list 的情况）</summary>
    public const string CapabilityInvokeToolName = "fd_capability_invoke";

    /// <summary>
    /// 两个「名字永不变化」的元工具。
    /// 动态工具名会随插件装载/卸载变化，很多 MCP 客户端会缓存 tools/list，
    /// 因此必须保留一个稳定入口，否则演示与真实使用都会受客户端行为影响。
    /// </summary>
    public static IReadOnlyList<Tool> BuildMetaTools() =>
    [
        new Tool
        {
            Name = CapabilitiesListToolName,
            Title = "列出可用能力",
            Description =
                "列出当前已装载、且当前用户有权调用的 Fastdotnet 插件能力（含对外工具名、说明与参数）。" +
                "当你不确定有哪些能力可用时，先调用它。",
            InputSchema = BuildObjectSchema(
                new JsonObject
                {
                    ["keyword"] = new JsonObject
                    {
                        ["type"] = "string",
                        ["description"] = "可选：按插件名或工具说明做模糊过滤"
                    }
                },
                required: [])
        },
        new Tool
        {
            Name = CapabilityInvokeToolName,
            Title = "调用插件能力",
            Description =
                "通用调用入口：按 pluginId + tool 调用插件能力，参数放在 arguments 对象中。" +
                "适用于工具列表被客户端缓存、或需要显式指定插件的场景。",
            InputSchema = BuildObjectSchema(
                new JsonObject
                {
                    ["pluginId"] = new JsonObject
                    {
                        ["type"] = "string",
                        ["description"] = "插件 ID，可由 fd_capabilities_list 获取"
                    },
                    ["tool"] = new JsonObject
                    {
                        ["type"] = "string",
                        ["description"] = "插件内工具名（不含 fd_ 前缀）"
                    },
                    ["arguments"] = new JsonObject
                    {
                        ["type"] = "object",
                        ["description"] = "工具参数对象，字段见 fd_capabilities_list 返回的参数说明",
                        ["additionalProperties"] = true
                    }
                },
                required: ["pluginId", "tool"])
        }
    ];

    /// <summary>由插件工具声明生成 inputSchema</summary>
    public static JsonElement BuildInputSchema(PluginToolDescriptor descriptor)
    {
        var properties = new JsonObject();
        var required = new JsonArray();

        foreach (var parameter in descriptor.Parameters)
        {
            if (parameter is null || string.IsNullOrWhiteSpace(parameter.Name)) continue;

            var schema = new JsonObject { ["type"] = JsonTypeName(parameter.Type) };

            if (!string.IsNullOrWhiteSpace(parameter.Description))
                schema["description"] = parameter.Description;

            if (parameter.Enum is { Count: > 0 })
            {
                var enumArray = new JsonArray();
                foreach (var value in parameter.Enum) enumArray.Add(value);
                schema["enum"] = enumArray;
            }

            if (parameter.Default is not null)
                schema["default"] = JsonValue.Create(parameter.Default.ToString());

            if (parameter.Type == ToolParameterType.Array)
                schema["items"] = new JsonObject { ["type"] = "string" };

            if (parameter.Type == ToolParameterType.Object)
                schema["additionalProperties"] = true;

            properties[parameter.Name] = schema;

            if (parameter.Required) required.Add(parameter.Name);
        }

        return BuildObjectSchema(properties, required.Select(n => n!.GetValue<string>()).ToArray());
    }

    private static JsonElement BuildObjectSchema(JsonObject properties, IReadOnlyCollection<string> required)
    {
        var requiredArray = new JsonArray();
        foreach (var name in required) requiredArray.Add(name);

        var root = new JsonObject
        {
            ["type"] = "object",
            ["properties"] = properties,
            ["required"] = requiredArray,
            ["additionalProperties"] = false
        };

        return JsonSerializer.SerializeToElement(root);
    }

    private static string JsonTypeName(ToolParameterType type) => type switch
    {
        ToolParameterType.Integer => "integer",
        ToolParameterType.Number => "number",
        ToolParameterType.Boolean => "boolean",
        ToolParameterType.Array => "array",
        ToolParameterType.Object => "object",
        _ => "string"
    };
}
