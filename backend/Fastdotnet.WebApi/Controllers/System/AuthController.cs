using Fastdotnet.Core.Entities.System;
using Fastdotnet.Core.Exceptions;
using Fastdotnet.Core.IService;
using Fastdotnet.Core.Models.Auth;
using Fastdotnet.Service.IService;
using Lazy.Captcha.Core;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace Fastdotnet.WebApi.Controllers.System
{
    /// <summary>
    /// 登录授权管理
    /// </summary>
    [ApiController]
    [Route("api/auth")]
    [AllowAnonymous] // 将此控制器标记为允许匿名访问
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;
        private readonly IVerificationCodeManager _verificationCodeManager;
        private readonly ICaptcha _captcha;
        private readonly IRepository<SystemConfig> _systemConfigRepository;

        public AuthController(IAuthService authService, IVerificationCodeManager verificationCodeManager, ICaptcha captcha, IRepository<SystemConfig> systemConfigRepository)
        {
            _authService = authService;
            _verificationCodeManager = verificationCodeManager;
            _captcha = captcha;
            _systemConfigRepository = systemConfigRepository;
        }
        
        /// <summary>
        /// 管理员端登录
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        [HttpPost("admin/login")]
        public async Task<IActionResult> AdminLogin([FromBody] LoginDto dto)
        {
            // 1. 检查系统配置是否启用验证码
            var enableCaptchaConfig = await _systemConfigRepository.GetFirstAsync(c => c.Code == "EnableCaptcha");
            var enableCaptcha = enableCaptchaConfig?.Value?.ToString()?.ToLower() == "true";
            
            // 2. 如果启用了验证码，则进行验证
            if (enableCaptcha)
            {
                // 3. 检查验证码类型
                var captchaTypeConfig = await _systemConfigRepository.GetFirstAsync(c => c.Code == "CaptchaType");
                var captchaType = captchaTypeConfig?.Value?.ToString() ?? "normal";
                
                // 4. 如果是图形验证码，则验证
                if (captchaType == "normal")
                {
                    // 5. 检查验证码字段是否为空
                    if (string.IsNullOrEmpty(dto.CaptchaId) || string.IsNullOrEmpty(dto.CaptchaCode))
                    {
                        //return BadRequest(new { success = false, message = "验证码不能为空" });
                        throw new BusinessException("验证码不能为空");
                    }

                    // 6. 验证图形验证码
                    if (!_captcha.Validate(dto.CaptchaId, dto.CaptchaCode))
                    {
                        throw new BusinessException("验证码错误");
                        //return BadRequest(new { success = false, message = "验证码错误" });
                    }
                }
                // 如果是其他类型的验证码（如行为验证码），可以在这里添加相应的验证逻辑
                // 例如，如果是行为验证码，可能不需要CaptchaId和CaptchaCode，而是有其他验证方式
            }
            
            var token = await _authService.LoginAsync(dto, "Admin");
            return Ok(new { Token = token });
        }

        /// <summary>
        /// 用户端登录
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        [HttpPost("app/login")]
        public async Task<IActionResult> AppLogin([FromBody] LoginDto dto)
        {
            // 1. 检查系统配置是否启用验证码
            var enableCaptchaConfig = await _systemConfigRepository.GetFirstAsync(c => c.Code == "EnableCaptcha");
            var enableCaptcha = enableCaptchaConfig?.Value?.ToString()?.ToLower() == "true";
            
            // 2. 如果启用了验证码，则进行验证
            if (enableCaptcha)
            {
                // 3. 检查验证码类型
                var captchaTypeConfig = await _systemConfigRepository.GetFirstAsync(c => c.Code == "CaptchaType");
                var captchaType = captchaTypeConfig?.Value?.ToString() ?? "normal";
                
                // 4. 如果是图形验证码，则验证
                if (captchaType == "normal")
                {
                    // 5. 检查验证码字段是否为空
                    if (string.IsNullOrEmpty(dto.CaptchaId) || string.IsNullOrEmpty(dto.CaptchaCode))
                    {
                        return BadRequest(new { success = false, message = "验证码不能为空" });
                    }
                    
                    // 6. 验证图形验证码
                    if (!_captcha.Validate(dto.CaptchaId, dto.CaptchaCode))
                    {
                        return BadRequest(new { success = false, message = "验证码错误" });
                    }
                }
                // 如果是其他类型的验证码（如行为验证码），可以在这里添加相应的验证逻辑
                // 例如，如果是行为验证码，可能不需要CaptchaId和CaptchaCode，而是有其他验证方式
            }
            
            var token = await _authService.LoginAsync(dto, "App");
            return Ok(new { Token = token });
        }

        /// <summary>
        /// 发送App注册验证码
        /// </summary>
        [HttpPost("app/send-registration-code")]
        public async Task<IActionResult> SendRegistrationCode([FromBody] SendRegistrationCodeDto dto)
        {
            // 调用通用验证码服务，不指定业务码，使用默认策略
            await _verificationCodeManager.SendCodeAsync(dto.Email, null);
            return Ok(new { Message = "验证码已发送至您的邮箱，请注意查收。" });
        }

        /// <summary>
        /// App端用户注册
        /// </summary>
        [HttpPost("app/register")]
        public async Task<IActionResult> AppRegister([FromBody] AppRegisterDto dto)
        {
            await _authService.AppRegisterAsync(dto);
            return Ok(new { Message = "注册成功" });
        }
    }
}
