using Fastdotnet.Core.Attributes;
using Fastdotnet.Core.Enum;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

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
        
        /// <summary>
        /// 加密特性使用示例 - 使用默认加密设置
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [EncryptRequest(true, "SM2")]
        [EncryptResponse(true, "SM2")]
        [HttpPost("encrypt/default")]
        public async Task<IActionResult> DefaultEncryption([FromBody] ExampleRequest request)
        {
            var response = new ExampleResponse
            {
                Data = $"Processed: {request.Data}",
                Timestamp = DateTime.Now
            };
            
            return Ok(response);
        }

        /// <summary>
        /// 加密特性使用示例 - 使用AES算法
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [EncryptRequest(algorithm: "AES")]
        [EncryptResponse(algorithm: "AES")]
        [HttpPost("encrypt/aes")]
        public async Task<IActionResult> AesEncryption([FromBody] ExampleRequest request)
        {
            var response = new ExampleResponse
            {
                Data = $"Processed with AES: {request.Data}",
                Timestamp = DateTime.Now
            };
            
            return Ok(response);
        }

        /// <summary>
        /// 加密特性使用示例 - 禁用加密
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [EncryptRequest(false)] // 禁用请求加密
        [EncryptResponse(false)] // 禁用响应加密
        [HttpPost("encrypt/no-encryption")]
        public async Task<IActionResult> NoEncryption([FromBody] ExampleRequest request)
        {
            var response = new ExampleResponse
            {
                Data = $"Processed without encryption: {request.Data}",
                Timestamp = DateTime.Now
            };
            
            return Ok(response);
        }
    }

    /// <summary>
    /// 示例请求模型
    /// </summary>
    public class ExampleRequest
    {
        public string Data { get; set; }
        public string Token { get; set; }
    }

    /// <summary>
    /// 示例响应模型
    /// </summary>
    public class ExampleResponse
    {
        public string Data { get; set; }
        public DateTime Timestamp { get; set; }
        public bool Success { get; set; } = true;
    }
}