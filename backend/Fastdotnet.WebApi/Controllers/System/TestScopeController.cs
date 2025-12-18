using Fastdotnet.Core.Attributes;
using Fastdotnet.Core.Enum;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Fastdotnet.WebApi.Controllers.System
{
    /// <summary>
    /// 测试API作用域控制的控制器
    /// </summary>
    [ApiController]
    [Route("api/test-scope")]
    public class TestScopeController : ControllerBase
    {
        /// <summary>
        /// 管理端专用接口
        /// </summary>
        [HttpGet("admin-only")]
        [ApiUsageScope(ApiUsageScopeEnum.AdminOnly)]
        public IActionResult AdminOnly()
        {
            return Ok(new { message = "这是管理端专用接口，只有管理员token可以访问" });
        }

        /// <summary>
        /// App端专用接口
        /// </summary>
        [HttpGet("app-only")]
        [ApiUsageScope(ApiUsageScopeEnum.AppOnly)]
        public IActionResult AppOnly()
        {
            return Ok(new { message = "这是App端专用接口，只有App用户token可以访问" });
        }

        /// <summary>
        /// 两端通用接口
        /// </summary>
        [HttpGet("both")]
        [ApiUsageScope(ApiUsageScopeEnum.Both)]
        public IActionResult Both()
        {
            return Ok(new { message = "这是两端通用接口，管理员和App用户都可以访问" });
        }

        /// <summary>
        /// 无作用域限制接口
        /// </summary>
        [HttpGet("no-scope")]
        public IActionResult NoScope()
        {
            return Ok(new { message = "这是无作用域限制接口，所有人都可以访问" });
        }
    }
}