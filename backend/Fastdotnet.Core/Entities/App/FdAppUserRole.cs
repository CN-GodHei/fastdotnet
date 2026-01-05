using Fastdotnet.Core.Dtos.Base;
using SqlSugar;

namespace Fastdotnet.Core.Entities.App
{
    /// <summary>
    /// 前台应用用户角色关联表
    /// </summary>
    [SugarTable("fd_app_user_role", "前台应用用户角色关联")]
    public class FdAppUserRole : BaseEntity
    {
        /// <summary>
        /// 应用用户ID
        /// </summary>
        [SugarColumn(ColumnName = "app_user_id", IsNullable = false, ColumnDescription = "应用用户ID")]
        public string AppUserId { get; set; }

        /// <summary>
        /// 角色ID
        /// </summary>
        [SugarColumn(ColumnName = "role_id", IsNullable = false, ColumnDescription = "角色ID")]
        public string RoleId { get; set; }
    }
}
