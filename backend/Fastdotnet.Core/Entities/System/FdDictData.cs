using Fastdotnet.Core.Enum;
using Fastdotnet.Core.Models.Base;
using SqlSugar;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fastdotnet.Core.Entities.System
{
    /// <summary>
    /// 系统字典值表
    /// </summary>
    [SugarTable("fd_dict_data", "系统字典值")]
    [SugarIndex("index_{table}_TV", nameof(DictTypeId), OrderByType.Asc, nameof(Value), OrderByType.Asc, nameof(Code), OrderByType.Asc, IsUnique = true)]
    public partial class FdDictData : BaseEntity
    {
        /// <summary>
        /// 字典类型Id
        /// </summary>
        [SugarColumn(ColumnName = "dict_type_id", ColumnDescription = "字典类型Id", Length = 50)]
        public string DictTypeId { get; set; }

        /// <summary>
        /// 字典类型
        /// </summary>
        [Newtonsoft.Json.JsonIgnore]
        [Navigate(NavigateType.OneToOne, nameof(DictTypeId))]
        public FdDictType DictType { get; set; }

        /// <summary>
        /// 显示文本
        /// </summary>
        [SugarColumn(ColumnName = "label", ColumnDescription = "显示文本", Length = 256)]
        [Required, MaxLength(256)]
        public virtual string Label { get; set; }

        /// <summary>
        /// 值
        /// </summary>
        [SugarColumn(ColumnName = "value", ColumnDescription = "值", Length = 140)]
        [Required, MaxLength(256)]
        public virtual string Value { get; set; }

        /// <summary>
        /// 编码
        /// </summary>
        /// <remarks>
        /// </remarks>
        [SugarColumn(ColumnName = "code", ColumnDescription = "编码", Length = 50)]
        public virtual string Code { get; set; }

        /// <summary>
        /// 名称
        /// </summary>
        [SugarColumn(ColumnName = "name", ColumnDescription = "名称", Length = 256, IsNullable = true)]
        [MaxLength(256)]
        public virtual string? Name { get; set; }

        /// <summary>
        /// 显示样式-标签颜色
        /// </summary>
        [SugarColumn(ColumnName = "tag_type", ColumnDescription = "显示样式-标签颜色", Length = 16, IsNullable = true)]
        [MaxLength(16)]
        public string? TagType { get; set; }

        /// <summary>
        /// 显示样式-Style(控制显示样式)
        /// </summary>
        [SugarColumn(ColumnName = "style_setting", ColumnDescription = "显示样式-Style", Length = 512, IsNullable = true)]
        [MaxLength(512)]
        public string? StyleSetting { get; set; }

        /// <summary>
        /// 显示样式-Class(控制显示样式)
        /// </summary>
        [SugarColumn(ColumnName = "class_setting", ColumnDescription = "显示样式-Class", Length = 512, IsNullable = true)]
        [MaxLength(512)]
        public string? ClassSetting { get; set; }

        /// <summary>
        /// 排序
        /// </summary>
        [SugarColumn(ColumnName = "order_no", ColumnDescription = "排序", DefaultValue = "100")]
        public int OrderNo { get; set; } = 100;

        /// <summary>
        /// 备注
        /// </summary>
        [SugarColumn(ColumnName = "remark", ColumnDescription = "备注", Length = 2048, IsNullable = true)]
        [MaxLength(2048)]
        public string? Remark { get; set; }

        /// <summary>
        /// 拓展数据(保存业务功能的配置项)
        /// </summary>
        [SugarColumn(ColumnName = "ext_data", ColumnDescription = "拓展数据(保存业务功能的配置项)", ColumnDataType = StaticConfig.CodeFirst_BigString, IsNullable = true)]
        public string? ExtData { get; set; }

        /// <summary>
        /// 状态
        /// </summary>
        [SugarColumn(ColumnName = "status", ColumnDescription = "状态", DefaultValue = "1")]
        public StatusEnum Status { get; set; } = StatusEnum.Enable;
    }
}
