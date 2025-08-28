using Fastdotnet.Core.Models.Auth;
using Fastdotnet.Service.IService;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace Fastdotnet.WebApi.Controllers
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

        public AuthController(IAuthService authService, IVerificationCodeManager verificationCodeManager)
        {
            _authService = authService;
            _verificationCodeManager = verificationCodeManager;
        }
        /// <summary>
        /// 管理员端登录
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        [HttpPost("admin/login")]
        public async Task<IActionResult> AdminLogin([FromBody] LoginDto dto)
        {
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
