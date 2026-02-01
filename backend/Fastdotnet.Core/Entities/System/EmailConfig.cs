

namespace Fastdotnet.Core.Entities.System
{
    /// <summary>
    /// 邮件服务配置
    /// </summary>
    [SugarTable("fd_email_config", "邮件服务配置")]
    public class EmailConfig : BaseEntity
    {
        /// <summary>
        /// SMTP服务器地址
        /// </summary>
        [SugarColumn(ColumnName = "host", Length = 200, ColumnDescription = "SMTP服务器地址")]
        public string Host { get; set; }

        /// <summary>
        /// SMTP服务器端口
        /// </summary>
        [SugarColumn(ColumnName = "port", ColumnDescription = "SMTP服务器端口")]
        public int Port { get; set; }

        /// <summary>
        /// SMTP用户名
        /// </summary>
        [SugarColumn(ColumnName = "username", Length = 200, ColumnDescription = "SMTP用户名")]
        public string Username { get; set; }

        /// <summary>
        /// SMTP密码或授权码 (注意：生产环境建议使用更安全的加密或密钥管理方案)
        /// </summary>
        [SugarColumn(ColumnName = "password", Length = 200, ColumnDescription = "SMTP密码或授权码 (注意：生产环境建议使用更安全的加密或密钥管理方案)")]
        public string Password { get; set; }

        /// <summary>
        /// 发件人邮箱
        /// </summary>
        [SugarColumn(ColumnName = "sender_email", Length = 200, ColumnDescription = "发件人邮箱")]
        public string SenderEmail { get; set; }

        /// <summary>
        /// 发件人名称
        /// </summary>
        [SugarColumn(ColumnName = "sender_name", Length = 200, ColumnDescription = "发件人名称")]
        public string SenderName { get; set; }

        /// <summary>
        /// 是否启用SSL
        /// </summary>
        [SugarColumn(ColumnName = "enable_ssl", ColumnDescription = "是否启用SSL")]
        public bool EnableSsl { get; set; }
    }
}
