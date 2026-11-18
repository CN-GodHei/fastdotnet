namespace __PLUGIN_NAME__.Providers;

/// <summary>
/// 把插件能力暴露给 AI Agent（MCP）。
/// 工具名会被规整为 fd_{pluginId}_{name}；RequiredPermission 为空表示登录即可调用。
/// </summary>
public class __PLUGIN_NAME__ToolProvider : IPluginToolProvider
{
    public IEnumerable<PluginToolDescriptor> GetTools() => new[]
    {
        new PluginToolDescriptor
        {
            Name = "query_thing",
            Title = "查询物品",
            Description = "按物品编号查询详情。当用户询问物品名称、状态或备注时使用。返回物品的编号、名称与备注。",
            Parameters = new[]
            {
                new PluginToolParameter
                {
                    Name = "thingId",
                    Type = ToolParameterType.String,
                    Required = true,
                    Description = "物品编号（实体主键）"
                }
            },
            RequiredPermission = __PLUGIN_NAME__Permissions.ThingView,
            IsDestructive = false,
            TimeoutSeconds = 15
        }
    };

    public Task<ToolInvocationResult> InvokeAsync(
        string toolName,
        ToolInvocationContext context,
        CancellationToken cancellationToken = default)
    {
        return toolName switch
        {
            "query_thing" => QueryThingAsync(context, cancellationToken),
            _ => Task.FromResult(ToolInvocationResult.Fail("TOOL_NOT_FOUND", $"未知工具：{toolName}"))
        };
    }

    private static async Task<ToolInvocationResult> QueryThingAsync(
        ToolInvocationContext context,
        CancellationToken cancellationToken)
    {
        var thingId = context.Arguments.GetValueOrDefault("thingId")?.ToString();
        if (string.IsNullOrWhiteSpace(thingId))
            return ToolInvocationResult.Fail("INVALID_ARGUMENT", "thingId 不能为空");

        // 插件自己的服务从插件 scope 解析，不要访问宿主全局容器
        var service = context.PluginServices?.Resolve<MyThingService>();
        if (service is null)
            return ToolInvocationResult.Fail("PLUGIN_NOT_READY", "插件服务不可用");

        var dto = await service.GetAsync(thingId, cancellationToken);
        return ToolInvocationResult.Ok(
            $"物品 **{dto.Name}**（{dto.Id}）\n- 备注：{dto.Remark ?? "无"}\n- 创建时间：{dto.CreatedAt:yyyy-MM-dd HH:mm}",
            dto);
    }
}
