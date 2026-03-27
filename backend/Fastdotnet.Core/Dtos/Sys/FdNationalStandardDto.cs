using System;
using System.Collections.Generic;
using System.Text;

namespace Fastdotnet.Core.Dtos.Sys
{
    /// <summary>
    /// AutoMapper配置文件
    /// </summary>
    public class FdNationalStandardProfile : Profile
    {
        public FdNationalStandardProfile()
        {
            // Source -> Target
            CreateMap<FdNationalStandard, FdNationalStandardDto>().MaskSensitiveData();
            CreateMap<CreateFdNationalStandardDto, FdNationalStandard>();
            CreateMap<UpdateFdNationalStandardDto, FdNationalStandard>();
        }
    }

    /// <summary>
    ///新增传输模型
    /// </summary>
    public class CreateFdNationalStandardDto
    {

        /// <summary>
        /// 国标编号（唯一标识），如 "GB/T 2260", "GB/T 14396"
        /// </summary>
        [StringLength(255, ErrorMessage = "国标编号（唯一标识")]
        public string StandardCode { get; set; }

        /// <summary>
        /// 国标中文名称，如 "中华人民共和国行政区划代码"
        /// </summary>
        [StringLength(255, ErrorMessage = "国标中文名称")]
        public string StandardName { get; set; }

        /// <summary>
        /// 英文名称（可选）
        /// </summary>
        [Required(ErrorMessage = "英文名称（可选）不能为空")]
        [StringLength(255, ErrorMessage = "英文名称（可选）最多255个字符")]
        public string? StandardNameEn { get; set; }

        /// <summary>
        /// 标准类型：GB(强制性国标)、GB/T(推荐性国标)、DB(地方标准) 等
        /// </summary>
        [StringLength(255, ErrorMessage = "标准类型：GB(强制性国标)、GB/T(推荐性国标)、DB(地方标准) 等最多255个字符")]
        public string StandardType { get; set; }

        /// <summary>
        /// 发布部门，如 "国家标准化管理委员会"
        /// </summary>
        [Required(ErrorMessage = "发布部门")]
        [StringLength(255, ErrorMessage = "发布部门")]
        public string? PublishDepartment { get; set; }

        /// <summary>
        /// 发布日期
        /// </summary>
        [Required(ErrorMessage = "发布日期不能为空")]
        public DateTime? PublishDate { get; set; }

        /// <summary>
        /// 实施日期
        /// </summary>
        [Required(ErrorMessage = "实施日期不能为空")]
        public DateTime? ImplementDate { get; set; }

        /// <summary>
        /// 当前有效版本号，如 "2023"
        /// </summary>
        [StringLength(255, ErrorMessage = "当前有效版本号")]
        public string CurrentVersion { get; set; }

        /// <summary>
        /// 状态：1=现行有效，0=已废止
        /// </summary>
        public bool Status { get; set; }

        /// <summary>
        /// 总条目数（冗余字段，便于统计）
        /// </summary>
        public int TotalItems { get; set; }
    }

    /// <summary>
    ///修改传输模型
    /// </summary>
    public class UpdateFdNationalStandardDto
    {

        /// <summary>
        /// 国标编号（唯一标识），如 "GB/T 2260", "GB/T 14396"
        /// </summary>
        [StringLength(255, ErrorMessage = "国标编号（唯一标识）")]
        public string StandardCode { get; set; }

        /// <summary>
        /// 国标中文名称，如 "中华人民共和国行政区划代码"
        /// </summary>
        [StringLength(255, ErrorMessage = "国标中文名称")]
        public string StandardName { get; set; }

        /// <summary>
        /// 英文名称（可选）
        /// </summary>
        [StringLength(255, ErrorMessage = "英文名称（可选）最多255个字符")]
        public string? StandardNameEn { get; set; }

        /// <summary>
        /// 标准类型：GB(强制性国标)、GB/T(推荐性国标)、DB(地方标准) 等
        /// </summary>
        [StringLength(255, ErrorMessage = "标准类型：GB(强制性国标)、GB/T(推荐性国标)、DB(地方标准) 等最多255个字符")]
        public string StandardType { get; set; }

        /// <summary>
        /// 发布部门，如 "国家标准化管理委员会"
        /// </summary>
        [StringLength(255, ErrorMessage = "发布部门")]
        public string? PublishDepartment { get; set; }

        /// <summary>
        /// 发布日期
        /// </summary>
        public DateTime? PublishDate { get; set; }

        /// <summary>
        /// 实施日期
        /// </summary>
        public DateTime? ImplementDate { get; set; }

        /// <summary>
        /// 当前有效版本号，如 "2023"
        /// </summary>
        [StringLength(255, ErrorMessage = "当前有效版本号")]
        public string CurrentVersion { get; set; }

        /// <summary>
        /// 状态：1=现行有效，0=已废止
        /// </summary>
        public bool Status { get; set; }

        /// <summary>
        /// 总条目数（冗余字段，便于统计）
        /// </summary>
        public int TotalItems { get; set; }
    }

    /// <summary>
    ///输出传输模型
    /// </summary>
    public class FdNationalStandardDto
    {

        public string Id { get; set; } = string.Empty;


        /// <summary>
        /// 国标编号（唯一标识），如 "GB/T 2260", "GB/T 14396"
        /// </summary>

        public string StandardCode { get; set; }

        /// <summary>
        /// 国标中文名称，如 "中华人民共和国行政区划代码"
        /// </summary>

        public string StandardName { get; set; }

        /// <summary>
        /// 英文名称（可选）
        /// </summary>

        public string? StandardNameEn { get; set; }

        /// <summary>
        /// 标准类型：GB(强制性国标)、GB/T(推荐性国标)、DB(地方标准) 等
        /// </summary>

        public string StandardType { get; set; }

        /// <summary>
        /// 发布部门，如 "国家标准化管理委员会"
        /// </summary>

        public string? PublishDepartment { get; set; }

        /// <summary>
        /// 发布日期
        /// </summary>
        public DateTime? PublishDate { get; set; }

        /// <summary>
        /// 实施日期
        /// </summary>
        public DateTime? ImplementDate { get; set; }

        /// <summary>
        /// 当前有效版本号，如 "2023"
        /// </summary>

        public string CurrentVersion { get; set; }

        /// <summary>
        /// 状态：1=现行有效，0=已废止
        /// </summary>
        public bool Status { get; set; }

        /// <summary>
        /// 总条目数（冗余字段，便于统计）
        /// </summary>
        public int TotalItems { get; set; }
    }
}
