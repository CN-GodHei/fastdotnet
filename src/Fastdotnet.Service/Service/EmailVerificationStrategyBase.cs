using System;
using System.Threading.Tasks;
using Fastdotnet.Core.Exceptions;
using Fastdotnet.Service.IService;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;

namespace Fastdotnet.Service.Service
{
    public abstract class EmailVerificationStrategyBase : IVerificationCodeStrategy
    {
        public abstract string BusinessCode { get; }

        protected readonly IEmailService _emailService;
        protected readonly IMemoryCache _memoryCache;
        protected readonly ILogger<EmailVerificationStrategyBase> _logger;

        // 定义默认和最大过期时间（分钟）
        private const int DefaultExpirationMinutes = 5;
        private const int MaxExpirationMinutes = 30;

        protected EmailVerificationStrategyBase(IEmailService emailService, IMemoryCache memoryCache, ILogger<EmailVerificationStrategyBase> logger)
        {
            _emailService = emailService;
            _memoryCache = memoryCache;
            _logger = logger;
        }

        public virtual async Task SendAsync(string recipient, int? expirationInMinutes = null)
        {
            var code = GenerateCode();
            var cacheKey = GetCacheKey(recipient);

            var finalExpiration = expirationInMinutes.HasValue ? expirationInMinutes.Value : DefaultExpirationMinutes;
            if (finalExpiration > MaxExpirationMinutes)
            {
                finalExpiration = MaxExpirationMinutes;
            }

            _memoryCache.Set(cacheKey, code, TimeSpan.FromMinutes(finalExpiration));

            _logger.LogInformation($"为业务场景 '{BusinessCode}' 生成验证码 {code}，接收者 {recipient}，有效期 {finalExpiration} 分钟。");

            var subject = $"您的验证码";
            var body = "";
            if (BusinessCode == "Default")
            {

                 body = $"您好，<br><br>您的验证码是：<b>{code}</b><br><br>该验证码将在 {finalExpiration} 分钟后失效，请勿泄露给他人。<br><br>如果您没有请求此验证码，请忽略此邮件。";
            }
            else
            {
                 body = $"您好，<br><br>您正在进行 '{BusinessCode}' 操作，您的验证码是：<b>{code}</b><br><br>该验证码将在 {finalExpiration} 分钟后失效，请勿泄露给他人。<br><br>如果您没有请求此验证码，请忽略此邮件。";
            }

            await _emailService.SendEmailAsync(recipient, subject, body);
        }

        public virtual Task<bool> VerifyAsync(string recipient, string code)
        {
            var cacheKey = GetCacheKey(recipient);

            if (string.IsNullOrWhiteSpace(code))
            {
                return Task.FromResult(false);
            }

            if (_memoryCache.TryGetValue(cacheKey, out string storedCode))
            {
                if (storedCode.Equals(code, StringComparison.OrdinalIgnoreCase))
                {
                    // 验证成功后立即移除，防止重复使用
                    _memoryCache.Remove(cacheKey);
                    return Task.FromResult(true);
                }
            }

            return Task.FromResult(false);
        }

        protected virtual string GenerateCode()
        {
            // 生成一个6位的随机数字验证码
            return new Random().Next(100000, 999999).ToString();
        }

        protected virtual string GetCacheKey(string recipient)
        {
            if (string.IsNullOrWhiteSpace(recipient))
                throw new BusinessException("接收者不能为空");

            return $"VerificationCode_{BusinessCode}_{recipient.ToLower()}";
        }
    }
}
