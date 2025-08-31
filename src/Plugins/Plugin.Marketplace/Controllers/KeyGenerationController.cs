using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Security.Cryptography;

namespace Fastdotnet.Plugin.Marketplace.Controllers
{
    [ApiController]
    [Route("api/marketplace/[controller]")]
    public class KeyGenerationController : ControllerBase
    {
        /// <summary>
        /// [仅限开发环境使用] 生成一个新的 RSA 密钥对 (4096 位)。
        /// </summary>
        /// <remarks>
        /// **安全警告:** 请妥善保管生成的私钥，切勿泄露或提交到版本控制系统。
        /// 公钥应硬编码到 Fastdotnet.Plugin.Core 中。
        /// 私钥应使用 user-secrets 或其他安全机制存储。
        /// </remarks>
        /// <returns>包含公钥和私钥的密钥对。</returns>
        [HttpGet("generate")]
        [AllowAnonymous]
        public IActionResult GenerateKeys()
        {
            try
            {
                using (var rsa = new RSACryptoServiceProvider(4096))
                {
                    // 导出公钥和私钥
                    var publicKey = Convert.ToBase64String(rsa.ExportRSAPublicKey());
                    var privateKey = Convert.ToBase64String(rsa.ExportRSAPrivateKey());

                    return Ok(new
                    {
                        PublicKey = publicKey,
                        PrivateKey = privateKey,
                        Note = "Please store the PrivateKey securely and do not commit it to version control."
                    });
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred while generating keys: {ex.Message}");
            }
        }
    }
}
