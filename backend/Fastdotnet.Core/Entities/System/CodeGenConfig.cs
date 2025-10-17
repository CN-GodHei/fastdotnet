using SqlSugar;
using Fastdotnet.Core.Models.Base;

namespace Fastdotnet.Core.Entities.System
{
    /// <summary>
    /// 代码生成配置表
    /// </summary>
    [SugarTable("sys_code_gen_config", "代码生成配置")]
    public class CodeGenConfig : BaseEntity
    {
        /// <summary>
        /// 表名
        /// </summary>
        [SugarColumn(Length = 100, IsNullable = false, ColumnDescription = "表名")]
        public string? TableName { get; set; }

        /// <summary>
        /// 实体名
        /// </summary>
        [SugarColumn(Length = 100, IsNullable = true, ColumnDescription = "实体名")]
        public string? EntityName { get; set; }


        /// <summary>
        /// 命名空间
        /// </summary>
        [SugarColumn(Length = 200, IsNullable = true, ColumnDescription = "命名空间")]
        public string? NameSpace { get; set; }


        /// <summary>
        /// 生成方式
        /// </summary>
        [SugarColumn(Length = 50, IsNullable = true, ColumnDescription = "生成方式")]
        public string? GenerateType { get; set; }

        /// <summary>
        /// 生成菜单
        /// </summary>
        [SugarColumn(ColumnDescription = "生成菜单", IsNullable = true)]
        public bool GenerateMenu { get; set; } = false;

        /// <summary>
        /// 菜单图标
        /// </summary>
        [SugarColumn(Length = 100, IsNullable = true, ColumnDescription = "菜单图标")]
        public string? MenuIcon { get; set; }

        /// <summary>
        /// 菜单父级ID
        /// </summary>
        [SugarColumn(Length = 32, IsNullable = true, ColumnDescription = "菜单父级ID")]
        public string? MenuPid { get; set; }

        /// <summary>
        /// 前端页面路径
        /// </summary>
        [SugarColumn(Length = 200, IsNullable = true, ColumnDescription = "前端页面路径")]
        public string? PagePath { get; set; }

        /// <summary>
        /// 支持打印
        /// </summary>
        [SugarColumn(Length = 50, IsNullable = true, ColumnDescription = "支持打印")]
        public string? PrintType { get; set; }

        /// <summary>
        /// 打印模板
        /// </summary>
        [SugarColumn(Length = 100, IsNullable = true, ColumnDescription = "打印模板")]
        public string? PrintName { get; set; }

        /// <summary>
        /// 数据唯一性配置
        /// </summary>
        [SugarColumn(IsNullable = true, ColumnDescription = "数据唯一性配置")]
        public List<TableUniqueConfig>? TableUniqueList { get; set; }
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