using Fastdotnet.Core.Attributes;
using Fastdotnet.Core.Enum;
using Fastdotnet.Core.Utils;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System.Threading.Tasks;

namespace Fastdotnet.WebApi.Controllers.Sys
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
        /// 加密特性使用示例 - 使用加密特性，请求参数解密固定使用RSA算法，响应加密使用RSA算法
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [EncryptRequest]
        [EncryptResponse]
        [HttpPost("encrypt/default")]
        [ApiUsageScope(ApiUsageScopeEnum.AdminOnly)]
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
        /// 加密特性使用示例 - 请求参数解密固定使用RSA算法，响应加密使用RSA算法
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [EncryptRequest]
        [EncryptResponse]
        [HttpPost("encrypt/rsa")]
        [ApiUsageScope(ApiUsageScopeEnum.AdminOnly)]
        [AllowAnonymous]
        public async Task<ExampleResponse> RSAEncryption([FromBody] ExampleRequest request)
        {
            var response = new ExampleResponse
            {
                Data = $"Processed with rsa: {request.Data}",
                Timestamp = DateTime.Now
            };
            
            return response;
        }

        /// <summary>
        /// 将参数使用RSA加密后返回
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost("encrypt-with-config-key")]
        [AllowAnonymous]
        public async Task<IActionResult> EncryptWithConfigKey([FromBody] ExampleRequest request)
        {
            try
            {
                
                // 从配置中获取公钥
                var configuration = HttpContext.RequestServices.GetRequiredService<IConfiguration>();
                var publicKey = configuration.GetSection("RequestParamEncryption:PublicKey").Value;
                
                if (string.IsNullOrEmpty(publicKey))
                {
                    return BadRequest("配置中未找到加密公钥");
                }
                
                
                // 将请求对象序列化为JSON字符串
                var jsonData = JsonConvert.SerializeObject(request);
                
                // 使用配置中的公钥进行RSA加密
                string encryptedData;
                try
                {
                    encryptedData = CryptographyUtils.RSAEncrypt(jsonData, publicKey);
                }
                catch (Exception ex)
                {
                    return BadRequest($"RSA加密失败: {ex.Message}. 请确保公钥格式正确");
                }
                
                var response = new
                {
                    OriginalData = request,
                    EncryptedData = encryptedData,
                    PublicKeyUsed = publicKey,
                    Success = true,
                    Message = "参数已使用配置中的公钥加密成功"
                };
                
                return Ok(response);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Success = false, Message = ex.Message });
            }
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