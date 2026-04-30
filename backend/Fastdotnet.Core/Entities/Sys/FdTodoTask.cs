using System.ComponentModel.DataAnnotations;
using SqlSugar;

namespace Fastdotnet.Core.Entities.Sys
{
    /// <summary>
    /// 管理端待办任务表
    /// </summary>
    [SugarTable("fd_todo_task", "管理端待办任务")]
    [SugarIndex("index_{table}_assignee", nameof(AssigneeId), OrderByType.Asc)]
    [SugarIndex("index_{table}_business", nameof(BusinessId), OrderByType.Asc)]
    public partial class FdTodoTask : BaseEntity
    {
        [SugarColumn(ColumnName = "title", ColumnDescription = "任务标题")]
        [Required, MaxLength(255)]
        public string Title { get; set; } = default!;

        [SugarColumn(ColumnName = "business_id", ColumnDescription = "业务单据ID", IsNullable = true)]
        [MaxLength(50)]
        public string? BusinessId { get; set; }

        [SugarColumn(ColumnName = "route", ColumnDescription = "前端跳转路由", IsNullable = true)]
        [MaxLength(255)]
        public string? Route { get; set; }

        [SugarColumn(ColumnName = "assignee_id", ColumnDescription = "办理人ID")]
        [Required, MaxLength(50)]
        public string AssigneeId { get; set; } = default!;

        [SugarColumn(ColumnName = "status", ColumnDescription = "状态(0:待办, 1:已办)", DefaultValue = "0")]
        public int Status { get; set; } = 0;

        [SugarColumn(ColumnName = "workflow_instance_id", ColumnDescription = "关联的工作流实例ID", IsNullable = true)]
        [MaxLength(50)]
        public string? WorkflowInstanceId { get; set; }
    }
}
