using Fastdotnet.Core.Models.Base;
using Fastdotnet.Core.Models.Interfaces;
using SqlSugar;

namespace Fastdotnet.Core.Entities.System
{
    /// <summary>
    /// 角色表
    /// </summary>
    [SugarTable("FdRole")]
    [SugarIndex("idx_role_code", nameof(Code), OrderByType.Asc, IsUnique = true)]
    public class FdRole : BaseEntity, ISoftDelete
    {
        /// <summary>
        /// 角色名称
        /// </summary>
        [SugarColumn(IsNullable = false)]
        public string Name { get; set; }

        /// <summary>
        /// 角色编码
        /// </summary>
        [SugarColumn(IsNullable = false)]
        public string Code { get; set; }

        /// <summary>
        /// 角色描述
        /// </summary>
        [SugarColumn(IsNullable = true)]
        public string Description { get; set; }

        /// <summary>
        /// 角色类别: "Admin" 或 "App"
        /// </summary>
        [SugarColumn(IsNullable = false, Length = 50)]
        public string Category { get; set; }

        /// <summary>
        /// 父级角色ID，用于支持角色层级
        /// </summary>
        [SugarColumn(IsNullable = true)]
        public long? ParentId { get; set; }

        /// <summary>
        /// 是否为系统内置角色
        /// </summary>
        public bool IsSystem { get; set; } = false;
    }
}