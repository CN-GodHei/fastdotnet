using Fastdotnet.Core.Attributes;
using Fastdotnet.Core.Enum;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Fastdotnet.WebApi.Controllers.System
{
    /// <summary>
    /// 演示同时使用API作用域和权限验证的控制器
    /// </summary>
    [ApiController]
    [Route("api/combined-auth")]
    public class CombinedAuthController : ControllerBase
    {
        /// <summary>
        /// 管理端专用且需要"user.read"权限的接口
        /// </summary>
        [HttpGet("admin-with-permission")]
        [ApiUsageScope(ApiUsageScopeEnum.AdminOnly)]
        [Authorize(Policy = "user.read")]
        public IActionResult AdminWithPermission()
        {
            return Ok(new { message = "这是管理端专用接口，且需要user.read权限" });
        }

        /// <summary>
        /// 两端通用且需要"order.create"权限的接口
        /// </summary>
        [HttpPost("both-with-permission")]
        [ApiUsageScope(ApiUsageScopeEnum.Both)]
        [Authorize(Policy = "order.create")]
        public IActionResult BothWithPermission()
        {
            return Ok(new { message = "这是两端通用接口，但需要order.create权限" });
        }

        /// <summary>
        /// 仅App端可用且需要特定权限的接口
        /// </summary>
        [HttpGet("app-with-permission")]
        [ApiUsageScope(ApiUsageScopeEnum.AppOnly)]
        [Authorize(Policy = "profile.update")]
        public IActionResult AppWithPermission()
        {
            return Ok(new { message = "这是App端专用接口，且需要profile.update权限" });
        }
    }
}