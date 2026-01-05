using Fastdotnet.Core.Dtos.Base;
using Fastdotnet.Core.Dtos.Interfaces;
using Fastdotnet.Core.Enum;
using Newtonsoft.Json.Linq;
using SqlSugar;

namespace Fastdotnet.Core.Entities.System
{
    /// <summary>
    /// 角色表
    /// </summary>
    [SugarTable("fd_role", "角色")]
    [SugarIndex("idx_role_code", nameof(Code), OrderByType.Asc, IsUnique = true)]
    [SugarIndex("idx_role_code_belong", nameof(Code), OrderByType.Asc, nameof(Belong), OrderByType.Asc, true)]
    public class FdRole : BaseEntity, ISoftDelete
    {
        /// <summary>
        /// 角色名称
        /// </summary>
        [SugarColumn(ColumnName = "name", IsNullable = false, ColumnDescription = "角色名称")]
        public string Name { get; set; }

        /// <summary>
        /// 角色编码
        /// </summary>
        [SugarColumn(ColumnName = "code", IsNullable = false, ColumnDescription = "角色编码")]
        public string Code { get; set; }

        /// <summary>
        /// 角色描述
        /// </summary>
        [SugarColumn(ColumnName = "description", IsNullable = true, ColumnDescription = "角色描述")]
        public string Description { get; set; }

        /// <summary>
        /// 角色类别: "Admin" 或 "App"
        /// </summary>
        [SugarColumn(ColumnName = "category", IsNullable = false, Length = 50, ColumnDescription = "角色类别: \"Admin\" 或 \"App\"")]
        public string Category { get; set; }

        /// <summary>
        /// 父级角色ID，用于支持角色层级
        /// </summary>
        [SugarColumn(ColumnName = "parent_id", IsNullable = true, ColumnDescription = "父级角色ID，用于支持角色层级")]
        public string? ParentId { get; set; }

        /// <summary>
        /// 是否为系统内置角色
        /// </summary>
        [SugarColumn(ColumnName = "is_system", ColumnDescription = "是否为系统内置角色")]
        public bool IsSystem { get; set; } = false;

        /// <summary>
        /// 是否为默认角色
        /// </summary>
        [SugarColumn(ColumnName = "is_default", ColumnDescription = "是否为默认角色")]
        public bool IsDefault { get; set; } = false;

        /// <summary>
        /// 属于
        /// </summary>
        [SugarColumn(ColumnName = "belong", IsNullable = false, ColumnDescription = "属于管理端还是应用端")]
        public SystemCategory Belong { get; set; }
    }
}