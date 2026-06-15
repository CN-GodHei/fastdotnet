using SqlSugar;

namespace Fastdotnet.Core.Notification.Entities;

/// <summary>
/// 消息死信表，重试超限后归仓，等待人工介入或重放
/// </summary>
[SugarTable("sys_dead_letter", "系统死信")]
public class SysDeadLetter
{
    /// <summary>继承原事件ID</summary>
    [SugarColumn(ColumnName = "id", IsPrimaryKey = true, ColumnDescription = "事件ID")]
    public string Id { get; set; } = string.Empty;

    /// <summary>租户ID</summary>
    [SugarColumn(ColumnName = "tenant_id", ColumnDescription = "租户ID")]
    public string? TenantId { get; set; }

    /// <summary>事件类型</summary>
    [SugarColumn(ColumnName = "event_type", ColumnDescription = "事件类型")]
    public string EventType { get; set; } = string.Empty;

    /// <summary>消息完整载荷</summary>
    [SugarColumn(ColumnName = "payload", ColumnDescription = "事件载荷")]
    public string Payload { get; set; } = string.Empty;

    /// <summary>最后一次失败的异常堆栈</summary>
    [SugarColumn(ColumnName = "error_message", ColumnDescription = "失败原因", IsNullable = true)]
    public string? ErrorMessage { get; set; }

    /// <summary>归入死信的时间</summary>
    [SugarColumn(ColumnName = "failed_at", ColumnDescription = "归入死信时间")]
    public DateTime FailedAt { get; set; } = DateTime.Now;
}
