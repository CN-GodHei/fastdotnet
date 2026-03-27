using System;
using System.Collections.Generic;
using System.Text;

namespace Fastdotnet.Core.Entities.Sys
{
    /// <summary>
    /// 国家标准条目明细表（标准中的具体条款/项目）
    /// </summary>
    /// <summary>
    /// 国家标准条目明细表 - 实体信息
    /// </summary>
    [SugarTable("fd_national_standard_item", "国家标准条目明细表")]
    public class FdNationalStandardItem
    : BaseEntity
    //: AuditableEntity
    {

        /// <summary>
        /// 关联的主表 ID
        /// </summary>
        [SugarColumn(ColumnName = "standard_id", IsNullable = true, ColumnDescription = "关联的主表 ID")]
        public string StandardId { get; set; }

        /// <summary>
        /// 标准项编码（在标准体系内唯一）
        ///示例：GB/T 2260 中的 "110000" (北京市)
        /// </summary>
        [SugarColumn(ColumnName = "item_code", IsNullable = true, ColumnDescription = "标准项编码（在标准体系内唯一）示例：GB / T 2260 ")]
        public string ItemCode { get; set; }

        /// <summary>
        /// 标准项名称
        /// </summary>
        [SugarColumn(ColumnName = "item_name", IsNullable = true, ColumnDescription = "标准项名称")]
        public string ItemName { get; set; }

        /// <summary>
        /// 英文名称（可选）
        /// </summary>
        [SugarColumn(ColumnName = "item_name_en", IsNullable = true, ColumnDescription = "英文名称（可选）")]
        public string? ItemNameEn { get; set; }

        /// <summary>
        /// 父级编码（用于树形结构，使用 ItemCode）
        /// </summary>
        [SugarColumn(ColumnName = "parent_code", IsNullable = true, ColumnDescription = "父级编码（用于树形结构，使用 ItemCode）")]
        public string? ParentCode { get; set; }

        /// <summary>
        /// 层级深度（从 1 开始）
        /// </summary>
        [SugarColumn(ColumnName = "level", IsNullable = true, ColumnDescription = "层级深度（从 1 开始）")]
        public int? Level { get; set; }

        /// <summary>
        /// 排序值
        /// </summary>
        [SugarColumn(ColumnName = "sort", IsNullable = true, ColumnDescription = "排序值", DefaultValue = "9999")]
        public int Sort { get; set; } = 9999;

        /// <summary>
        /// 状态：1=启用，0=废止
        /// </summary>
        [SugarColumn(ColumnName = "status", IsNullable = true, ColumnDescription = "状态：1=启用，0=废止", DefaultValue = "1")]
        public bool Status { get; set; } = true;

        /// <summary>
        /// 扩展属性（JSON）
        /// </summary>
        [SugarColumn(ColumnName = "extra", IsNullable = true, ColumnDescription = "扩展属性（JSON）")]
        public string? Extra { get; set; }
    }
}
