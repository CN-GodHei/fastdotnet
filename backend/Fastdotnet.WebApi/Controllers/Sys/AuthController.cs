using Fastdotnet.Core.Dtos;
using Fastdotnet.Core.Dtos.Auth;
using Fastdotnet.Core.Enum;
using Fastdotnet.Core.Service.Sys;
using Fastdotnet.Core.Attributes;
using Fastdotnet.Core.Entities.Sys;
using Fastdotnet.Service.IService.Sys;

namespace Fastdotnet.WebApi.Controllers.Sys
{
    /// <summary>
    /// 登录授权管理
    /// </summary>
    [ApiController]
    [Route("api/auth")]
    [AllowAnonymous] // 将此控制器标记为允许匿名访问
    [SkipAntiReplay(Reason = "登录注册接口需要跳过防重放验证")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;
        private readonly IVerificationCodeManager _verificationCodeManager;
        private readonly ICaptcha _captcha;
        private readonly ICurrentUser _currentUser;
        private readonly IRepository<SystemInfoConfig> _systemConfigRepository;
        private readonly IRepository<FdAppUser> _appuserRepository;
        private readonly IFdDictDataService _dictDataService;

        public AuthController(IAuthService authService, IVerificationCodeManager verificationCodeManager, ICaptcha captcha,
            IRepository<SystemInfoConfig> systemConfigRepository, ICurrentUser currentUser, IRepository<FdAppUser> appuserRepository, IFdDictDataService dictDataService)
        {
            _authService = authService;
            _verificationCodeManager = verificationCodeManager;
            _captcha = captcha;
            _systemConfigRepository = systemConfigRepository;
            _currentUser = currentUser;
            _appuserRepository = appuserRepository;
            _dictDataService = dictDataService;
        }

        /// <summary>
        /// 管理员端登录
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        [HttpPost("admin/login")]
        [ApiUsageScope(ApiUsageScopeEnum.AdminOnly)]
        //[EncryptResponse]
        public async Task<LoginResultDto> AdminLogin([FromBody] LoginDto dto)
        {
            // 1. 检查系统配置是否启用验证码
            //var enableCaptchaConfig = await _systemConfigRepository.GetFirstAsync(c => c.Code == "EnableCaptcha");
            var enableCaptchaConfig = await _systemConfigRepository.GetFirstAsync(c => c.Code == "EnableCaptcha" && c.Belong == EnumHelper.ParseEnum<SystemCategory>(_currentUser.UserName));
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
            return new LoginResultDto() { Token = token };
        }

        /// <summary>
        /// 用户端登录
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        [HttpPost("app/login")]
        [ApiUsageScope(ApiUsageScopeEnum.AppOnly)]
        public async Task<LoginResultDto> AppLogin([FromBody] LoginDto dto)
        {
            // 1. 检查系统配置是否启用验证码
            var enableCaptchaConfig = await _systemConfigRepository.GetFirstAsync(c => c.Code == "EnableCaptcha" && c.Belong == EnumHelper.ParseEnum<SystemCategory>(_currentUser.UserName));
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
                        throw new BusinessException("验证码不能为空");
                        //return BadRequest(new { success = false, message = "验证码不能为空" });
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

            var token = await _authService.LoginAsync(dto, "App");
            return new LoginResultDto() { Token = token };
        }

        /// <summary>
        /// 发送App注册验证码
        /// </summary>
        [HttpPost("app/send-registration-code")]
        [ApiUsageScope(ApiUsageScopeEnum.AppOnly)]
        public async Task<ApiResult<bool>> SendRegistrationCode([FromBody] SendRegistrationCodeDto dto)
        {
            dto.IsValid();
            ApiResult<bool> result = new ApiResult<bool>();

            var userex = await _appuserRepository.GetFirstAsync(w => w.Email == dto.Email);
            if (userex != null)
            {
                throw new BusinessException("该邮箱已注册");
                //result.Msg = "该邮箱已注册!";
                //result.Data = false;
                //return result;
            }
            // 调用通用验证码服务，不指定业务码，使用默认策略
            await _verificationCodeManager.SendCodeAsync(dto.Email, "UserRegister");
            result.Msg = "验证码已发送至您的邮箱，请注意查收。";
            result.Data = true;
            return result;
        }

        [HttpPost("app/checkregistrusername")]
        [ApiUsageScope(ApiUsageScopeEnum.AppOnly)]
        public async Task<ApiResult<bool>> CheckRegistrUserName([FromBody] CheckRegistrUserNameDto dto)
        {
            dto.IsValid();
            ApiResult<bool> result = new ApiResult<bool>();
            var userex = await _appuserRepository.GetFirstAsync(w => w.Username == dto.Username);
            if (userex == null)
            {
                result.Msg = "未存在";
                result.Data = false;
                return result;
            }
            result.Msg = "已存在";
            result.Data = true;
            return result;
        }

        /// <summary>
        /// App端用户注册
        /// </summary>
        [HttpPost("app/register")]
        [ApiUsageScope(ApiUsageScopeEnum.AppOnly)]
        public async Task<ApiResult<bool>> AppRegister([FromBody] AppRegisterDto dto)
        {
            ApiResult<bool> result = new ApiResult<bool>();
            
            // 验证 DTO
            dto.IsValid();
            
            // 获取用户配置进行验证
            var userConfig = await _dictDataService.GetUserConfig();
            
            // 验证用户名
            var usernameMinLength = 6;
            var usernameMaxLength = 15;
            string? usernameRegex = null;
            var usernameRemark = "请按照要求设置用户名";
            
            var usernameMinConfig = userConfig.FirstOrDefault(c => c.Code == "CODE_10_01");
            if (usernameMinConfig != null && int.TryParse(usernameMinConfig.Value, out int minLen))
                usernameMinLength = minLen;
                
            var usernameMaxConfig = userConfig.FirstOrDefault(c => c.Code == "CODE_10_02");
            if (usernameMaxConfig != null && int.TryParse(usernameMaxConfig.Value, out int maxLen))
                usernameMaxLength = maxLen;
                
            var usernameRegexConfig = userConfig.FirstOrDefault(c => c.Code == "CODE_10_04");
            if (usernameRegexConfig != null)
            {
                if (!string.IsNullOrEmpty(usernameRegexConfig.Value))
                    usernameRegex = usernameRegexConfig.Value;
                if (!string.IsNullOrEmpty(usernameRegexConfig.Remark))
                    usernameRemark = usernameRegexConfig.Remark;
            }
            
            // 验证用户名长度
            if (dto.Username.Length < usernameMinLength || dto.Username.Length > usernameMaxLength)
            {
                result.Msg = $"用户名长度必须在{usernameMinLength}-{usernameMaxLength}位之间";
                result.Data = false;
                return result;
            }
            
            // 验证用户名格式（只有配置了正则才验证）
            if (!string.IsNullOrEmpty(usernameRegex) && !System.Text.RegularExpressions.Regex.IsMatch(dto.Username, usernameRegex))
            {
                result.Msg = usernameRemark;
                result.Data = false;
                return result;
            }
            
            // 验证密码（只有配置了正则才验证）
            string? passwordRegex = null;
            var passwordRemark = "请按照要求设置密码";
            var passwordConfig = userConfig.FirstOrDefault(c => c.Code == "CODE_10_03");
            if (passwordConfig != null)
            {
                if (!string.IsNullOrEmpty(passwordConfig.Value))
                    passwordRegex = passwordConfig.Value;
                if (!string.IsNullOrEmpty(passwordConfig.Remark))
                    passwordRemark = passwordConfig.Remark;
            }
            
            if (!string.IsNullOrEmpty(passwordRegex) && !System.Text.RegularExpressions.Regex.IsMatch(dto.Password, passwordRegex))
            {
                result.Msg = passwordRemark;
                result.Data = false;
                return result;
            }
            
            await _authService.AppRegisterAsync(dto);
            result.Msg = "注册成功";
            result.Data = true;
            return result;
        }
    }
}
