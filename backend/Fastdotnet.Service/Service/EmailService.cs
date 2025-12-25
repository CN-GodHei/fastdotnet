using System.Threading.Tasks;
using Fastdotnet.Core.Entities.System;
using Fastdotnet.Core.Exceptions;
using Fastdotnet.Core.IService;
using Fastdotnet.Service.IService;
using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Logging;
using MimeKit;
using MimeKit.Text;

namespace Fastdotnet.Service.Service
{
    public class EmailService : IEmailService
    {
        private readonly IRepository<EmailConfig> _emailConfigRepository;
        private readonly ILogger<EmailService> _logger;

        public EmailService(IRepository<EmailConfig> emailConfigRepository, ILogger<EmailService> logger)
        {
            _emailConfigRepository = emailConfigRepository;
            _logger = logger;
        }

        public async Task SendEmailAsync(string toEmail, string subject, string body)
        {
            var config = await _emailConfigRepository.GetFirstAsync(e => true);
            if (config == null || string.IsNullOrWhiteSpace(config.Host))
            {
                //_logger.LogError("邮件服务未配置，无法发送邮件。");
                throw new BusinessException("邮件服务未配置，请联系管理员。");
            }

            try
            {
                var message = new MimeMessage();
                message.From.Add(new MailboxAddress(config.SenderName, config.SenderEmail));
                message.To.Add(MailboxAddress.Parse(toEmail));
                message.Subject = subject;
                message.Body = new TextPart(TextFormat.Html) { Text = body };

                using var client = new SmtpClient();
                
                await client.ConnectAsync(config.Host, config.Port, config.EnableSsl);
                await client.AuthenticateAsync(config.Username, config.Password);
                await client.SendAsync(message);
                await client.DisconnectAsync(true);

                //_logger.LogInformation($"邮件已成功发送至 {toEmail}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "邮件发送失败。 To: {ToEmail}, Subject: {Subject}", toEmail, subject);
                throw new BusinessException("邮件发送失败，请稍后重试或联系管理员。");
            }
        }
    }
}
