using System;
using System.Collections.Generic;
using System.Text;

namespace Fastdotnet.Core.Dtos.Sys
{
    /// <summary>
    /// AutoMapper配置文件
    /// </summary>
    public class FdNationalStandardItemProfile : Profile
    {
        public FdNationalStandardItemProfile()
        {
            // Source -> Target
            CreateMap<FdNationalStandardItem, FdNationalStandardItemDto>().MaskSensitiveData();
            CreateMap<CreateFdNationalStandardItemDto, FdNationalStandardItem>();
            CreateMap<UpdateFdNationalStandardItemDto, FdNationalStandardItem>();
        }
    }

    /// <summary>
    ///新增传输模型
    /// </summary>
    public class CreateFdNationalStandardItemDto
    {

        /// <summary>
        /// 关联的主表 ID
        /// </summary>
        [StringLength(255, ErrorMessage = "关联的主表 ID最多255个字符")]
        public string StandardId { get; set; }

        /// <summary>
        /// 标准项编码（在标准体系内唯一）
        ///示例：GB/T 2260 中的 "110000" (北京市)
        /// </summary>
        [StringLength(255, ErrorMessage = "标准项编码（在标准体系内唯一）示例：GB / T 2260 中的 110000")]
        public string ItemCode { get; set; }

        /// <summary>
        /// 标准项名称
        /// </summary>
        [StringLength(255, ErrorMessage = "标准项名称最多255个字符")]
        public string ItemName { get; set; }

        /// <summary>
        /// 英文名称（可选）
        /// </summary>
        [Required(ErrorMessage = "英文名称（可选）不能为空")]
        [StringLength(255, ErrorMessage = "英文名称（可选）最多255个字符")]
        public string? ItemNameEn { get; set; }

        /// <summary>
        /// 父级编码（用于树形结构，使用 ItemCode）
        /// </summary>
        [Required(ErrorMessage = "父级编码（用于树形结构，使用 ItemCode）不能为空")]
        [StringLength(255, ErrorMessage = "父级编码（用于树形结构，使用 ItemCode）最多255个字符")]
        public string? ParentCode { get; set; }

        /// <summary>
        /// 层级深度（从 1 开始）
        /// </summary>
        [Required(ErrorMessage = "层级深度（从 1 开始）不能为空")]
        public int? Level { get; set; }

        /// <summary>
        /// 排序值
        /// </summary>
        public int Sort { get; set; }

        /// <summary>
        /// 状态：1=启用，0=废止
        /// </summary>
        public bool Status { get; set; }
    }

    /// <summary>
    ///修改传输模型
    /// </summary>
    public class UpdateFdNationalStandardItemDto
    {

        /// <summary>
        /// 关联的主表 ID
        /// </summary>
        [StringLength(255, ErrorMessage = "关联的主表 ID最多255个字符")]
        public string StandardId { get; set; }

        /// <summary>
        /// 标准项编码（在标准体系内唯一）
        ///示例：GB/T 2260 中的 "110000" (北京市)
        /// </summary>
        [StringLength(255, ErrorMessage = "标准项编码（在标准体系内唯一）示例：GB / T 2260 中的 110000")]
        public string ItemCode { get; set; }

        /// <summary>
        /// 标准项名称
        /// </summary>
        [StringLength(255, ErrorMessage = "标准项名称最多255个字符")]
        public string ItemName { get; set; }

        /// <summary>
        /// 英文名称（可选）
        /// </summary>
        [StringLength(255, ErrorMessage = "英文名称（可选）最多255个字符")]
        public string? ItemNameEn { get; set; }

        /// <summary>
        /// 父级编码（用于树形结构，使用 ItemCode）
        /// </summary>
        [StringLength(255, ErrorMessage = "父级编码（用于树形结构，使用 ItemCode）最多255个字符")]
        public string? ParentCode { get; set; }

        /// <summary>
        /// 层级深度（从 1 开始）
        /// </summary>
        public int? Level { get; set; }

        /// <summary>
        /// 排序值
        /// </summary>
        public int Sort { get; set; }

        /// <summary>
        /// 状态：1=启用，0=废止
        /// </summary>
        public bool Status { get; set; }
    }

    /// <summary>
    /// 输出传输模型
    /// </summary>
    public class FdNationalStandardItemDto
    {
        /// <summary>
        /// 主键 ID
        /// </summary>
        public string Id { get; set; } = string.Empty;
    
        /// <summary>
        /// 关联的主表 ID
        /// </summary>
        public string StandardId { get; set; } = string.Empty;

        /// <summary>
        /// 标准项编码（在标准体系内唯一）
        ///示例：GB/T 2260 中的 "110000" (北京市)
        /// </summary>

        public string ItemCode { get; set; }

        /// <summary>
        /// 标准项名称
        /// </summary>

        public string ItemName { get; set; }

        /// <summary>
        /// 英文名称（可选）
        /// </summary>

        public string? ItemNameEn { get; set; }

        /// <summary>
        /// 父级编码（用于树形结构，使用 ItemCode）
        /// </summary>

        public string? ParentCode { get; set; }

        /// <summary>
        /// 层级深度（从 1 开始）
        /// </summary>
        public int? Level { get; set; }

        /// <summary>
        /// 排序值
        /// </summary>
        public int Sort { get; set; }

        /// <summary>
        /// 状态：1=启用，0=废止
        /// </summary>
        public bool Status { get; set; }

        /// <summary>
        /// 子节点列表（用于树形结构）
        /// </summary>
        public List<FdNationalStandardItemDto>? Children { get; set; }
    }
}
