
namespace Fastdotnet.Core.Dtos.System
{
    /// <summary>
    /// 邮件配置 - 输出DTO
    /// </summary>
    public class FdEmailConfigDto
    {
        public string Id { get; set; }
        public string Host { get; set; }
        public int Port { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public string SenderEmail { get; set; }
        public string SenderName { get; set; }
        public bool EnableSsl { get; set; }
    }

    /// <summary>
    /// 邮件配置 - 创建DTO
    /// </summary>
    public class FdCreateEmailConfigDto
    {
        [Required]
        [StringLength(200)]
        public string Host { get; set; }

        [Required]
        public int Port { get; set; }

        [Required]
        [StringLength(200)]
        public string Username { get; set; }

        [Required]
        [StringLength(200)]
        public string Password { get; set; }

        [Required]
        [StringLength(200)]
        public string SenderEmail { get; set; }

        [Required]
        [StringLength(200)]
        public string SenderName { get; set; }

        public bool EnableSsl { get; set; }
    }

    /// <summary>
    /// 邮件配置 - 更新DTO
    /// </summary>
    public class FdUpdateEmailConfigDto
    {
        [Required]
        [StringLength(200)]
        public string Host { get; set; }

        [Required]
        public int Port { get; set; }

        [Required]
        [StringLength(200)]
        public string Username { get; set; }

        [Required]
        [StringLength(200)]
        public string Password { get; set; }

        [Required]
        [StringLength(200)]
        public string SenderEmail { get; set; }

        [Required]
        [StringLength(200)]
        public string SenderName { get; set; }

        public bool EnableSsl { get; set; }
    }
}