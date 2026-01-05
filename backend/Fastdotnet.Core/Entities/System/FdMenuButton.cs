using Fastdotnet.Core.Dtos.Base;
using Fastdotnet.Core.Dtos.Interfaces;
using SqlSugar;

namespace Fastdotnet.Core.Entities.System
{
    /// <summary>
    /// 菜单按钮表
    /// </summary>
    [SugarTable("fd_menu_button", "菜单按")]
    public class FdMenuButton : BaseEntity, ISoftDelete
    {
        /// <summary>
        /// 按钮名称
        /// </summary>
        [SugarColumn(ColumnName = "name", IsNullable = false, ColumnDescription = "按钮名称")]
        public string Name { get; set; }

        /// <summary>
        /// 按钮代码（用于前端标识）
        /// </summary>
        [SugarColumn(ColumnName = "code", IsNullable = false, ColumnDescription = "按钮代码（用于前端标识）")]
        public string Code { get; set; }

        /// <summary>
        /// 按钮描述
        /// </summary>
        [SugarColumn(ColumnName = "description", IsNullable = true, ColumnDescription = "按钮描述")]
        public string Description { get; set; }

        /// <summary>
        /// 关联的菜单编码
        /// </summary>
        [SugarColumn(ColumnName = "menu_code", ColumnDescription = "关联的菜单编码")]
        public string MenuCode { get; set; }

        /// <summary>
        /// 所属模块 (System/PluginId)
        /// </summary>
        [SugarColumn(ColumnName = "module", IsNullable = true, ColumnDescription = "所属模块 (System/PluginId)")]
        public string Module { get; set; }

        /// <summary>
        /// 按钮分类: Admin/App
        /// </summary>
        [SugarColumn(ColumnName = "category", IsNullable = false, Length = 50, ColumnDescription = "按钮分类: Admin/App")]
        public string Category { get; set; }

        /// <summary>
        /// 排序
        /// </summary>
        [SugarColumn(ColumnName = "sort", ColumnDescription = "排序")]
        public int Sort { get; set; }

        /// <summary>
        /// 关联的权限代码 (关联FdPermission)
        /// </summary>
        [SugarColumn(ColumnName = "permission_code", IsNullable = true, ColumnDescription = "关联的权限代码 (关联FdPermission)")]
        public string PermissionCode { get; set; }

        /// <summary>
        /// 是否启用
        /// </summary>
        [SugarColumn(ColumnName = "is_enabled", ColumnDescription = "是否启用")]
        public bool IsEnabled { get; set; } = true;
    }
}