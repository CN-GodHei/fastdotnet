using Fastdotnet.Core.Models.Base;
using SqlSugar;

namespace Fastdotnet.Core.Entities.App
{
    /// <summary>
    /// 前台用户角色关联表
    /// </summary>
    [SugarTable("FdAppUserRole")]
    public class FdAppUserRole : BaseEntity
    {
        /// <summary>
        /// 应用用户ID
        /// </summary>
        public long AppUserId { get; set; }

        /// <summary>
        /// 角色ID
        /// </summary>
        public long RoleId { get; set; }
    }
}
