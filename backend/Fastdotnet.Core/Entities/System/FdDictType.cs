

namespace Fastdotnet.Core.Entities.System
{
    /// <summary>
    /// 系统字典类型表
    /// </summary>
    [SugarTable("fd_dict_type", "系统字典类型")]
    [SugarIndex("index_{table}_N", nameof(Name), OrderByType.Asc)]
    [SugarIndex("index_{table}_C", nameof(Code), OrderByType.Asc)]
    public partial class FdDictType : BaseEntity
    {
        /// <summary>
        /// 名称
        /// </summary>
        [SugarColumn(ColumnName = "name", ColumnDescription = "名称", Length = 64)]
        [Required, MaxLength(64)]
        public virtual string Name { get; set; }

        /// <summary>
        /// 编码
        /// </summary>
        [SugarColumn(ColumnName = "code", ColumnDescription = "编码", Length = 50)]
        [Required, MaxLength(50)]
        public virtual string Code { get; set; }

        /// <summary>
        /// 排序
        /// </summary>
        [SugarColumn(ColumnName = "order_no", ColumnDescription = "排序", DefaultValue = "100")]
        public int OrderNo { get; set; } = 100;

        /// <summary>
        /// 备注
        /// </summary>
        [SugarColumn(ColumnName = "remark", ColumnDescription = "备注", Length = 256)]
        [MaxLength(256)]
        public string? Remark { get; set; }

        /// <summary>
        /// 状态
        /// </summary>
        [SugarColumn(ColumnName = "status", ColumnDescription = "状态", DefaultValue = "1")]
        public StatusEnum Status { get; set; } = StatusEnum.Enable;

        /// <summary>
        /// 是否是内置字典（Y-是，N-否）
        /// </summary>
        [SugarColumn(ColumnName = "sys_flag", ColumnDescription = "是否是内置字典", DefaultValue = "1")]
        public virtual YesNoEnum SysFlag { get; set; } = YesNoEnum.Y;

        /// <summary>
        /// 是否是租户字典（Y-是，N-否）
        /// </summary>
        //[SugarColumn(ColumnDescription = "是否是租户字典", DefaultValue = "2")]
        //public virtual YesNoEnum IsTenant { get; set; } = YesNoEnum.N;


        [SugarColumn(ColumnName = "plugin_sys_flag", ColumnDescription = "插件系统内置", DefaultValue = "0")]
        public virtual YesNoEnum PluginSysFlag { get; set; } = YesNoEnum.N;

        [SugarColumn(ColumnName = "plugin_id", ColumnDescription = "插件Id", Length = 50, IsNullable = true)]
        [MaxLength(50)]
        public string? PluginId { get; set; }
        /// <summary>
        /// 字典值集合
        /// </summary>
        [Navigate(NavigateType.OneToMany, nameof(FdDictData.DictTypeId))]
        public List<FdDictData> Children { get; set; }
    }
}
