
namespace Fastdotnet.Service.IService
{
    public interface IEmailService
    {
        /// <summary>
        /// 发送邮件
        /// </summary>
        /// <param name="toEmail">收件人邮箱</param>
        /// <param name="subject">邮件主题</param>
        /// <param name="body">邮件内容 (支持HTML)</param>
        /// <returns></returns>
        Task SendEmailAsync(string toEmail, string subject, string body);
    }
}
