
namespace Fastdotnet.Core.Entities.System
{
    /// <summary>
    /// 角色菜单关联表
    /// </summary>
    [SugarTable("fd_role_menu", "角色菜单关联")]
    public class FdRoleMenu : BaseEntity
    {
        /// <summary>
        /// 角色ID
        /// </summary>
        [SugarColumn(ColumnName = "role_id", IsNullable = false, ColumnDescription = "角色ID")]
        public string RoleId { get; set; }

        /// <summary>
        /// 菜单ID
        /// </summary>
        [SugarColumn(ColumnName = "menu_id", IsNullable = false, ColumnDescription = "菜单ID")]
        public string MenuId { get; set; }
    }
}