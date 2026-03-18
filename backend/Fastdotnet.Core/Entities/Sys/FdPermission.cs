namespace Fastdotnet.Core.Entities.Sys
{
    /// <summary>
    /// 权限表
    /// </summary>
    [SugarTable("fd_permission", "权限")]
    [SugarIndex("idx_perm_code", nameof(Code), OrderByType.Asc, IsUnique = true)]
    public class FdPermission : BaseEntity, ISoftDelete
    {
        /// <summary>
        /// 权限名称
        /// </summary>
        [SugarColumn(ColumnName = "name", IsNullable = false, ColumnDescription = "权限名称")]
        public string Name { get; set; }

        /// <summary>
        /// 权限代码 (e.g., "admin.user.create")
        /// </summary>
        [SugarColumn(ColumnName = "code", IsNullable = false, ColumnDescription = "权限代码 (e.g., \"admin.user.create\")")]
        public string Code { get; set; }

        /// <summary>
        /// 权限描述
        /// </summary>
        [SugarColumn(ColumnName = "description", IsNullable = true, ColumnDescription = "权限描述")]
        public string Description { get; set; }

        /// <summary>
        /// 所属模块（主框架/插件标识）
        /// </summary>
        [SugarColumn(ColumnName = "module", IsNullable = true, ColumnDescription = "所属模块（主框架/插件标识）")]
        public string Module { get; set; }

        /// <summary>
        /// 权限类型 (e.g., "Menu", "Api", "Data")
        /// </summary>
        [SugarColumn(ColumnName = "type", IsNullable = false, ColumnDescription = "权限类型 (e.g., \"Menu\", \"Api\", \"Data\")")]
        public PermissionType Type { get; set; }

        /// <summary>
        /// 权限分类: "Admin" 或 "App"
        /// </summary>
        [SugarColumn(ColumnName = "category", IsNullable = false, Length = 50, ColumnDescription = "权限分类: \"Admin\" 或 \"App\"")]
        public string Category { get; set; }
    }
}