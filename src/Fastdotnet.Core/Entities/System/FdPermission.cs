using Fastdotnet.Core.Models.Base;
using Fastdotnet.Core.Models.Interfaces;
using SqlSugar;
using Fastdotnet.Core;

namespace Fastdotnet.Core.Entities.System
{
    /// <summary>
    /// 权限表
    /// </summary>
    [SugarTable("FdPermission")]
    [SugarIndex("idx_perm_code", nameof(Code), OrderByType.Asc, IsUnique = true)]
    public class FdPermission : BaseEntity, ISoftDelete
    {
        /// <summary>
        /// 权限名称
        /// </summary>
        [SugarColumn(IsNullable = false)]
        public string Name { get; set; }

        /// <summary>
        /// 权限代码 (e.g., "admin.user.create")
        /// </summary>
        [SugarColumn(IsNullable = false)]
        public string Code { get; set; }

        /// <summary>
        /// 权限描述
        /// </summary>
        [SugarColumn(IsNullable = true)]
        public string Description { get; set; }

        /// <summary>
        /// 所属模块（主框架/插件标识）
        /// </summary>
        [SugarColumn(IsNullable = true)]
        public string Module { get; set; }

        /// <summary>
        /// 权限类型 (e.g., "Menu", "Api", "Data")
        /// </summary>
        [SugarColumn(IsNullable = false, Length = 50)]
        public PermissionType Type { get; set; }

        /// <summary>
        /// 权限分类: "Admin" 或 "App"
        /// </summary>
        [SugarColumn(IsNullable = false, Length = 50)]
        public string Category { get; set; }
    }
}