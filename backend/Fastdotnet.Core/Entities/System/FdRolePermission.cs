using Fastdotnet.Core.Models.Base;
using SqlSugar;

namespace Fastdotnet.Core.Entities.System
{
    /// <summary>
    /// 角色权限关联表
    /// </summary>
    [SugarTable("fd_rolepermission", "角色权限关联")]
    public class FdRolePermission : BaseEntity
    {
        /// <summary>
        /// 角色ID
        /// </summary>
        [SugarColumn(ColumnName = "role_id", IsNullable = false, ColumnDescription = "角色ID")]
        public string RoleId { get; set; }

        /// <summary>
        /// 权限ID
        /// </summary>
        [SugarColumn(ColumnName = "permission_id", IsNullable = false, ColumnDescription = "权限ID")]
        public string PermissionId { get; set; }
    }
}
