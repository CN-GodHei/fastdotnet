namespace Fastdotnet.Core.Dtos.Sys
{
    /// <summary>
    /// 邮件配置 - 输出DTO
    /// </summary>
    public class FdEmailConfigDto
    {
        public string Id { get; set; } = string.Empty;
        public string Host { get; set; } = string.Empty;
        public int Port { get; set; }
        public string Username { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public string SenderEmail { get; set; } = string.Empty;
        public string SenderName { get; set; } = string.Empty;
        public bool EnableSsl { get; set; }
    }

    /// <summary>
    /// 邮件配置 - 创建DTO
    /// </summary>
    public class FdCreateEmailConfigDto
    {
        [Required]
        [StringLength(200)]
        public string Host { get; set; } = string.Empty;

        [Required]
        public int Port { get; set; }

        [Required]
        [StringLength(200)]
        public string Username { get; set; } = string.Empty;

        [Required]
        [StringLength(200)]
        public string Password { get; set; } = string.Empty;

        [Required]
        [StringLength(200)]
        public string SenderEmail { get; set; } = string.Empty;

        [Required]
        [StringLength(200)]
        public string SenderName { get; set; } = string.Empty;

        public bool EnableSsl { get; set; }
    }

    /// <summary>
    /// 邮件配置 - 更新DTO
    /// </summary>
    public class FdUpdateEmailConfigDto
    {
        [Required]
        [StringLength(200)]
        public string Host { get; set; } = string.Empty;

        [Required]
        public int Port { get; set; }

        [Required]
        [StringLength(200)]
        public string Username { get; set; } = string.Empty;

        [Required]
        [StringLength(200)]
        public string Password { get; set; } = string.Empty;

        [Required]
        [StringLength(200)]
        public string SenderEmail { get; set; } = string.Empty;

        [Required]
        [StringLength(200)]
        public string SenderName { get; set; } = string.Empty;

        public bool EnableSsl { get; set; }
    }

    /// <summary>
    /// 测试发送邮件 - 请求DTO
    /// </summary>
    public class TestSendEmailDto
    {
        /// <summary>
        /// 收件人邮箱
        /// </summary>
        [Required]
        [EmailAddress]
        public string ToEmail { get; set; } = string.Empty;

        /// <summary>
        /// 邮件主题
        /// </summary>
        [StringLength(200)]
        public string Subject { get; set; } = "测试邮件";

        /// <summary>
        /// 邮件内容
        /// </summary>
        public string Body { get; set; } = "这是一封测试邮件，用于验证邮件配置是否正确。";
    }
}