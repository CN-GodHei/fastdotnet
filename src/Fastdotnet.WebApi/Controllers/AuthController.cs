using Fastdotnet.Core.Models.Auth;
using Fastdotnet.Service.IService;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace Fastdotnet.WebApi.Controllers
{
    [ApiController]
    [Route("api/auth")]
    [AllowAnonymous] // 将此控制器标记为允许匿名访问
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        [HttpPost("admin/login")]
        public async Task<IActionResult> AdminLogin([FromBody] LoginDto dto)
        {
            var token = await _authService.LoginAsync(dto, "Admin");
            return Ok(new { Token = token });
        }

        [HttpPost("app/login")]
        public async Task<IActionResult> AppLogin([FromBody] LoginDto dto)
        {
            var token = await _authService.LoginAsync(dto, "App");
            return Ok(new { Token = token });
        }
    }
}