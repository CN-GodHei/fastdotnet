using System.ComponentModel.DataAnnotations;

namespace Fastdotnet.Core.Dtos.System
{
    /// <summary>
    /// 黑名单DTO
    /// </summary>
    public class FdBlacklistDto
    {
        public long Id { get; set; }

        /// <summary>
        /// 黑名单类型 (IP, User, ApiKey)
        /// </summary>
        [Required]
        public string Type { get; set; }

        /// <summary>
        /// 黑名单值 (具体的IP地址、用户ID或API密钥)
        /// </summary>
        [Required]
        public string Value { get; set; }

        /// <summary>
        /// 加入黑名单的原因
        /// </summary>
        public string Reason { get; set; }

        /// <summary>
        /// 过期时间 (可选)
        /// </summary>
        public DateTime? ExpiredAt { get; set; }

        /// <summary>
        /// 是否为系统内置
        /// </summary>
        public bool IsSystem { get; set; }

        public DateTime CreateTime { get; set; }
        public DateTime? UpdateTime { get; set; }
        public bool IsDeleted { get; set; }
    }

    /// <summary>
    /// 创建黑名单DTO
    /// </summary>
    public class CreateFdBlacklistDto
    {
        /// <summary>
        /// 黑名单类型 (IP, User, ApiKey)
        /// </summary>
        [Required]
        public string Type { get; set; }

        /// <summary>
        /// 黑名单值 (具体的IP地址、用户ID或API密钥)
        /// </summary>
        [Required]
        public string Value { get; set; }

        /// <summary>
        /// 加入黑名单的原因
        /// </summary>
        public string Reason { get; set; }

        /// <summary>
        /// 过期时间 (可选)
        /// </summary>
        public DateTime? ExpiredAt { get; set; }
    }

    /// <summary>
    /// 更新黑名单DTO
    /// </summary>
    public class UpdateFdBlacklistDto
    {
        /// <summary>
        /// 黑名单类型 (IP, User, ApiKey)
        /// </summary>
        [Required]
        public string Type { get; set; }

        /// <summary>
        /// 黑名单值 (具体的IP地址、用户ID或API密钥)
        /// </summary>
        [Required]
        public string Value { get; set; }

        /// <summary>
        /// 加入黑名单的原因
        /// </summary>
        public string Reason { get; set; }

        /// <summary>
        /// 过期时间 (可选)
        /// </summary>
        public DateTime? ExpiredAt { get; set; }
    }
}