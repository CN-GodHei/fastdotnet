using Fastdotnet.Core.Dtos.Base;
using SqlSugar;

namespace Fastdotnet.Core.Entities.System
{
    /// <summary>
    /// 角色菜单按钮关联表
    /// </summary>
    [SugarTable("fd_rolemenubutton", "角色菜单按钮关联")]
    public class FdRoleMenuButton : BaseEntity
    {
        /// <summary>
        /// 角色ID
        /// </summary>
        [SugarColumn(ColumnName = "role_id", IsNullable = false, ColumnDescription = "角色ID")]
        public string RoleId { get; set; }

        /// <summary>
        /// 菜单按钮ID
        /// </summary>
        [SugarColumn(ColumnName = "menu_button_id", IsNullable = false, ColumnDescription = "菜单按钮ID")]
        public string MenuButtonId { get; set; }
    }
}