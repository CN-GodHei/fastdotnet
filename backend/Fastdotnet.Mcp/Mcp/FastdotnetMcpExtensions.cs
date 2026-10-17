using System.Diagnostics;
using System.Text.Json;
using Fastdotnet.Mcp.Tools;
using Fastdotnet.Plugin.Contracts;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using ModelContextProtocol.AspNetCore;
using ModelContextProtocol.Protocol;
using ModelContextProtocol.Server;

namespace Fastdotnet.Mcp.Mcp;

/// <summary>
/// MCP 暴露层的注册入口。
/// <para>
/// 传输选择：<b>Streamable HTTP + 无状态模式</b>。
/// 无状态意味着每个请求独立协商，<c>tools/list</c> 每请求重新计算——
/// 这正是「插件运行时装载后工具立刻可见」所需要的语义，无需依赖服务端推送通知。
/// </para>
/// </summary>
public static class FastdotnetMcpExtensions
{
    public static IServiceCollection AddFastdotnetMcp(this IServiceCollection services, IConfiguration configuration)
    {
        var options = new McpOptions();
        configuration.GetSection(McpOptions.SectionName).Bind(options);
        services.AddSingleton(options);

        services.AddSingleton<IPluginToolRegistry, PluginToolRegistry>();
        services.AddScoped<ToolCallerContextFactory>();

        // 宿主内置能力：框架自身的能力同样暴露给 Agent（不依赖插件授权）
        services.AddSingleton<IHostToolProvider, FastdotnetSystemToolProvider>();

        if (!options.Enabled) return services;

        services.AddMcpServer(o =>
            {
                o.ServerInfo = new Implementation
                {
                    Name = options.ServerName,
                    Version = options.ServerVersion
                };
                o.ServerInstructions = options.ServerInstructions;
                o.Capabilities = new ServerCapabilities
                {
                    Tools = new ToolsCapability { ListChanged = false }
                };
                o.Handlers.ListToolsHandler = HandleListToolsAsync;
                o.Handlers.CallToolHandler = HandleCallToolAsync;
            })
            .WithHttpTransport(t => t.SessionMode = HttpServerSessionMode.Stateless);

        return services;
    }

    /// <summary>映射 MCP 端点（要求已认证，复用宿主 JWT）</summary>
    public static IEndpointConventionBuilder MapFastdotnetMcp(this IEndpointRouteBuilder endpoints)
    {
        var options = endpoints.ServiceProvider.GetRequiredService<McpOptions>();
        return endpoints.MapMcp(options.EndpointPath).RequireAuthorization();
    }

    private static async ValueTask<ListToolsResult> HandleListToolsAsync(
        RequestContext<ListToolsRequestParams> request,
        CancellationToken cancellationToken)
    {
        var registry = request.Services.GetRequiredService<IPluginToolRegistry>();
        var callerFactory = request.Services.GetRequiredService<ToolCallerContextFactory>();
        var caller = await callerFactory.CreateAsync(request.User, cancellationToken);

        var tools = new List<Tool>(McpToolSchemaBuilder.BuildMetaTools());

        foreach (var registered in registry.GetVisibleTools(caller))
        {
            tools.Add(new Tool
            {
                Name = registered.QualifiedName,
                Title = string.IsNullOrWhiteSpace(registered.Descriptor.Title)
                    ? registered.Descriptor.Name
                    : registered.Descriptor.Title,
                Description = registered.Descriptor.Description,
                InputSchema = McpToolSchemaBuilder.BuildInputSchema(registered.Descriptor)
            });
        }

        return new ListToolsResult { Tools = tools };
    }

    private static async ValueTask<CallToolResult> HandleCallToolAsync(
        RequestContext<CallToolRequestParams> request,
        CancellationToken cancellationToken)
    {
        var toolName = request.Params?.Name ?? string.Empty;
        var arguments = ConvertArguments(request.Params?.Arguments);

        var registry = request.Services.GetRequiredService<IPluginToolRegistry>();
        var callerFactory = request.Services.GetRequiredService<ToolCallerContextFactory>();
        var logger = request.Services.GetRequiredService<ILoggerFactory>()
            .CreateLogger("Fastdotnet.Mcp.ToolCall");

        var caller = await callerFactory.CreateAsync(request.User, cancellationToken);
        var stopwatch = Stopwatch.StartNew();

        ToolInvocationResult result;
        try
        {
            if (string.Equals(toolName, McpToolSchemaBuilder.CapabilitiesListToolName, StringComparison.OrdinalIgnoreCase))
            {
                result = BuildCapabilitiesList(registry, caller, arguments);
            }
            else if (string.Equals(toolName, McpToolSchemaBuilder.CapabilityInvokeToolName, StringComparison.OrdinalIgnoreCase))
            {
                result = await InvokeByPluginAndToolAsync(registry, caller, arguments, cancellationToken);
            }
            else
            {
                result = await registry.InvokeAsync(toolName, arguments, caller, cancellationToken);
            }
        }
        finally
        {
            stopwatch.Stop();
        }

        // 调用审计（P0：结构化日志；落库见技术方案 P2）
        logger.LogInformation(
            "MCP tool call | tool={Tool} plugin={PluginId} user={UserId} userType={UserType} success={Success} code={Code} durationMs={DurationMs} requestId={RequestId}",
            toolName,
            ResolvePluginId(registry, caller, toolName),
            caller.UserId,
            caller.UserType,
            result.Success,
            result.ErrorCode ?? "-",
            stopwatch.ElapsedMilliseconds,
            caller.RequestId);

        return ToCallToolResult(result);
    }

    private static async Task<ToolInvocationResult> InvokeByPluginAndToolAsync(
        IPluginToolRegistry registry,
        ToolCallerContext caller,
        IReadOnlyDictionary<string, object?> arguments,
        CancellationToken cancellationToken)
    {
        var pluginId = arguments.GetValueOrDefault("pluginId")?.ToString();
        var tool = arguments.GetValueOrDefault("tool")?.ToString();

        if (string.IsNullOrWhiteSpace(pluginId) || string.IsNullOrWhiteSpace(tool))
            return ToolInvocationResult.Fail("INVALID_ARGUMENT", "pluginId 与 tool 均为必填");

        var inner = AsArgumentDictionary(arguments.GetValueOrDefault("arguments"));
        var qualifiedName = PluginToolNaming.Qualify(pluginId, tool);

        return await registry.InvokeAsync(qualifiedName, inner, caller, cancellationToken);
    }

    private static ToolInvocationResult BuildCapabilitiesList(
        IPluginToolRegistry registry,
        ToolCallerContext caller,
        IReadOnlyDictionary<string, object?> arguments)
    {
        var keyword = arguments.GetValueOrDefault("keyword")?.ToString();
        var tools = registry.GetVisibleTools(caller);

        if (!string.IsNullOrWhiteSpace(keyword))
        {
            tools = tools
                .Where(t => t.PluginDisplayName.Contains(keyword, StringComparison.OrdinalIgnoreCase)
                            || t.QualifiedName.Contains(keyword, StringComparison.OrdinalIgnoreCase)
                            || (t.Descriptor.Description?.Contains(keyword, StringComparison.OrdinalIgnoreCase) ?? false))
                .ToList();
        }

        if (tools.Count == 0)
        {
            return ToolInvocationResult.Ok(
                "当前没有可用能力。可能原因：没有已启用的插件，或当前账号缺少相应权限。");
        }

        var payload = tools.Select(t => new
        {
            tool = t.QualifiedName,
            pluginId = t.PluginId,
            pluginName = t.PluginDisplayName,
            name = t.Descriptor.Name,
            title = t.Descriptor.Title,
            description = t.Descriptor.Description,
            destructive = t.Descriptor.IsDestructive,
            parameters = t.Descriptor.Parameters.Select(p => new
            {
                name = p.Name,
                type = p.Type.ToString(),
                required = p.Required,
                description = p.Description
            })
        }).ToList();

        var json = JsonSerializer.Serialize(new
        {
            count = payload.Count,
            capabilities = payload
        }, new JsonSerializerOptions { WriteIndented = true, Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping });

        return ToolInvocationResult.Ok(json, payload);
    }

    private static CallToolResult ToCallToolResult(ToolInvocationResult result)
    {
        var text = result.Success
            ? result.Content ?? "(工具返回空结果)"
            : $"调用失败：{result.ErrorCode} - {result.ErrorMessage}";

        var callResult = new CallToolResult
        {
            Content = new List<ContentBlock> { new TextContentBlock { Text = text } },
            IsError = !result.Success
        };

        if (result.Success && result.StructuredContent is not null)
        {
            try
            {
                callResult.StructuredContent = JsonSerializer.SerializeToElement(result.StructuredContent);
            }
            catch
            {
                // 结构化输出失败不影响文本结果
            }
        }

        return callResult;
    }

    private static string ResolvePluginId(IPluginToolRegistry registry, ToolCallerContext caller, string toolName)
    {
        if (toolName.StartsWith(McpToolSchemaBuilder.CapabilitiesListToolName, StringComparison.OrdinalIgnoreCase)
            || toolName.StartsWith(McpToolSchemaBuilder.CapabilityInvokeToolName, StringComparison.OrdinalIgnoreCase))
            return "-";

        return registry.GetVisibleTools(caller)
                   .FirstOrDefault(t => string.Equals(t.QualifiedName, toolName, StringComparison.OrdinalIgnoreCase))
                   ?.PluginId
               ?? "-";
    }

    private static IReadOnlyDictionary<string, object?> ConvertArguments(IDictionary<string, JsonElement>? arguments)
    {
        var result = new Dictionary<string, object?>(StringComparer.OrdinalIgnoreCase);
        if (arguments is null) return result;

        foreach (var (key, value) in arguments)
            result[key] = ConvertJsonValue(value);

        return result;
    }

    private static object? ConvertJsonValue(JsonElement element) => element.ValueKind switch
    {
        JsonValueKind.String => element.GetString(),
        JsonValueKind.Number => element.TryGetInt64(out var l) ? l : element.GetDouble(),
        JsonValueKind.True => true,
        JsonValueKind.False => false,
        JsonValueKind.Null or JsonValueKind.Undefined => null,
        _ => element.Clone()
    };

    private static IReadOnlyDictionary<string, object?> AsArgumentDictionary(object? value)
    {
        if (value is null) return new Dictionary<string, object?>();

        if (value is JsonElement element && element.ValueKind == JsonValueKind.Object)
        {
            var parsed = JsonSerializer.Deserialize<Dictionary<string, JsonElement>>(element.GetRawText());
            return ConvertArguments(parsed);
        }

        if (value is IReadOnlyDictionary<string, object?> dict) return dict;

        return new Dictionary<string, object?>();
    }
}
