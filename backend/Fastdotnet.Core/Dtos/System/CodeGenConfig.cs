
namespace Fastdotnet.Core.Dtos.System
{

    /// <summary>
    /// 代码生成字段配置 DTO
    /// </summary>
    public class FdCodeGenConfigDto
    {
        public string? Id { get; set; }

        /// <summary>
        /// 代码生成主表Id
        /// </summary>
        public string CodeGenId { get; set; }

        /// <summary>
        /// 数据库字段名
        /// </summary>
        public string? ColumnName { get; set; }

        /// <summary>
        /// 主键
        /// </summary>
        public bool ColumnKey { get; set; }

        /// <summary>
        /// 实体属性名
        /// </summary>
        public string? PropertyName { get; set; }

        /// <summary>
        /// 字段数据长度
        /// </summary>
        public int ColumnLength { get; set; }

        /// <summary>
        /// 字段描述
        /// </summary>
        public string? ColumnComment { get; set; }

        /// <summary>
        /// 数据库中类型（物理类型）
        /// </summary>
        public string? DataType { get; set; }

        /// <summary>
        /// .NET数据类型
        /// </summary>
        public string? NetType { get; set; }

        /// <summary>
        /// 字段数据默认值
        /// </summary>
        public string? DefaultValue { get; set; }

        /// <summary>
        /// 作用类型（字典）
        /// </summary>
        public string? EffectType { get; set; }

        /// <summary>
        /// 外键库标识
        /// </summary>
        public string? FkConfigId { get; set; }

        /// <summary>
        /// 外键实体名称
        /// </summary>
        public string? FkEntityName { get; set; }

        /// <summary>
        /// 外键表名称
        /// </summary>
        // public string? FkTableName { get; set; }

        /// <summary>
        /// 外键显示字段
        /// </summary>
        // public string? FkDisplayColumns { get; set; }

        /// <summary>
        /// 外键链接字段
        /// </summary>
        // public string? FkLinkColumnName { get; set; }

        /// <summary>
        /// 外键显示字段.NET类型
        /// </summary>
        // public string? FkColumnNetType { get; set; }

        /// <summary>
        /// 父级字段
        /// </summary>
        // public string? PidColumn { get; set; }

        /// <summary>
        /// 字典编码
        /// </summary>
        public string? DictTypeCode { get; set; }

        /// <summary>
        /// 查询方式
        /// </summary>
        public string? QueryType { get; set; }

        /// <summary>
        /// 是否是查询条件
        /// </summary>
        public bool WhetherQuery { get; set; }

        /// <summary>
        /// 列表是否缩进（字典）
        /// </summary>
        public bool WhetherRetract { get; set; }

        /// <summary>
        /// 是否必填（字典）
        /// </summary>
        public bool WhetherRequired { get; set; }

        /// <summary>
        /// 是否可排序（字典）
        /// </summary>
        public bool WhetherSortable { get; set; }

        /// <summary>
        /// 列表显示
        /// </summary>
        public bool WhetherTable { get; set; }

        /// <summary>
        /// 增改
        /// </summary>
        public bool WhetherAddUpdate { get; set; }

        /// <summary>
        /// 新增
        /// </summary>
        public bool WhetherAdd { get; set; }

        /// <summary>
        /// 修改
        /// </summary>
        public bool WhetherUpdate { get; set; }

        /// <summary>
        /// 导入
        /// </summary>
        public bool WhetherImport { get; set; }

        /// <summary>
        /// 是否通用字段
        /// </summary>
        public bool WhetherCommon { get; set; }

        /// <summary>
        /// 排序
        /// </summary>
        public int OrderNo { get; set; } = 100;

        public virtual string ShowColumnName { get; set; }
        
        /// <summary>
        /// 脱敏配置
        /// </summary>
        public MaskConfigModel? MaskConfig { get; set; }

        /// <summary>
        /// 是否启用脱敏处理
        /// </summary>
        public bool EnableMask { get; set; }
        
        /// <summary>
        /// 外键配置
        /// </summary>
        public ForeignKeyConfigModel? ForeignKeyConfig { get; set; }
    }

    /// <summary>
    /// 创建代码生成字段配置 DTO
    /// </summary>
    public class CreateFdCodeGenConfigDto
    {
        /// <summary>
        /// 代码生成主表Id
        /// </summary>
        [Required(ErrorMessage = "代码生成主表Id不能为空")]
        public string CodeGenId { get; set; }

        /// <summary>
        /// 数据库字段名
        /// </summary>
        [Required(ErrorMessage = "字段名称不能为空")]
        [StringLength(128, ErrorMessage = "字段名称长度不能超过128个字符")]
        public string? ColumnName { get; set; }

        /// <summary>
        /// 主键
        /// </summary>
        public bool ColumnKey { get; set; }

        /// <summary>
        /// 实体属性名
        /// </summary>
        [Required(ErrorMessage = "属性名称不能为空")]
        [StringLength(128, ErrorMessage = "属性名称长度不能超过128个字符")]
        public string? PropertyName { get; set; }

        /// <summary>
        /// 字段数据长度
        /// </summary>
        public int ColumnLength { get; set; }

        /// <summary>
        /// 字段描述
        /// </summary>
        [StringLength(128, ErrorMessage = "字段描述长度不能超过128个字符")]
        public string? ColumnComment { get; set; }

        /// <summary>
        /// 数据库中类型（物理类型）
        /// </summary>
        [StringLength(64, ErrorMessage = "数据库类型长度不能超过64个字符")]
        public string? DataType { get; set; }

        /// <summary>
        /// .NET数据类型
        /// </summary>
        [StringLength(64, ErrorMessage = "NET数据类型长度不能超过64个字符")]
        public string? NetType { get; set; }

        /// <summary>
        /// 字段数据默认值
        /// </summary>
        public string? DefaultValue { get; set; }

        /// <summary>
        /// 作用类型（字典）
        /// </summary>
        [StringLength(64, ErrorMessage = "作用类型长度不能超过64个字符")]
        public string? EffectType { get; set; }

        ///// <summary>
        ///// 外键库标识
        ///// </summary>
        //[StringLength(20, ErrorMessage = "外键库标识长度不能超过20个字符")]
        //public string? FkConfigId { get; set; }

        ///// <summary>
        ///// 外键实体名称
        ///// </summary>
        //[StringLength(64, ErrorMessage = "外键实体名称长度不能超过64个字符")]
        //public string? FkEntityName { get; set; }

        ///// <summary>
        ///// 外键表名称
        ///// </summary>
        //[StringLength(128, ErrorMessage = "外键表名称长度不能超过128个字符")]
        //public string? FkTableName { get; set; }

        ///// <summary>
        ///// 外键显示字段
        ///// </summary>
        //[StringLength(64, ErrorMessage = "外键显示字段长度不能超过64个字符")]
        //public string? FkDisplayColumns { get; set; }

        ///// <summary>
        ///// 外键链接字段
        ///// </summary>
        //[StringLength(64, ErrorMessage = "外键链接字段长度不能超过64个字符")]
        //public string? FkLinkColumnName { get; set; }

        ///// <summary>
        ///// 外键显示字段.NET类型
        ///// </summary>
        //[StringLength(64, ErrorMessage = "外键显示字段.NET类型长度不能超过64个字符")]
        //public string? FkColumnNetType { get; set; }

        /// <summary>
        /// 父级字段
        /// </summary>
        [StringLength(128, ErrorMessage = "父级字段长度不能超过128个字符")]
        public string? PidColumn { get; set; }

        /// <summary>
        /// 外键配置
        /// </summary>
        public ForeignKeyConfigModel? ForeignKeyConfig { get; set; }
        /// <summary>
        /// 字典编码
        /// </summary>
        [StringLength(64, ErrorMessage = "字典编码长度不能超过64个字符")]
        public string? DictTypeCode { get; set; }

        /// <summary>
        /// 查询方式
        /// </summary>
        [StringLength(16, ErrorMessage = "查询方式长度不能超过16个字符")]
        public string? QueryType { get; set; }

        /// <summary>
        /// 是否是查询条件
        /// </summary>
        public bool WhetherQuery { get; set; }

        /// <summary>
        /// 列表是否缩进（字典）
        /// </summary>
        public bool WhetherRetract { get; set; }

        /// <summary>
        /// 是否必填（字典）
        /// </summary>
        public bool WhetherRequired { get; set; }

        /// <summary>
        /// 是否可排序（字典）
        /// </summary>
        public bool WhetherSortable { get; set; }

        /// <summary>
        /// 列表显示
        /// </summary>
        public bool WhetherTable { get; set; }


        /// <summary>
        /// 新增
        /// </summary>
        public bool WhetherAdd { get; set; }

        /// <summary>
        /// 修改
        /// </summary>
        public bool WhetherUpdate { get; set; }

        /// <summary>
        /// 导入
        /// </summary>
        public bool WhetherImport { get; set; }

        /// <summary>
        /// 是否通用字段
        /// </summary>
        public bool WhetherCommon { get; set; }

        /// <summary>
        /// 排序
        /// </summary>
        public int OrderNo { get; set; } = 100;

        public virtual string ShowColumnName { get; set; }


        /// <summary>
        /// 脱敏配置
        /// </summary>
        public MaskConfigModel? MaskConfig { get; set; }

        /// <summary>
        /// 是否启用脱敏处理
        /// </summary>
        public bool EnableMask { get; set; }

    }

    /// <summary>
    /// 更新代码生成字段配置 DTO
    /// </summary>
    public class UpdateFdCodeGenConfigDto
    {
        /// <summary>
        /// 主键Id不能为空
        /// </summary>
        [Required(ErrorMessage = "主键Id不能为空")]
        public string Id { get; set; }

        /// <summary>
        /// 代码生成主表Id
        /// </summary>
        [Required(ErrorMessage = "代码生成主表Id不能为空")]
        public string CodeGenId { get; set; }

        /// <summary>
        /// 数据库字段名
        /// </summary>
        [Required(ErrorMessage = "字段名称不能为空")]
        [StringLength(128, ErrorMessage = "字段名称长度不能超过128个字符")]
        public string? ColumnName { get; set; }

        /// <summary>
        /// 主键
        /// </summary>
        public bool ColumnKey { get; set; }

        /// <summary>
        /// 实体属性名
        /// </summary>
        [Required(ErrorMessage = "属性名称不能为空")]
        [StringLength(128, ErrorMessage = "属性名称长度不能超过128个字符")]
        public string? PropertyName { get; set; }

        /// <summary>
        /// 字段数据长度
        /// </summary>
        public int ColumnLength { get; set; }

        /// <summary>
        /// 字段描述
        /// </summary>
        [StringLength(128, ErrorMessage = "字段描述长度不能超过128个字符")]
        public string? ColumnComment { get; set; }

        /// <summary>
        /// 数据库中类型（物理类型）
        /// </summary>
        [StringLength(64, ErrorMessage = "数据库类型长度不能超过64个字符")]
        public string? DataType { get; set; }

        /// <summary>
        /// .NET数据类型
        /// </summary>
        [StringLength(64, ErrorMessage = "NET数据类型长度不能超过64个字符")]
        public string? NetType { get; set; }

        /// <summary>
        /// 字段数据默认值
        /// </summary>
        public string? DefaultValue { get; set; }

        /// <summary>
        /// 作用类型（字典）
        /// </summary>
        [StringLength(64, ErrorMessage = "作用类型长度不能超过64个字符")]
        public string? EffectType { get; set; }

        /// <summary>
        /// 外键库标识
        /// </summary>
        //[StringLength(20, ErrorMessage = "外键库标识长度不能超过20个字符")]
        //public string? FkConfigId { get; set; }

        ///// <summary>
        ///// 外键实体名称
        ///// </summary>
        //[StringLength(64, ErrorMessage = "外键实体名称长度不能超过64个字符")]
        //public string? FkEntityName { get; set; }

        ///// <summary>
        ///// 外键表名称
        ///// </summary>
        //[StringLength(128, ErrorMessage = "外键表名称长度不能超过128个字符")]
        //public string? FkTableName { get; set; }

        ///// <summary>
        ///// 外键显示字段
        ///// </summary>
        //[StringLength(64, ErrorMessage = "外键显示字段长度不能超过64个字符")]
        //public string? FkDisplayColumns { get; set; }

        ///// <summary>
        ///// 外键链接字段
        ///// </summary>
        //[StringLength(64, ErrorMessage = "外键链接字段长度不能超过64个字符")]
        //public string? FkLinkColumnName { get; set; }

        ///// <summary>
        ///// 外键显示字段.NET类型
        ///// </summary>
        //[StringLength(64, ErrorMessage = "外键显示字段.NET类型长度不能超过64个字符")]
        //public string? FkColumnNetType { get; set; }

        /// <summary>
        /// 父级字段
        /// </summary>
        [StringLength(128, ErrorMessage = "父级字段长度不能超过128个字符")]
        public string? PidColumn { get; set; }

        /// <summary>
        /// 外键配置
        /// </summary>
        public ForeignKeyConfigModel? ForeignKeyConfig { get; set; }

        /// <summary>
        /// 字典编码
        /// </summary>
        [StringLength(64, ErrorMessage = "字典编码长度不能超过64个字符")]
        public string? DictTypeCode { get; set; }

        /// <summary>
        /// 查询方式
        /// </summary>
        [StringLength(16, ErrorMessage = "查询方式长度不能超过16个字符")]
        public string? QueryType { get; set; }

        /// <summary>
        /// 是否是查询条件
        /// </summary>
        public bool WhetherQuery { get; set; }

        /// <summary>
        /// 列表是否缩进（字典）
        /// </summary>
        public bool WhetherRetract { get; set; }

        /// <summary>
        /// 是否必填（字典）
        /// </summary>
        public bool WhetherRequired { get; set; }

        /// <summary>
        /// 是否可排序（字典）
        /// </summary>
        public bool WhetherSortable { get; set; }

        /// <summary>
        /// 列表显示
        /// </summary>
        public bool WhetherTable { get; set; }

        /// <summary>
        /// 新增
        /// </summary>
        public bool WhetherAdd { get; set; }

        /// <summary>
        /// 修改
        /// </summary>
        public bool WhetherUpdate { get; set; }

        /// <summary>
        /// 导入
        /// </summary>
        public bool WhetherImport { get; set; }

        /// <summary>
        /// 是否通用字段
        /// </summary>
        public bool WhetherCommon { get; set; }

        /// <summary>
        /// 排序
        /// </summary>
        public int OrderNo { get; set; } = 100;

        public virtual string ShowColumnName { get; set; }

        /// <summary>
        /// 脱敏配置
        /// </summary>
        public MaskConfigModel? MaskConfig { get; set; }

        /// <summary>
        /// 是否启用脱敏处理
        /// </summary>
        public bool EnableMask { get; set; }
    }

    /// <summary>
    /// 脱敏配置模型
    /// </summary>
    public class MaskConfigModel
    {
        /// <summary>
        /// 脱敏类型
        /// </summary>
        public string? Type { get; set; }

        /// <summary>
        /// 保留前缀字符数
        /// </summary>
        public int? PrefixKeep { get; set; }

        /// <summary>
        /// 保留后缀字符数
        /// </summary>
        public int? SuffixKeep { get; set; }

        /// <summary>
        /// 脱敏字符
        /// </summary>
        public char? MaskChar { get; set; }

        /// <summary>
        /// 脱敏字符长度
        /// </summary>
        public int? MaskLength { get; set; }

        /// <summary>
        /// 自定义正则表达式
        /// </summary>
        public string? CustomPattern { get; set; }

        /// <summary>
        /// 自定义替换字符串
        /// </summary>
        public string? CustomReplacement { get; set; }
    }

    /// <summary>
    /// 外键配置模型
    /// </summary>
    public class ForeignKeyConfigModel
    {
        /// <summary>
        /// 外键库标识
        /// </summary>
        public string? FkConfigId { get; set; }

        /// <summary>
        /// 外键实体名称
        /// </summary>
        public string? FkEntityName { get; set; }

        /// <summary>
        /// 外键表名称
        /// </summary>
        public string? FkTableName { get; set; }

        /// <summary>
        /// 外键显示字段列表
        /// </summary>
        public List<string>? FkDisplayColumnList { get; set; }

        /// <summary>
        /// 外键链接字段
        /// </summary>
        public string? FkLinkColumnName { get; set; }

        /// <summary>
        /// 外键显示字段.NET类型
        /// </summary>
        public string? FkColumnNetType { get; set; }

        /// <summary>
        /// 代码生成主表Id
        /// </summary>
        public string? CodeGenId { get; set; }
    }
}
