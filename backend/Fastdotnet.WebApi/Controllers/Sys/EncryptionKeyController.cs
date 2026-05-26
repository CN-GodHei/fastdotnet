using Fastdotnet.Core.Attributes;
using Fastdotnet.Core.Enum;
using Fastdotnet.Core.Utils;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Hybrid;
using System.Security.Cryptography;

namespace Fastdotnet.WebApi.Controllers.Sys
{
    /// <summary>
    /// 加密密钥管理控制器
    /// 提供生成和获取不同加密算法公钥私钥的功能
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    [ApiUsageScope(ApiUsageScopeEnum.Both)]
    public class EncryptionKeyController : ControllerBase
    {
        private readonly IEncryptionKeyService _encryptionKeyService;

        public EncryptionKeyController(IEncryptionKeyService encryptionKeyService)
        {
            _encryptionKeyService = encryptionKeyService;
        }
        /// <summary>
        /// 生成加密算法的密钥对
        /// </summary>
        /// <param name="algorithm">加密算法类型 (RSA)</param>
        /// <returns>包含公钥和私钥的响应</returns>
        [HttpPost("generate")]
        [AllowAnonymous] // 允许匿名访问
        public async Task<IActionResult> GenerateKeyPair([FromBody] string algorithm)
        {
            try
            {

                // 非对称加密算法
                var (publicKey, privateKey) = CryptographyUtils.GenerateRSAKeyPair();

                return Ok(new
                {
                    Success = true,
                    Algorithm = algorithm,
                    PublicKey = publicKey,
                    PrivateKey = privateKey,
                    Message = $"{algorithm}密钥对生成成功"
                });
            }
            catch (NotSupportedException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    Success = false,
                    Message = $"生成密钥对时发生错误: {ex.Message}"
                });
            }
        }

        /// <summary>
        /// 获取 RSA 公钥（用于混合加密）
        /// </summary>
        /// <returns>公钥信息</returns>
        [HttpGet("public-key")]
        [AllowAnonymous] // 允许匿名访问（登录前需要获取）
        public async Task<IActionResult> GetPublicKey()
        {
            try
            {
                var publicKey = await _encryptionKeyService.GetOrCreatePublicKeyAsync();

                return Ok(new
                {
                    Success = true,
                    Algorithm = "RSA",
                    PublicKey = publicKey,
                    Message = "RSA公钥获取成功"
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    Success = false,
                    Message = $"获取公钥时发生错误: {ex.Message}"
                });
            }
        }

        /// <summary>
        /// 获取指定算法的私钥
        /// </summary>
        /// <param name="algorithm">加密算法类型</param>
        /// <returns>私钥信息</returns>
        //[HttpGet("private/{algorithm}")]
        //[AllowAnonymous] // 允许匿名访问
        //public async Task<IActionResult> GetPrivateKey(string algorithm)
        //{

        //    try
        //    {
        //        var (_, privateKey) = CryptographyUtils.GenerateRSAKeyPair();

        //        return Ok(new
        //        {
        //            Success = true,
        //            Algorithm = algorithm,
        //            PrivateKey = privateKey
        //        });
        //    }
        //    catch (Exception ex)
        //    {
        //        return StatusCode(500, new
        //        {
        //            Success = false,
        //            Message = $"获取私钥时发生错误: {ex.Message}"
        //        });
        //    }
        //}
    }
}