
namespace Fastdotnet.Core.Entities.Admin
{
    /// <summary>
    /// 后台管理员角色关联表
    /// </summary>
    [SugarTable("fd_admin_user_role")]
    public class FdAdminUserRole : BaseEntity
    {
        /// <summary>
        /// 管理员用户ID
        /// </summary>
        [SugarColumn(ColumnName = "admin_user_id", IsNullable = false, ColumnDescription = "管理员用户ID")]
        public string AdminUserId { get; set; }

        /// <summary>
        /// 角色ID
        /// </summary>
        [SugarColumn(ColumnName = "role_id", IsNullable = false, ColumnDescription = "角色ID")]
        public string RoleId { get; set; }
    }
}
