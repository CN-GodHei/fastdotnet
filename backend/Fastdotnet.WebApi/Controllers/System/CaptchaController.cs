using Lazy.Captcha.Core;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace Fastdotnet.WebApi.Controllers
{
    /// <summary>
    /// 验证码控制器
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    [AllowAnonymous] // 允许匿名访问
    public class CaptchaController : ControllerBase
    {
        private readonly ICaptcha _captcha;
        private readonly CaptchaOptions _captchaOptions;

        public CaptchaController(ICaptcha captcha, IOptions<CaptchaOptions> captchaOptions)
        {
            _captcha = captcha;
            _captchaOptions = captchaOptions.Value;
        }

        /// <summary>
        /// 生成验证码图片
        /// </summary>
        /// <param name="id">验证码标识符，通常是用户会话ID或GUID</param>
        /// <returns>GIF/PNG格式的验证码图片</returns>
        [HttpGet("generate")]
        [ProducesResponseType(typeof(FileContentResult), 200)]
        [ProducesResponseType(typeof(string), 400)]
        public IActionResult Generate(string id)
        {
            try
            {
                // 生成验证码
                var captchaInfo = _captcha.Generate(id);
                var stream = new MemoryStream(captchaInfo.Bytes);
                
                // 返回图片文件
                // 根据配置决定是 GIF 还是 PNG
                var contentType = _captchaOptions.ImageOption.Animation ? "image/gif" : "image/png";
                return File(stream, contentType);
            }
            catch (Exception ex)
            {
                // 处理异常
                return BadRequest(new { error = "Failed to generate captcha", details = ex.Message });
            }
        }

        /// <summary>
        /// 验证验证码 (仅供测试使用)
        /// 在正常的登录流程中，验证码验证应在后端完成，而不是通过此接口。
        /// </summary>
        /// <param name="id">验证码标识符</param>
        /// <param name="code">用户输入的验证码</param>
        /// <returns>验证结果</returns>
        [HttpPost("validate")]
        [ProducesResponseType(typeof(bool), 200)]
        [ProducesResponseType(typeof(string), 400)]
        //[ApiExplorerSettings(IgnoreApi = true)] // 在Swagger文档中隐藏此接口
        public IActionResult Validate([FromQuery] string id, [FromQuery] string code)
        {
            try
            {
                // 验证验证码
                var isValid = _captcha.Validate(id, code);
                return Ok(new { success = isValid });
            }
            catch (Exception ex)
            {
                // 处理异常
                return BadRequest(new { error = "Failed to validate captcha", details = ex.Message });
            }
        }
    }
}