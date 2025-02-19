using Fastdotnet.Core.Models.Base;

namespace Fastdotnet.Core.Models.Admin
{
    /// <summary>
    /// 角色权限表
    /// </summary>
    public class FdAdminRolePermission : BaseEntity
    {
        /// <summary>
        /// 角色ID
        /// </summary>
        public long RoleId { get; set; }

        /// <summary>
        /// 权限类型（1：接口权限，2：数据权限，3：菜单权限）
        /// </summary>
        public int PermissionType { get; set; }

        /// <summary>
        /// 权限ID
        /// </summary>
        public long PermissionId { get; set; }

        /// <summary>
        /// 权限编码
        /// </summary>
        public string PermissionCode { get; set; }

        /// <summary>
        /// 状态（0：禁用，1：启用）
        /// </summary>
        public int Status { get; set; }

        /// <summary>
        /// 是否继承上级角色的权限（0：不继承，1：继承）
        /// </summary>
        public bool InheritFromParent { get; set; }
    }
}