using Fastdotnet.Core.Entities.Sys;

namespace Fastdotnet.Core.Dtos.Sys
{
    /// <summary>
    /// 国家标准详情 DTO（包含统计信息）
    /// </summary>
    public class FdNationalStandardDetailDto
    {
        /// <summary>
        /// 主键 ID
        /// </summary>
        public string Id { get; set; } = string.Empty;

        /// <summary>
        /// 国标编号
        /// </summary>
        public string StandardCode { get; set; } = string.Empty;

        /// <summary>
        /// 标准名称
        /// </summary>
        public string StandardName { get; set; } = string.Empty;

        /// <summary>
        /// 标准类型
        /// </summary>
        public string StandardType { get; set; } = string.Empty;

        /// <summary>
        /// 当前版本号
        /// </summary>
        public string CurrentVersion { get; set; } = string.Empty;

        /// <summary>
        /// 状态
        /// </summary>
        public bool Status { get; set; }

        /// <summary>
        /// 条目总数
        /// </summary>
        public int TotalItems { get; set; }

        /// <summary>
        /// 发布日期
        /// </summary>
        public DateTime? PublishDate { get; set; }

        /// <summary>
        /// 实施日期
        /// </summary>
        public DateTime? ImplementDate { get; set; }
    }
}
