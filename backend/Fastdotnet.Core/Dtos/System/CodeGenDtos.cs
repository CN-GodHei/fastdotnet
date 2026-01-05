using System.ComponentModel.DataAnnotations;

namespace Fastdotnet.Core.Dtos.System
{
    /// <summary>
    /// 代码生成配置 DTO
    /// </summary>
    public class CodeGenConfigDto
    {
        public string? Id { get; set; }


        /// <summary>
        /// 表名
        /// </summary>
        public string? TableName { get; set; }

        /// <summary>
        /// 表注释
        /// </summary>
        public string? TableComment { get; set; }

        /// <summary>
        /// 实体名
        /// </summary>
        public string? EntityName { get; set; }


        /// <summary>
        /// 命名空间
        /// </summary>
        public string? NameSpace { get; set; }

        /// <summary>
        /// 生成方式
        /// </summary>
        public string? GenerateType { get; set; }

        /// <summary>
        /// 生成菜单
        /// </summary>
        public bool GenerateMenu { get; set; }

        /// <summary>
        /// 菜单图标
        /// </summary>
        public string? MenuIcon { get; set; }

        /// <summary>
        /// 菜单父级ID
        /// </summary>
        public string? MenuPid { get; set; }

        /// <summary>
        /// 前端页面路径
        /// </summary>
        public string? PagePath { get; set; }

        /// <summary>
        /// 支持打印
        /// </summary>
        public string? PrintType { get; set; }

        /// <summary>
        /// 打印模板
        /// </summary>
        public string? PrintName { get; set; }

        /// <summary>
        /// 数据唯一性配置
        /// </summary>
        public List<TableUniqueConfigDto>? TableUniqueList { get; set; }
    }

    /// <summary>
    /// 创建代码生成配置 DTO
    /// </summary>
    public class CreateCodeGenDto
    {

        /// <summary>
        /// 表名
        /// </summary>
        [Required(ErrorMessage = "表名不能为空")]
        [StringLength(100, ErrorMessage = "表名长度不能超过100个字符")]
        public string? TableName { get; set; }

        /// <summary>
        /// 表注释
        /// </summary>
        public string? TableComment { get; set; }

        /// <summary>
        /// 命名空间
        /// </summary>
        [Required(ErrorMessage = "命名空间不能为空")]
        [StringLength(200, ErrorMessage = "命名空间长度不能超过200个字符")]
        public string? NameSpace { get; set; }


        /// <summary>
        /// 生成方式
        /// </summary>
        [Required(ErrorMessage = "生成方式不能为空")]
        [StringLength(50, ErrorMessage = "生成方式长度不能超过50个字符")]
        public string? GenerateType { get; set; }

        /// <summary>
        /// 生成菜单
        /// </summary>
        public bool GenerateMenu { get; set; } = false;

        /// <summary>
        /// 菜单图标
        /// </summary>
        public string? MenuIcon { get; set; }

        /// <summary>
        /// 菜单父级ID
        /// </summary>
        public string? MenuPid { get; set; }

        /// <summary>
        /// 前端页面路径
        /// </summary>
        public string? PagePath { get; set; }

        /// <summary>
        /// 支持打印
        /// </summary>
        public string? PrintType { get; set; }

        /// <summary>
        /// 打印模板
        /// </summary>
        public string? PrintName { get; set; }

        /// <summary>
        /// 数据唯一性配置
        /// </summary>
        public List<TableUniqueConfigDto>? TableUniqueList { get; set; }
    }

    /// <summary>
    /// 更新代码生成配置 DTO
    /// </summary>
    public class UpdateCodeGenDto
    {
       

        /// <summary>
        /// 表名
        /// </summary>
        [Required(ErrorMessage = "表名不能为空")]
        [StringLength(100, ErrorMessage = "表名长度不能超过100个字符")]
        public string? TableName { get; set; }

        /// <summary>
        /// 表注释
        /// </summary>
        public string? TableComment { get; set; }

        /// <summary>
        /// 实体名
        /// </summary>
        [Required(ErrorMessage = "实体名不能为空")]
        [StringLength(100, ErrorMessage = "实体名长度不能超过100个字符")]
        public string? EntityName { get; set; }

        ///// <summary>
        ///// 业务名
        ///// </summary>
        //[Required(ErrorMessage = "业务名不能为空")]
        //[StringLength(100, ErrorMessage = "业务名长度不能超过100个字符")]
        //public string? BusName { get; set; }

        /// <summary>
        /// 命名空间
        /// </summary>
        [Required(ErrorMessage = "命名空间不能为空")]
        [StringLength(200, ErrorMessage = "命名空间长度不能超过200个字符")]
        public string? NameSpace { get; set; }


        /// <summary>
        /// 生成方式
        /// </summary>
        [Required(ErrorMessage = "生成方式不能为空")]
        [StringLength(50, ErrorMessage = "生成方式长度不能超过50个字符")]
        public string? GenerateType { get; set; }

        /// <summary>
        /// 生成菜单
        /// </summary>
        public bool GenerateMenu { get; set; }

        /// <summary>
        /// 菜单图标
        /// </summary>
        public string? MenuIcon { get; set; }

        /// <summary>
        /// 菜单父级ID
        /// </summary>
        public string? MenuPid { get; set; }

        /// <summary>
        /// 前端页面路径
        /// </summary>
        public string? PagePath { get; set; }

        /// <summary>
        /// 支持打印
        /// </summary>
        public string? PrintType { get; set; }

        /// <summary>
        /// 打印模板
        /// </summary>
        public string? PrintName { get; set; }

        /// <summary>
        /// 数据唯一性配置
        /// </summary>
        public List<TableUniqueConfigDto>? TableUniqueList { get; set; }
    }

    /// <summary>
    /// 表唯一性配置 DTO
    /// </summary>
    public class TableUniqueConfigDto
    {
        /// <summary>
        /// 字段列表
        /// </summary>
        [Required(ErrorMessage = "字段列表不能为空")]
        [MinLength(1, ErrorMessage = "字段列表至少包含一个字段")]
        public List<string>? Columns { get; set; }

        /// <summary>
        /// 描述信息
        /// </summary>
        [StringLength(200, ErrorMessage = "描述信息长度不能超过200个字符")]
        public string? Message { get; set; }
    }

    /// <summary>
    /// 数据库表信息 DTO
    /// </summary>
    public class TableInfoDto
    {
        /// <summary>
        /// 表名
        /// </summary>
        public string? TableName { get; set; }

        /// <summary>
        /// 实体名
        /// </summary>
        public string? EntityName { get; set; }

        /// <summary>
        /// 表注释
        /// </summary>
        public string? TableComment { get; set; }
    }

    /// <summary>
    /// 数据库列信息 DTO
    /// </summary>
    public class ColumnInfoDto
    {
        /// <summary>
        /// 字段名
        /// </summary>
        public string? ColumnName { get; set; }

        /// <summary>
        /// 属性名
        /// </summary>
        public string? PropertyName { get; set; }

        /// <summary>
        /// 数据类型
        /// </summary>
        public string? DataType { get; set; }

        /// <summary>
        /// .NET类型
        /// </summary>
        public string? NetType { get; set; }

        /// <summary>
        /// 是否主键
        /// </summary>
        public bool IsPrimarykey { get; set; }

        /// <summary>
        /// 是否自增
        /// </summary>
        public bool IsIdentity { get; set; }

        /// <summary>
        /// 是否可空
        /// </summary>
        public bool IsNullable { get; set; }

        /// <summary>
        /// 列长度
        /// </summary>
        public int Length { get; set; }

        /// <summary>
        /// 列精度
        /// </summary>
        public int? Scale { get; set; }

        /// <summary>
        /// 默认值
        /// </summary>
        public string? DefaultValue { get; set; }

        /// <summary>
        /// 列注释
        /// </summary>
        public string? ColumnComment { get; set; }

        /// <summary>
        /// 是否忽略
        /// </summary>
        public bool IsIgnore { get; set; }

        public virtual string ShowColumnName { get; set; }

    }

    /// <summary>
    /// 代码生成输入 DTO
    /// </summary>
    public class CodeGenInput
    {
        /// <summary>
        /// 配置ID
        /// </summary>
        [Required(ErrorMessage = "配置ID不能为空")]
        [StringLength(100, ErrorMessage = "配置ID长度不能超过100个字符")]
        public string? ConfigId { get; set; }

        /// <summary>
        /// 表名
        /// </summary>
        [Required(ErrorMessage = "表名不能为空")]
        [StringLength(100, ErrorMessage = "表名长度不能超过100个字符")]
        public string? TableName { get; set; }

        /// <summary>
        /// 命名空间
        /// </summary>
        [Required(ErrorMessage = "命名空间不能为空")]
        [StringLength(200, ErrorMessage = "命名空间长度不能超过200个字符")]
        public string? NameSpace { get; set; }

        /// <summary>
        /// 生成方式
        /// </summary>
        [Required(ErrorMessage = "生成方式不能为空")]
        [StringLength(50, ErrorMessage = "生成方式长度不能超过50个字符")]
        public string? GenerateType { get; set; }

        /// <summary>
        /// 生成菜单
        /// </summary>
        public bool GenerateMenu { get; set; }

        /// <summary>
        /// 菜单图标
        /// </summary>
        [StringLength(100, ErrorMessage = "菜单图标长度不能超过100个字符")]
        public string? MenuIcon { get; set; }

        /// <summary>
        /// 菜单父级ID
        /// </summary>
        [StringLength(100, ErrorMessage = "菜单父级ID长度不能超过100个字符")]
        public string? MenuPid { get; set; }

        /// <summary>
        /// 前端页面路径
        /// </summary>
        [StringLength(200, ErrorMessage = "前端页面路径长度不能超过200个字符")]
        public string? PagePath { get; set; }
        
        /// <summary>
        /// 预览操作类型（filelist获取文件列表，其他为获取文件内容）
        /// </summary>
        public string? Action { get; set; }
        
        /// <summary>
        /// 选中的预览文件路径
        /// </summary>
        public string? SelectedFile { get; set; }
    }
    
    /// <summary>
    /// 代码生成预览结果 DTO
    /// </summary>
    public class CodeGenPreviewResult
    {
        /// <summary>
        /// 文件内容
        /// </summary>
        public string? Content { get; set; }
        
        /// <summary>
        /// 文件列表
        /// </summary>
        public List<PreviewFileItem>? FileList { get; set; }
    }
    
    /// <summary>
    /// 预览文件项 DTO
    /// </summary>
    public class PreviewFileItem
    {
        /// <summary>
        /// 文件名
        /// </summary>
        public string? Name { get; set; }
        
        /// <summary>
        /// 文件路径
        /// </summary>
        public string? Path { get; set; }
        
        /// <summary>
        /// 文件类型
        /// </summary>
        public string? Type { get; set; }
    }
}