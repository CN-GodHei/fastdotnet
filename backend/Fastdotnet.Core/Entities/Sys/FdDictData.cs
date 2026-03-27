namespace Fastdotnet.Core.Entities.Sys
{
    /// <summary>
    /// 值的数据类型枚举（用于字典数据的 ValueType 字段）
    /// </summary>
    public enum DictValueType
    {
        /// <summary>
        /// 字符串类型（默认）
        /// </summary>
        String = 0,

        /// <summary>
        /// 整数类型（Int32）
        /// </summary>
        Int = 1,

        /// <summary>
        /// 长整型（Int64）
        /// </summary>
        Long = 2,

        /// <summary>
        /// 浮点型（Double）
        /// </summary>
        Double = 3,

        /// <summary>
        /// 金额类型（Decimal，精确计算）
        /// </summary>
        Decimal = 4,

        /// <summary>
        /// 布尔类型（true/false）
        /// </summary>
        Boolean = 5,

        /// <summary>
        /// 日期时间类型（DateTime）
        /// </summary>
        DateTime = 6,

        /// <summary>
        /// JSON 对象（{"key":"value"}）
        /// </summary>
        Json = 7,

        /// <summary>
        /// JSON 数组（["item1","item2"]）
        /// </summary>
        JsonArray = 8
    }
    /// <summary>
    /// 系统字典值表
    /// </summary>
    [SugarTable("fd_dict_data", "系统字典值")]
    //[SugarIndex("index_{table}_type_value", nameof(DictTypeId), OrderByType.Asc, nameof(Value), OrderByType.Asc, IsUnique = true)]
    [SugarIndex("index_{table}_type_code", nameof(DictTypeCode), OrderByType.Asc)]
    [SugarIndex("index_{table}_parent", nameof(ParentId), OrderByType.Asc)]
    public partial class FdDictData : BaseEntity
    {
        /// <summary>
        /// 字典类型 ID（关联 fd_dict_type 表主键，用于高性能查询和外键约束）
        /// </summary>
        [SugarColumn(ColumnName = "dict_type_id", ColumnDescription = "字典类型 ID")]
        public string DictTypeId { get; set; }

        /// <summary>
        /// 字典类型导航属性（不映射到数据库）
        /// </summary>
        [Newtonsoft.Json.JsonIgnore]
        [Navigate(NavigateType.OneToOne, nameof(DictTypeId))]
        public FdDictType DictType { get; set; }

        /// <summary>
        /// 字典类型编码（冗余字段，便于直观查询和调试，与 DictTypeId 保持一致）
        /// </summary>
        [SugarColumn(ColumnName = "dict_type_code", ColumnDescription = "字典类型编码", IsNullable = true)]
        public string? DictTypeCode { get; set; }

        /// <summary>
        /// 字典标签（前端显示的文本）
        /// </summary>
        [SugarColumn(ColumnName = "label", ColumnDescription = "字典标签")]
        public virtual string Label { get; set; }

        /// <summary>
        /// 字典键值（实际存储到业务数据表中的值，建议格式：0、1 或 SYS_XXX）
        /// </summary>
        [SugarColumn(ColumnName = "value", ColumnDescription = "字典键值")]
        public virtual string Value { get; set; }

        /// <summary>
        /// 值的数据类型（用于前端自动渲染和后端自动序列化）
        /// </summary>
        [SugarColumn(ColumnName = "value_type", ColumnDescription = "值的数据类型", DefaultValue = "0")]
        public virtual DictValueType ValueType { get; set; } = DictValueType.String;

        /// <summary>
        /// 字典编码（用于代码访问，建议格式：TYPE_CODE_01）
        /// </summary>
        [SugarColumn(ColumnName = "code", ColumnDescription = "字典编码", IsNullable = true)]
        public virtual string? Code { get; set; }

        /// <summary>
        /// 父级字典项 ID（支持树形结构，根节点为空或""）
        /// </summary>
        [SugarColumn(ColumnName = "parent_id", ColumnDescription = "父级 ID", DefaultValue = "")]
        public string ParentId { get; set; } = "";

        /// <summary>
        /// 层级深度（根节点为 0，子节点依次递增）
        /// </summary>
        [SugarColumn(ColumnName = "level", ColumnDescription = "层级", DefaultValue = "0")]
        public int Level { get; set; } = 0;

        /// <summary>
        /// 排序（数值越小越靠前，默认 100）
        /// </summary>
        [SugarColumn(ColumnName = "order_no", ColumnDescription = "排序", DefaultValue = "100")]
        public int OrderNo { get; set; } = 100;

        /// <summary>
        /// 备注说明
        /// </summary>
        [SugarColumn(ColumnName = "remark", ColumnDescription = "备注", IsNullable = true)]
        public string? Remark { get; set; }

        /// <summary>
        /// 状态标识（Tag 颜色：primary/success/warning/danger/info）
        /// </summary>
        [SugarColumn(ColumnName = "tag_type", ColumnDescription = "状态标识", IsNullable = true)]
        public string? TagType { get; set; }

        /// <summary>
        /// CSS 类名（自定义样式类名）
        /// </summary>
        [SugarColumn(ColumnName = "css_class", ColumnDescription = "CSS 类名", IsNullable = true)]
        public string? CssClass { get; set; }

        /// <summary>
        /// 列表样式（表格中显示时的额外样式类名）
        /// </summary>
        [SugarColumn(ColumnName = "list_class", ColumnDescription = "列表样式", IsNullable = true)]
        public string? ListClass { get; set; }

        /// <summary>
        /// 是否默认值（Y-是，N-否）。标记为默认值的选项会在下拉框中优先选中
        /// </summary>
        [SugarColumn(ColumnName = "is_default", ColumnDescription = "是否默认值")]
        public virtual YesNoEnum IsDefault { get; set; } = YesNoEnum.N;

        /// <summary>
        /// 状态（0-停用 1-启用）
        /// </summary>
        [SugarColumn(ColumnName = "status", ColumnDescription = "状态", DefaultValue = "1")]
        public StatusEnum Status { get; set; } = StatusEnum.Enable;

        /// <summary>
        /// 扩展数据（JSON 格式，保存业务功能的配置项）
        /// </summary>
        [SugarColumn(ColumnName = "ext_data", ColumnDescription = "扩展数据", ColumnDataType = StaticConfig.CodeFirst_BigString, IsNullable = true)]
        public string? ExtData { get; set; }

        /// <summary>
        /// 子字典项集合（导航属性，支持树形结构）
        /// </summary>
        [Navigate(NavigateType.OneToMany, nameof(ParentId))]
        public List<FdDictData> Children { get; set; }
    }
}
