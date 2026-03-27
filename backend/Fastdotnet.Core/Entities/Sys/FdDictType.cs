namespace Fastdotnet.Core.Entities.Sys
{
    /// <summary>
    /// 系统字典类型表
    /// </summary>
    [SugarTable("fd_dict_type", "系统字典类型")]
    [SugarIndex("index_{table}_code", nameof(Code), OrderByType.Asc, IsUnique = true)]
    [SugarIndex("index_{table}_name", nameof(Name), OrderByType.Asc)]
    public partial class FdDictType : BaseEntity
    {
        /// <summary>
        /// 类型名称
        /// </summary>
        [SugarColumn(ColumnName = "name", ColumnDescription = "类型名称", Length = 100)]
        [Required, MaxLength(100)]
        public virtual string Name { get; set; }

        /// <summary>
        /// 类型编码（业务使用的主要字段，建议格式：SYS_XXX、BIZ_XXX）
        /// </summary>
        [SugarColumn(ColumnName = "code", ColumnDescription = "类型编码", Length = 100)]
        [Required, MaxLength(100)]
        public virtual string Code { get; set; }

        /// <summary>
        /// 排序（数值越小越靠前）
        /// </summary>
        [SugarColumn(ColumnName = "order_no", ColumnDescription = "排序", DefaultValue = "100")]
        public int OrderNo { get; set; } = 100;

        /// <summary>
        /// 备注
        /// </summary>
        [SugarColumn(ColumnName = "remark", ColumnDescription = "备注", Length = 500, IsNullable = true)]
        [MaxLength(500)]
        public string? Remark { get; set; }

        /// <summary>
        /// 状态（0-停用 1-启用）
        /// </summary>
        [SugarColumn(ColumnName = "status", ColumnDescription = "状态", DefaultValue = "1")]
        public StatusEnum Status { get; set; } = StatusEnum.Enable;

        /// <summary>
        /// 是否系统内置（Y-是，N-否）。系统内置字典不可修改删除
        /// </summary>
        [SugarColumn(ColumnName = "sys_flag", ColumnDescription = "是否系统内置", DefaultValue = "N")]
        public virtual YesNoEnum SysFlag { get; set; } = YesNoEnum.N;

        /// <summary>
        /// 插件系统内置标记（Y-是，N-否）。用于区分插件自带的字典类型
        /// </summary>
        [SugarColumn(ColumnName = "plugin_sys_flag", ColumnDescription = "插件系统内置", DefaultValue = "N")]
        public virtual YesNoEnum PluginSysFlag { get; set; } = YesNoEnum.N;

        /// <summary>
        /// 所属插件 ID（仅当 plugin_sys_flag=Y 时有效）
        /// </summary>
        [SugarColumn(ColumnName = "plugin_id", ColumnDescription = "所属插件 ID", Length = 50, IsNullable = true)]
        [MaxLength(50)]
        public string? PluginId { get; set; }

        /// <summary>
        /// 字典值集合（导航属性，不映射到数据库）
        /// </summary>
        [Navigate(NavigateType.OneToMany, nameof(FdDictData.DictTypeId))]
        public List<FdDictData> Children { get; set; }
    }
}
