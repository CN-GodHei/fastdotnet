using Fastdotnet.Core.Models.Base;
using SqlSugar;

namespace Fastdotnet.Core.Entities.System
{
    /// <summary>
    /// 角色菜单按钮关联表
    /// </summary>
    [SugarTable("FdRoleMenuButton")]
    public class FdRoleMenuButton : BaseEntity
    {
        /// <summary>
        /// 角色ID
        /// </summary>
        public string RoleId { get; set; }

        /// <summary>
        /// 菜单按钮ID
        /// </summary>
        public string MenuButtonId { get; set; }
    }
}