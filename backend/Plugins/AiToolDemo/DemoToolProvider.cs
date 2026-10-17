using System.Globalization;
using Fastdotnet.Plugin.Contracts;

namespace AiToolDemo;

/// <summary>
/// 演示用的工具提供者。
/// <para>
/// 两个工具分别覆盖两类场景：
/// <list type="bullet">
/// <item><c>query_order_status</c>：登录即可调用（无权限要求）</item>
/// <item><c>export_order_report</c>：需要 RBAC 权限 <c>ai.tool.demo.report.export</c>，
/// 用于验证「无权限者看不到、也调不动」</item>
/// </list>
/// </para>
/// </summary>
public class DemoToolProvider : IPluginToolProvider
{
    public const string ReportExportPermission = "ai.tool.demo.report.export";

    public IEnumerable<PluginToolDescriptor> GetTools() =>
    [
        new PluginToolDescriptor
        {
            Name = "query_order_status",
            Title = "查询订单物流状态",
            Description =
                "按订单号查询物流轨迹。当用户询问某个订单的发货、配送进度或预计送达时间时使用。" +
                "返回最新物流节点与预计送达时间。仅支持已发货订单，未发货订单会提示「暂无物流信息」。",
            Parameters =
            [
                new PluginToolParameter
                {
                    Name = "orderNo",
                    Type = ToolParameterType.String,
                    Required = true,
                    Description = "订单号，例如 A123"
                }
            ],
            IsDestructive = false
        },

        new PluginToolDescriptor
        {
            Name = "export_order_report",
            Title = "导出订单报表",
            Description =
                "导出指定日期区间的订单报表（演示数据）。当用户要求导出、汇总或统计订单数据时使用。" +
                "需要「订单报表导出」权限，无权限时调用会被拒绝。",
            Parameters =
            [
                new PluginToolParameter
                {
                    Name = "startDate",
                    Type = ToolParameterType.String,
                    Required = true,
                    Description = "开始日期，格式 yyyy-MM-dd"
                },
                new PluginToolParameter
                {
                    Name = "endDate",
                    Type = ToolParameterType.String,
                    Required = true,
                    Description = "结束日期，格式 yyyy-MM-dd"
                },
                new PluginToolParameter
                {
                    Name = "format",
                    Type = ToolParameterType.String,
                    Required = false,
                    Description = "导出格式",
                    Enum = ["csv", "xlsx"],
                    Default = "csv"
                }
            ],
            RequiredPermission = ReportExportPermission,
            IsDestructive = false
        }
    ];

    public Task<ToolInvocationResult> InvokeAsync(
        string toolName,
        ToolInvocationContext context,
        CancellationToken cancellationToken = default)
    {
        return toolName switch
        {
            "query_order_status" => Task.FromResult(QueryOrderStatus(context)),
            "export_order_report" => Task.FromResult(ExportOrderReport(context)),
            _ => Task.FromResult(ToolInvocationResult.Fail("TOOL_NOT_FOUND", $"未知工具：{toolName}"))
        };
    }

    private static ToolInvocationResult QueryOrderStatus(ToolInvocationContext context)
    {
        var orderNo = context.Arguments.GetValueOrDefault("orderNo")?.ToString();

        if (string.IsNullOrWhiteSpace(orderNo))
            return ToolInvocationResult.Fail("INVALID_ARGUMENT", "orderNo 不能为空");

        // 演示数据：真实插件应在此处调用插件自己的服务（context.PluginServices.Resolve<T>()）
        var status = orderNo.Trim().ToUpperInvariant() switch
        {
            "A123" => "已签收",
            "A124" => "派送中",
            "A125" => "运输中",
            _ => null
        };

        if (status is null)
        {
            return ToolInvocationResult.Ok(
                $"订单 {orderNo} 暂无物流信息（可能尚未发货）。",
                new { orderNo, found = false });
        }

        var now = DateTime.Now;
        var content =
            $"订单 **{orderNo}** 当前状态：{status}\n" +
            $"- 最新节点：{now:yyyy-MM-dd HH:mm} 杭州转运中心\n" +
            $"- 预计送达：{now.AddDays(1):yyyy-MM-dd}\n" +
            $"- 调用者：{context.UserName ?? context.UserId ?? "未知"}（{context.UserType}）";

        return ToolInvocationResult.Ok(content, new
        {
            orderNo,
            found = true,
            status,
            lastUpdate = now.ToString("yyyy-MM-dd HH:mm", CultureInfo.InvariantCulture),
            operatorName = context.UserName
        });
    }

    private static ToolInvocationResult ExportOrderReport(ToolInvocationContext context)
    {
        var startDate = context.Arguments.GetValueOrDefault("startDate")?.ToString();
        var endDate = context.Arguments.GetValueOrDefault("endDate")?.ToString();
        var format = context.Arguments.GetValueOrDefault("format")?.ToString() ?? "csv";

        if (string.IsNullOrWhiteSpace(startDate) || string.IsNullOrWhiteSpace(endDate))
            return ToolInvocationResult.Fail("INVALID_ARGUMENT", "startDate 与 endDate 均为必填");

        var content =
            $"已生成订单报表（演示）：{startDate} ~ {endDate}，格式 {format}\n" +
            "- 订单总数：128\n" +
            "- 已完成：121\n" +
            "- 异常：7\n" +
            $"- 生成人：{context.UserName ?? context.UserId ?? "未知"}";

        return ToolInvocationResult.Ok(content, new
        {
            startDate,
            endDate,
            format,
            total = 128,
            completed = 121,
            failed = 7
        });
    }
}
