using Dm.util;

namespace Fastdotnet.Core.Entities.Sys
{
    /// <summary>
    /// 国家标准主表（一个标准一条记录）
    /// </summary>
    /// <summary>
    /// 国家标准主表 - 实体信息
    /// </summary>
    [SugarTable("fd_national_standard", "国家标准主表")]
    public class FdNationalStandard
    : BaseEntity
    //: AuditableEntity
    {

        /// <summary>
        /// 国标编号（唯一标识），如 "GB/T 2260", "GB/T 14396"
        /// </summary>
        [SugarColumn(ColumnName = "standard_code", IsNullable = true, ColumnDescription = "国标编号（唯一标识），如 GB / T 2260, GB / T 14396")]
        public string StandardCode { get; set; }

        /// <summary>
        /// 国标中文名称，如 "中华人民共和国行政区划代码"
        /// </summary>
        [SugarColumn(ColumnName = "standard_name", IsNullable = true, ColumnDescription = "国标中文名称，如 中华人民共和国行政区划代码")]
        public string StandardName { get; set; }

        /// <summary>
        /// 英文名称（可选）
        /// </summary>
        [SugarColumn(ColumnName = "standard_name_en", IsNullable = true, ColumnDescription = "英文名称（可选）")]
        public string? StandardNameEn { get; set; }

        /// <summary>
        /// 标准类型：GB(强制性国标)、GB/T(推荐性国标)、DB(地方标准) 等
        /// </summary>
        [SugarColumn(ColumnName = "standard_type", IsNullable = true, ColumnDescription = "标准类型：GB(强制性国标)、GB/T(推荐性国标)、DB(地方标准) 等")]
        public string StandardType { get; set; }

        /// <summary>
        /// 发布部门，如 "国家标准化管理委员会"
        /// </summary>
        [SugarColumn(ColumnName = "publish_department", IsNullable = false, ColumnDescription = "发布部门，如 国家标准化管理委员会")]
        public string? PublishDepartment { get; set; }

        /// <summary>
        /// 发布日期
        /// </summary>
        [SugarColumn(ColumnName = "publish_date", IsNullable = false, ColumnDescription = "发布日期")]
        public DateTime? PublishDate { get; set; }

        /// <summary>
        /// 实施日期
        /// </summary>
        [SugarColumn(ColumnName = "implement_date", IsNullable = false, ColumnDescription = "实施日期")]
        public DateTime? ImplementDate { get; set; }

        /// <summary>
        /// 当前有效版本号，如 "2023"
        /// </summary>
        [SugarColumn(ColumnName = "current_version", IsNullable = true, ColumnDescription = "当前有效版本号，如 2023")]
        public string CurrentVersion { get; set; }

        /// <summary>
        /// 状态：1=现行有效，0=已废止
        /// </summary>
        [SugarColumn(ColumnName = "status", IsNullable = true, ColumnDescription = "状态：1=现行有效，0=已废止")]
        public bool Status { get; set; } = true;

        /// <summary>
        /// 总条目数（冗余字段，便于统计）
        /// </summary>
        [SugarColumn(ColumnName = "total_items", IsNullable = true, ColumnDescription = "总条目数（冗余字段，便于统计）")]
        public int TotalItems { get; set; } = 0;

        /// <summary>
        /// 扩展属性（JSON）
        /// </summary>
        [SugarColumn(ColumnName = "extra", IsNullable = true, ColumnDescription = "扩展属性（JSON）")]
        public string? Extra { get; set; }
    }
}
