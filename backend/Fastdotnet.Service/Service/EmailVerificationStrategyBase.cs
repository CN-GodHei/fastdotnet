using Fastdotnet.Core.Entities.System;
using Fastdotnet.Core.Enum;
using Fastdotnet.Core.Exceptions;
using Fastdotnet.Core.IService;
using Fastdotnet.Service.IService;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;

namespace Fastdotnet.Service.Service
{
    public abstract class EmailVerificationStrategyBase : IVerificationCodeStrategy
    {
        public abstract string BusinessCode { get; }

        protected readonly IEmailService _emailService;
        protected readonly IMemoryCache _memoryCache;
        private readonly IBaseService<FdDictData, string> _FdDictDataservice;
        private readonly IBaseService<SystemInfoConfig, string> _SystemInfoConfigservice;
        //protected readonly ILogger<EmailVerificationStrategyBase> _logger;

        // 定义默认和最大过期时间（分钟）
        private const int DefaultExpirationMinutes = 5;
        private const int MaxExpirationMinutes = 30;

        protected EmailVerificationStrategyBase(IEmailService emailService, IMemoryCache memoryCache, IBaseService<FdDictData, string> FdDictDataservice,
            IBaseService<SystemInfoConfig, string> SystemInfoConfigservice
            //ILogger<EmailVerificationStrategyBase> logger
            )
        {
            _emailService = emailService;
            _memoryCache = memoryCache;
            //_logger = logger;
            _FdDictDataservice = FdDictDataservice;
            _SystemInfoConfigservice = SystemInfoConfigservice;
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
            _memoryCache.Remove(cacheKey);
            _memoryCache.Set(cacheKey, code, TimeSpan.FromMinutes(finalExpiration));
            string BusinessCodeName = "身份验证";
            if (BusinessCode != "Default")
            {
                var oper = (await _FdDictDataservice.GetListAsync(x => x.DictTypeId == "11921994044146608" && x.Value == BusinessCode)).FirstOrDefault();
                if (oper != null)
                {
                    BusinessCodeName = oper.Value;
                }
            }


            //_logger.LogInformation($"为业务场景 '{BusinessCode}' 生成验证码 {code}，接收者 {recipient}，有效期 {finalExpiration} 分钟。");
           var CopyrightInfo=(await _SystemInfoConfigservice.GetFirstAsync(x => x.Code == "CopyrightInfo" && x.Belong == SystemCategory.App)).Value;
           var globalTitle = (await _SystemInfoConfigservice.GetFirstAsync(x => x.Code == "globalTitle" && x.Belong == SystemCategory.App)).Value;
            var subject = $"【{globalTitle}】您的验证码";

            var body = "";

            body = $@"
                <div style='font-family: 'Segoe UI', Tahoma, Geneva, Verdana, sans-serif; max-width: 600px; margin: 0 auto; padding: 20px; background-color: #f8f9fa;'>
                    <div style='background-color: white; border-radius: 10px; box-shadow: 0 4px 12px rgba(0,0,0,0.1); overflow: hidden;'>
                        <div style='background: linear-gradient(135deg, #6a11cb 0%, #2575fc 100%); padding: 30px; text-align: center;'>
                            <h1 style='color: white; margin: 0; font-size: 28px;'>{globalTitle}</h1>
                            <p style='color: rgba(255,255,255,0.8); margin: 10px 0 0 0;'>安全验证</p>
                        </div>
                        
                        <div style='padding: 40px 30px; text-align: center;'>
                            <h2 style='color: #333; margin-top: 0;'>您好！</h2>
                            <p style='color: #666; line-height: 1.6; font-size: 16px;'>您正在进行 <strong style='color: #2575fc;'>{BusinessCodeName}</strong> ，请使用以下验证码完成操作：</p>
                            
                            <div style='margin: 30px 0;'>
                                <div style='display: inline-block; background-color: #f0f8ff; padding: 15px 30px; border-radius: 8px; border: 2px dashed #2575fc; font-size: 24px; letter-spacing: 4px; font-weight: bold; color: #2575fc;'>
                                    {code}
                                </div>
                            </div>
                            
                            <p style='color: #888; font-size: 14px; margin: 30px 0;'>
                                <strong>此验证码将在 <span style='color: #e74c3c;'>{finalExpiration} 分钟</span> 后失效</strong><br/>
                                请勿将验证码泄露给他人
                            </p>
                            
                            <hr style='margin: 30px 0; border: none; border-top: 1px solid #eee;' />
                            
                            <p style='color: #aaa; font-size: 13px; margin: 0;'>
                                如果您没有请求此验证码，请忽略此邮件。<br/>
                                此邮件由系统自动发送，请勿回复。
                            </p>
                        </div>
                        
                        <div style='background-color: #f9f9f9; padding: 20px; text-align: center; color: #999; font-size: 12px; border-top: 1px solid #eee;'>
                            {CopyrightInfo}
                        </div>
                    </div>
                </div>";

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