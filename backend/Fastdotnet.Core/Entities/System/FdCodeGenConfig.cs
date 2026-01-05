using SqlSugar;
using System.ComponentModel.DataAnnotations;
using Fastdotnet.Core.Dtos.Base;

namespace Fastdotnet.Core.Entities.System
{
    /// <summary>
    /// 代码生成字段配置表
    /// </summary>
    [SugarTable("fd_code_gen_config", "代码生成字段配置")]
    public partial class FdCodeGenConfig : BaseEntity
    {
        /// <summary>
        /// 代码生成主表Id
        /// </summary>
        [SugarColumn(ColumnDescription = "主表Id")]
        public string CodeGenId { get; set; }

        /// <summary>
        /// 数据库字段名
        /// </summary>
        [SugarColumn(ColumnDescription = "字段名称", Length = 128)]
        [Required, MaxLength(128)]
        public virtual string ColumnName { get; set; }

        /// <summary>
        /// 显示字段名称
        /// </summary>
        [SugarColumn(ColumnDescription = "显示字段名称", Length = 128)]
        [MaxLength(128)]
        public virtual string ShowColumnName { get; set; }

        /// <summary>
        /// 主键
        /// </summary>
        [SugarColumn(ColumnDescription = "主键",IsNullable =true)]
        [MaxLength(8)]
        public bool ColumnKey { get; set; }

        /// <summary>
        /// 实体属性名
        /// </summary>
        [SugarColumn(ColumnDescription = "属性名称", Length = 128)]
        [Required, MaxLength(128)]
        public virtual string PropertyName { get; set; }

        /// <summary>
        /// 字段数据长度
        /// </summary>
        [SugarColumn(ColumnDescription = "字段数据长度", DefaultValue = "0")]
        public int ColumnLength { get; set; }

        /// <summary>
        /// 字段描述
        /// </summary>
        [SugarColumn(ColumnDescription = "字段描述", Length = 128)]
        [MaxLength(128)]
        public string? ColumnComment { get; set; }

        /// <summary>
        /// 数据库中类型（物理类型）
        /// </summary>
        [SugarColumn(ColumnDescription = "数据库中类型", Length = 64)]
        [MaxLength(64)]
        public string? DataType { get; set; }

        /// <summary>
        /// .NET数据类型
        /// </summary>
        [SugarColumn(ColumnDescription = "NET数据类型", Length = 64)]
        [MaxLength(64)]
        public string? NetType { get; set; }

        /// <summary>
        /// 字段数据默认值
        /// </summary>
        [SugarColumn(ColumnDescription = "默认值",IsNullable =true)]
        public string? DefaultValue { get; set; }

        /// <summary>
        /// 作用类型（字典）
        /// </summary>
        [SugarColumn(ColumnDescription = "作用类型", Length = 64)]
        [MaxLength(64)]
        public string? EffectType { get; set; }

        /// <summary>
        /// 外键库标识
        /// </summary>
        [SugarColumn(ColumnDescription = "外键库标识",IsNullable =true, Length = 20)]
        [MaxLength(20)]
        public string? FkConfigId { get; set; }

        /// <summary>
        /// 外键实体名称
        /// </summary>
        [SugarColumn(ColumnDescription = "外键实体名称", IsNullable = true, Length = 64)]
        [MaxLength(64)]
        public string? FkEntityName { get; set; }

        /// <summary>
        /// 脱敏配置
        /// 存储脱敏类型和相关参数的JSON配置
        /// </summary>
        [SugarColumn(ColumnDescription = "脱敏配置", IsNullable = true, Length = 512)]
        [MaxLength(512)]
        public string? MaskConfig { get; set; }

        /// <summary>
        /// 是否启用脱敏处理
        /// </summary>
        [SugarColumn(ColumnDescription = "是否启用脱敏处理", IsNullable = true)]
        public bool EnableMask { get; set; }

        /// <summary>
        /// 外键配置
        /// 存储外键配置的JSON配置
        /// </summary>
        [SugarColumn(ColumnDescription = "外键配置", IsNullable = true, Length = 1024)]
        [MaxLength(1024)]
        public string? ForeignKeyConfig { get; set; }

        /*
        /// <summary>
        /// 外键表名称
        /// </summary>
        [SugarColumn(ColumnDescription = "外键表名称", IsNullable = true, Length = 128)]
        [MaxLength(128)]
        public string? FkTableName { get; set; }

        /// <summary>
        /// 外键显示字段
        /// </summary>
        [SugarColumn(ColumnDescription = "外键显示字段", IsNullable = true, Length = 64)]
        [MaxLength(64)]
        public string? FkDisplayColumns { get; set; }

        /// <summary>
        /// 外键链接字段
        /// </summary>
        [SugarColumn(ColumnDescription = "外键链接字段", IsNullable = true, Length = 64)]
        [MaxLength(64)]
        public string? FkLinkColumnName { get; set; }

        /// <summary>
        /// 外键显示字段.NET类型
        /// </summary>
        [SugarColumn(ColumnDescription = "外键显示字段.NET类型", IsNullable = true, Length = 64)]
        [MaxLength(64)]
        public string? FkColumnNetType { get; set; }


        */

        /// <summary>
        /// 父级字段
        /// </summary>
        [SugarColumn(ColumnDescription = "父级字段", IsNullable = true, Length = 128)]
        [MaxLength(128)]
        public string? PidColumn { get; set; }

        /// <summary>
        /// 字典编码
        /// </summary>
        [SugarColumn(ColumnDescription = "字典编码", IsNullable = true, Length = 64)]
        [MaxLength(64)]
        public string? DictTypeCode { get; set; }

        /// <summary>
        /// 查询方式
        /// </summary>
        [SugarColumn(ColumnDescription = "查询方式", IsNullable = true, Length = 16)]
        [MaxLength(16)]
        public string? QueryType { get; set; }

        /// <summary>
        /// 是否是查询条件
        /// </summary>
        [SugarColumn(ColumnDescription = "是否是查询条件", IsNullable = true)]
        [MaxLength(8)]
        public bool WhetherQuery { get; set; }

        /// <summary>
        /// 列表是否缩进（字典）
        /// </summary>
        [SugarColumn(ColumnDescription = "列表是否缩进", IsNullable = true)]
        [MaxLength(8)]
        public bool WhetherRetract { get; set; }

        /// <summary>
        /// 是否必填（字典）
        /// </summary>
        [SugarColumn(ColumnDescription = "是否必填", IsNullable = true)]
        [MaxLength(8)]
        public bool WhetherRequired { get; set; }

        /// <summary>
        /// 是否可排序（字典）
        /// </summary>
        [SugarColumn(ColumnDescription = "是否可排序", IsNullable = true)]
        [MaxLength(8)]
        public bool WhetherSortable { get; set; }

        /// <summary>
        /// 列表显示
        /// </summary>
        [SugarColumn(ColumnDescription = "列表显示", IsNullable = true)]
        [MaxLength(8)]
        public bool WhetherTable { get; set; }

        /// <summary>
        /// 增改
        /// </summary>
        [SugarColumn(ColumnDescription = "增改", IsNullable = true)]
        [MaxLength(8)]
        public bool WhetherAddUpdate { get; set; }

        /// <summary>
        /// 导入
        /// </summary>
        [SugarColumn(ColumnDescription = "导入", IsNullable = true)]
        [MaxLength(8)]
        public bool WhetherImport { get; set; }

        /// <summary>
        /// 是否通用字段
        /// </summary>
        [SugarColumn(ColumnDescription = "是否通用字段", IsNullable = true)]
        [MaxLength(8)]
        public bool WhetherCommon { get; set; }

        /// <summary>
        /// 排序
        /// </summary>
        [SugarColumn(ColumnDescription = "排序",IsNullable =true)]
        public int OrderNo { get; set; } = 100;
    }
}