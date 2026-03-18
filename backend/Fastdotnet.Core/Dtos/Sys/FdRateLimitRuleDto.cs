namespace Fastdotnet.Core.Dtos.Sys
{
    /// <summary>
    /// 限流规则DTO
    /// </summary>
    public class FdRateLimitRuleDto
    {
        public long Id { get; set; }

        /// <summary>
        /// 限流类型 (IP, User, ApiKey)
        /// </summary>
        [Required]
        public string Type { get; set; }

        /// <summary>
        /// 限流键 (具体的IP地址、用户ID或API密钥)
        /// </summary>
        [Required]
        public string Key { get; set; }

        /// <summary>
        /// 允许的最大请求数
        /// </summary>
        [Required]
        public int PermitLimit { get; set; }

        /// <summary>
        /// 时间窗口 (以秒为单位)
        /// </summary>
        [Required]
        public int WindowSeconds { get; set; }

        /// <summary>
        /// 规则描述
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// 是否为系统内置
        /// </summary>
        public bool IsSystem { get; set; }

        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public bool IsDeleted { get; set; }
    }

    /// <summary>
    /// 创建限流规则DTO
    /// </summary>
    public class CreateFdRateLimitRuleDto
    {
        /// <summary>
        /// 限流类型 (IP, User, ApiKey)
        /// </summary>
        [Required]
        public string Type { get; set; }

        /// <summary>
        /// 限流键 (具体的IP地址、用户ID或API密钥)
        /// </summary>
        [Required]
        public string Key { get; set; }

        /// <summary>
        /// 允许的最大请求数
        /// </summary>
        [Required]
        [Range(1, int.MaxValue)]
        public int PermitLimit { get; set; }

        /// <summary>
        /// 时间窗口 (以秒为单位)
        /// </summary>
        [Required]
        [Range(1, int.MaxValue)]
        public int WindowSeconds { get; set; }

        /// <summary>
        /// 规则描述
        /// </summary>
        public string Description { get; set; }
    }

    /// <summary>
    /// 更新限流规则DTO
    /// </summary>
    public class UpdateFdRateLimitRuleDto
    {
        /// <summary>
        /// 限流类型 (IP, User, ApiKey)
        /// </summary>
        [Required]
        public string Type { get; set; }

        /// <summary>
        /// 限流键 (具体的IP地址、用户ID或API密钥)
        /// </summary>
        [Required]
        public string Key { get; set; }

        /// <summary>
        /// 允许的最大请求数
        /// </summary>
        [Required]
        [Range(1, int.MaxValue)]
        public int PermitLimit { get; set; }

        /// <summary>
        /// 时间窗口 (以秒为单位)
        /// </summary>
        [Required]
        [Range(1, int.MaxValue)]
        public int WindowSeconds { get; set; }

        /// <summary>
        /// 规则描述
        /// </summary>
        public string Description { get; set; }
    }
}