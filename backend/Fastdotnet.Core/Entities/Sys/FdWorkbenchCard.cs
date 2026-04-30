using SqlSugar;

namespace Fastdotnet.Core.Entities.Sys
{
    /// <summary>
    /// 工作台卡片定义表 (管理端)
    /// </summary>
    [SugarTable("fd_workbench_card")]
    public class FdWorkbenchCard : BaseEntity
    {
        /// <summary>
        /// 卡片名称
        /// </summary>
        [SugarColumn(ColumnDescription = "卡片名称")]
        public string Name { get; set; } = null!;

        /// <summary>
        /// 卡片描述
        /// </summary>
        [SugarColumn(IsNullable = true)]
        public string? Description { get; set; }

        /// <summary>
        /// 图标 (Lucide/Antd 图标名)
        /// </summary>
        public string? Icon { get; set; }

        /// <summary>
        /// 卡片类型 (List, Chart, Count, Custom)
        /// </summary>
        public string Type { get; set; } = "List";

        /// <summary>
        /// 数据源 API URL
        /// </summary>
        public string DataSourceUrl { get; set; } = null!;

        /// <summary>
        /// 渲染配置 (JSON 存储字段映射、颜色、阈值等)
        /// </summary>
        [SugarColumn(ColumnDataType = "text", IsNullable = true)]
        public string? ConfigJson { get; set; }

        /// <summary>
        /// 默认宽度 (网格列数)
        /// </summary>
        public int DefaultWidth { get; set; } = 4;

        /// <summary>
        /// 默认高度 (网格行数)
        /// </summary>
        public int DefaultHeight { get; set; } = 2;
    }

    /// <summary>
    /// 卡片-角色授权表 (管理端)
    /// </summary>
    [SugarTable("fd_role_card")]
    public class FdRoleCard : BaseEntity
    {
        public string RoleId { get; set; } = null!;
        public string CardId { get; set; } = null!;
    }
}
