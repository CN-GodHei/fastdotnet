using SqlSugar;

namespace Fastdotnet.Core.Notification.Entities;

/// <summary>
/// 发件箱核心表，暂存所有未投递或等待重试的事件
/// </summary>
[SugarTable("sys_outbox", "系统发件箱")]
public class SysOutbox
{
    /// <summary>主键，事件全局唯一ID</summary>
    [SugarColumn(ColumnName = "id", IsPrimaryKey = true, ColumnDescription = "事件ID")]
    public string Id { get; set; } = Guid.NewGuid().ToString("N");

    /// <summary>租户ID</summary>
    [SugarColumn(ColumnName = "tenant_id", ColumnDescription = "租户ID")]
    public string? TenantId { get; set; }

    /// <summary>来源模块/插件标识</summary>
    [SugarColumn(ColumnName = "source_module", ColumnDescription = "来源模块")]
    public string SourceModule { get; set; } = string.Empty;

    /// <summary>事件唯一标示键（如 "com.fastdotnet.order.paid.v1"）</summary>
    [SugarColumn(ColumnName = "event_type", ColumnDescription = "事件类型")]
    public string EventType { get; set; } = string.Empty;

    /// <summary>完整序列化载荷（JSON）</summary>
    [SugarColumn(ColumnName = "payload", ColumnDescription = "事件载荷")]
    public string Payload { get; set; } = string.Empty;

    /// <summary>状态：0-Pending, 1-Processing, 2-Published, 3-Failed</summary>
    [SugarColumn(ColumnName = "status", ColumnDescription = "状态")]
    public OutboxStatus Status { get; set; } = OutboxStatus.Pending;

    /// <summary>已尝试投递次数</summary>
    [SugarColumn(ColumnName = "retry_count", ColumnDescription = "重试次数")]
    public int RetryCount { get; set; } = 0;

    /// <summary>下次重试时间（指数退避）</summary>
    [SugarColumn(ColumnName = "next_attempt_time", ColumnDescription = "下次重试时间", IsNullable = true)]
    public DateTime? NextAttemptTime { get; set; }

    /// <summary>当前抢占节点实例ID</summary>
    [SugarColumn(ColumnName = "lock_id", ColumnDescription = "锁持有者", IsNullable = true)]
    public string? LockId { get; set; }

    /// <summary>锁过期时间，防止死锁</summary>
    [SugarColumn(ColumnName = "lock_expired_time", ColumnDescription = "锁过期时间", IsNullable = true)]
    public DateTime? LockExpiredTime { get; set; }

    /// <summary>创建时间</summary>
    [SugarColumn(ColumnName = "created_at", ColumnDescription = "创建时间")]
    public DateTime CreatedAt { get; set; } = DateTime.Now;
}
