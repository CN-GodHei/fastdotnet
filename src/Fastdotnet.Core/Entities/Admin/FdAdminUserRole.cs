using Fastdotnet.Core.Models.Base;
using SqlSugar;

namespace Fastdotnet.Core.Entities.Admin
{
    /// <summary>
    /// 后台管理员角色关联表
    /// </summary>
    [SugarTable("FdAdminUserRole")]
    public class FdAdminUserRole : BaseEntity
    {
        /// <summary>
        /// 管理员用户ID
        /// </summary>
        public long AdminUserId { get; set; }

        /// <summary>
        /// 角色ID
        /// </summary>
        public long RoleId { get; set; }
    }
}
