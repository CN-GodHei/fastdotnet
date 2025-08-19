using Fastdotnet.Core.Models.Base;
using SqlSugar;

namespace Fastdotnet.Core.Entities.System
{
    /// <summary>
    /// 邮件服务配置
    /// </summary>
    [SugarTable("FdEmailConfig")]
    public class EmailConfig : BaseEntity
    {
        /// <summary>
        /// SMTP服务器地址
        /// </summary>
        [SugarColumn(Length = 200)]
        public string Host { get; set; }

        /// <summary>
        /// SMTP服务器端口
        /// </summary>
        public int Port { get; set; }

        /// <summary>
        /// SMTP用户名
        /// </summary>
        [SugarColumn(Length = 200)]
        public string Username { get; set; }

        /// <summary>
        /// SMTP密码或授权码 (注意：生产环境建议使用更安全的加密或密钥管理方案)
        /// </summary>
        [SugarColumn(Length = 200)]
        public string Password { get; set; }

        /// <summary>
        /// 发件人邮箱
        /// </summary>
        [SugarColumn(Length = 200)]
        public string SenderEmail { get; set; }

        /// <summary>
        /// 发件人名称
        /// </summary>
        [SugarColumn(Length = 200)]
        public string SenderName { get; set; }

        /// <summary>
        /// 是否启用SSL
        /// </summary>
        public bool EnableSsl { get; set; }
    }
}
