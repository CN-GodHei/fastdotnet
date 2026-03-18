namespace Fastdotnet.Core.Entities.Sys
{
    /// <summary>
    /// 代码生成配置表
    /// </summary>
    [SugarTable("fd_code_gen", "代码生成配置")]
    public class FdCodeGen : BaseEntity
    {
        /// <summary>
        /// 表名
        /// </summary>
        [SugarColumn(ColumnName = "table_name", Length = 100, IsNullable = false, ColumnDescription = "表名")]
        public string? TableName { get; set; }

        /// <summary>
        /// 表注释
        /// </summary>
        [SugarColumn(ColumnName = "table_comment", Length = 100, IsNullable = true, ColumnDescription = "表注释")]
        public string? TableComment { get; set; }

        /// <summary>
        /// 实体名
        /// </summary>
        [SugarColumn(ColumnName = "entity_name", Length = 100, IsNullable = true, ColumnDescription = "实体名")]
        public string? EntityName { get; set; }


        /// <summary>
        /// 命名空间
        /// </summary>
        [SugarColumn(ColumnName = "name_space", Length = 200, IsNullable = true, ColumnDescription = "命名空间")]
        public string? NameSpace { get; set; }


        /// <summary>
        /// 生成方式
        /// </summary>
        [SugarColumn(ColumnName = "generate_type", Length = 50, IsNullable = true, ColumnDescription = "生成方式")]
        public string? GenerateType { get; set; }

        /// <summary>
        /// 生成菜单
        /// </summary>
        [SugarColumn(ColumnName = "generate_menu", ColumnDescription = "生成菜单", IsNullable = true)]
        public bool GenerateMenu { get; set; } = false;

        /// <summary>
        /// 菜单图标
        /// </summary>
        [SugarColumn(ColumnName = "menu_icon", Length = 100, IsNullable = true, ColumnDescription = "菜单图标")]
        public string? MenuIcon { get; set; }

        /// <summary>
        /// 菜单父级ID
        /// </summary>
        [SugarColumn(ColumnName = "menu_pid", Length = 32, IsNullable = true, ColumnDescription = "菜单父级ID")]
        public string? MenuPid { get; set; }

        /// <summary>
        /// 前端页面路径
        /// </summary>
        [SugarColumn(ColumnName = "page_path", Length = 200, IsNullable = true, ColumnDescription = "前端页面路径")]
        public string? PagePath { get; set; }

        /// <summary>
        /// 支持打印
        /// </summary>
        [SugarColumn(ColumnName = "print_type", Length = 50, IsNullable = true, ColumnDescription = "支持打印")]
        public string? PrintType { get; set; }

        /// <summary>
        /// 打印模板
        /// </summary>
        [SugarColumn(ColumnName = "print_name", Length = 100, IsNullable = true, ColumnDescription = "打印模板")]
        public string? PrintName { get; set; }


        [SugarColumn(ColumnDescription = "表唯一字段配置", Length = 512, IsNullable = true)]
        [MaxLength(128)]
        public string? TableUniqueConfig { get; set; }

        /// <summary>
        /// 表唯一字段列表
        /// </summary>
        //[SugarColumn(IsIgnore = true)]
        //public virtual List<TableUniqueConfig> TableUniqueList => string.IsNullOrWhiteSpace(TableUniqueConfig) ? null : JsonConvert.DeserializeObject<List<TableUniqueConfig>>(TableUniqueConfig);

    }

    /// <summary>
    /// 表唯一性配置
    /// </summary>
    public class TableUniqueConfig
    {
        /// <summary>
        /// 字段列表
        /// </summary>
        public List<string>? Columns { get; set; }

        /// <summary>
        /// 描述信息
        /// </summary>
        public string? Message { get; set; }
    }
}