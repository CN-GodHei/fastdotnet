using Fastdotnet.Core.Models.Base;
using SqlSugar;

namespace Fastdotnet.Core.Entities.System
{
    /// <summary>
    /// 角色权限关联表
    /// </summary>
    [SugarTable("FdRolePermission")]
    public class FdRolePermission : BaseEntity
    {
        /// <summary>
        /// 角色ID
        /// </summary>
        public string RoleId { get; set; }

        /// <summary>
        /// 权限ID
        /// </summary>
        public string PermissionId { get; set; }
    }
}
