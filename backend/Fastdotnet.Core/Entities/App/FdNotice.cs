using System.ComponentModel.DataAnnotations;
using SqlSugar;

namespace Fastdotnet.Core.Entities.App
{
    /// <summary>
    /// 应用端系统通知表
    /// </summary>
    [SugarTable("fd_app_notice", "应用端系统通知")]
    [SugarIndex("index_{table}_receiver", nameof(ReceiverId), OrderByType.Asc)]
    public partial class FdNotice : BaseEntity
    {
        [SugarColumn(ColumnName = "receiver_id", ColumnDescription = "接收人ID")]
        [Required, MaxLength(50)]
        public string ReceiverId { get; set; } = default!;

        [SugarColumn(ColumnName = "title", ColumnDescription = "通知标题")]
        [Required, MaxLength(255)]
        public string Title { get; set; } = default!;

        [SugarColumn(ColumnName = "content", ColumnDescription = "通知内容", ColumnDataType = "text", IsNullable = true)]
        public string? Content { get; set; }

        [SugarColumn(ColumnName = "is_read", ColumnDescription = "是否已读(0:未读, 1:已读)", DefaultValue = "0")]
        public int IsRead { get; set; } = 0;
    }
}
