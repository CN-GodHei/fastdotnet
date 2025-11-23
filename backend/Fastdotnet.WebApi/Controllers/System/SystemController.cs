using Fastdotnet.Plugin.Core.Security;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
namespace Fastdotnet.WebApi.Controllers.System
{
    /// <summary>
    /// 系统信息控制器
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    [Authorize] //
    public class SystemController : ControllerBase
    {
        /// <summary>
        /// 获取当前服务器的唯一机器指纹
        /// </summary>
        /// <returns>服务器的机器指纹字符串</returns>
        [HttpGet("machine-fingerprint")]
        [ProducesResponseType(typeof(string), 200)]
        [AllowAnonymous]
        public IActionResult GetMachineFingerprint()
        {
            var fingerprint = LicenseValidator.GenerateMachineFingerprint();
            //var fingerprint = string.Empty;
            //if (string.IsNullOrEmpty(fingerprint))
            //{
            //    return Problem("Failed to generate machine fingerprint.");
            //}
            return Ok(fingerprint);
        }
    }
}
