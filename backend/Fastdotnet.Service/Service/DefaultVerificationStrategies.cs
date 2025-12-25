using Fastdotnet.Core.Entities.System;
using Fastdotnet.Core.IService;
using Fastdotnet.Service.IService;
using Fastdotnet.Service.Service;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;

namespace Fastdotnet.Service.Service
{
    /// <summary>
    /// 处理“用户注册”场景的验证码策略
    /// </summary>
    public class UserRegisterVerificationStrategy : EmailVerificationStrategyBase
    {
        public override string BusinessCode => "UserRegister";

        public UserRegisterVerificationStrategy(IEmailService emailService, IMemoryCache memoryCache, IBaseService<FdDictData, string> FdDictDataservice,
            IBaseService<SystemInfoConfig, string> SystemInfoConfigservice)
            : base(emailService, memoryCache, FdDictDataservice, SystemInfoConfigservice)
        {
        }
    }

    /// <summary>
    /// 处理“重置密码”场景的验证码策略
    /// </summary>
    public class ResetPasswordVerificationStrategy : EmailVerificationStrategyBase
    {
        public override string BusinessCode => "ResetPassword";

        public ResetPasswordVerificationStrategy(IEmailService emailService, IMemoryCache memoryCache, IBaseService<FdDictData, string> FdDictDataservice,
            IBaseService<SystemInfoConfig, string> SystemInfoConfigservice)
            : base(emailService, memoryCache, FdDictDataservice, SystemInfoConfigservice)
        {
        }
    }
}

/// <summary>
/// 处理“默认”场景的验证码策略
/// </summary>
public class DefaultVerificationStrategy : EmailVerificationStrategyBase
{
    public override string BusinessCode => "Default";

    public DefaultVerificationStrategy(IEmailService emailService, IMemoryCache memoryCache, IBaseService<FdDictData, string> FdDictDataservice,
        IBaseService<SystemInfoConfig, string> SystemInfoConfigservice)
        : base(emailService, memoryCache, FdDictDataservice, SystemInfoConfigservice)
    {
    }
}
