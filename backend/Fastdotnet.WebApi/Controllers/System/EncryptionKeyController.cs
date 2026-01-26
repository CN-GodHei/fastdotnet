using Fastdotnet.Core.Attributes;
using Fastdotnet.Core.Enum;
using Fastdotnet.Core.Utils;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Cryptography;

namespace Fastdotnet.WebApi.Controllers.System
{
    /// <summary>
    /// 加密密钥管理控制器
    /// 提供生成和获取不同加密算法公钥私钥的功能
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    [ApiUsageScope(ApiUsageScopeEnum.AdminOnly)]
    public class EncryptionKeyController : ControllerBase
    {
        /// <summary>
        /// 生成加密算法的密钥对
        /// </summary>
        /// <param name="algorithm">加密算法类型 (RSA, AES)</param>
        /// <returns>包含公钥和私钥的响应</returns>
        [HttpPost("generate")]
        [AllowAnonymous] // 允许匿名访问
        public async Task<IActionResult> GenerateKeyPair([FromBody] string algorithm)
        {
            if (string.IsNullOrWhiteSpace(algorithm))
            {
                return BadRequest("算法类型不能为空");
            }

            algorithm = algorithm.ToUpper();

            try
            {
                var encryptionService = new EncryptionService();

                if (algorithm is "RSA")
                {
                    // 非对称加密算法
                    var (publicKey, privateKey) = encryptionService.GenerateKeyPair(
                        algorithm switch
                        {
                            "RSA" => EncryptionService.AlgorithmType.RSA,
                            _ => throw new NotSupportedException($"不支持的加密算法: {algorithm}")
                        });

                    return Ok(new
                    {
                        Success = true,
                        Algorithm = algorithm,
                        PublicKey = publicKey,
                        PrivateKey = privateKey,
                        Message = $"{algorithm}密钥对生成成功"
                    });
                }
                else if (algorithm is "AES")
                {
                    // 对称加密算法
                    var key = GenerateSymmetricKey(algorithm);

                    return Ok(new
                    {
                        Success = true,
                        Algorithm = algorithm,
                        Key = key, // 对称加密算法只有单一密钥
                        Message = $"{algorithm}密钥生成成功"
                    });
                }
                else
                {
                    return BadRequest($"不支持的加密算法: {algorithm}");
                }
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
        /// 获取指定算法的公钥
        /// </summary>
        /// <param name="algorithm">加密算法类型</param>
        /// <returns>公钥信息</returns>
        [HttpGet("public/{algorithm}")]
        [AllowAnonymous] // 允许匿名访问
        public async Task<IActionResult> GetPublicKey(string algorithm)
        {
            if (string.IsNullOrWhiteSpace(algorithm))
            {
                return BadRequest("算法类型不能为空");
            }

            algorithm = algorithm.ToUpper();

            try
            {
                if (algorithm is "RSA")
                {
                    // 非对称加密算法 - 生成临时密钥对返回公钥
                    var encryptionService = new EncryptionService();
                    var (publicKey, _) = encryptionService.GenerateKeyPair(
                        algorithm switch
                        {
                            "RSA" => EncryptionService.AlgorithmType.RSA,
                            _ => throw new NotSupportedException($"不支持的加密算法: {algorithm}")
                        });

                    return Ok(new
                    {
                        Success = true,
                        Algorithm = algorithm,
                        PublicKey = publicKey
                    });
                }
                else if (algorithm is "AES")
                {
                    // 对称加密算法
                    var key = GenerateSymmetricKey(algorithm);

                    return Ok(new
                    {
                        Success = true,
                        Algorithm = algorithm,
                        Key = key
                    });
                }
                else
                {
                    return BadRequest($"不支持的加密算法: {algorithm}");
                }
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
        [HttpGet("private/{algorithm}")]
        [AllowAnonymous] // 允许匿名访问
        public async Task<IActionResult> GetPrivateKey(string algorithm)
        {
            if (string.IsNullOrWhiteSpace(algorithm))
            {
                return BadRequest("算法类型不能为空");
            }

            algorithm = algorithm.ToUpper();

            try
            {
                if (algorithm is "RSA")
                {
                    // 非对称加密算法 - 生成临时密钥对返回私钥
                    var encryptionService = new EncryptionService();
                    var (_, privateKey) = encryptionService.GenerateKeyPair(
                        algorithm switch
                        {
                            "RSA" => EncryptionService.AlgorithmType.RSA,
                            _ => throw new NotSupportedException($"不支持的加密算法: {algorithm}")
                        });

                    return Ok(new
                    {
                        Success = true,
                        Algorithm = algorithm,
                        PrivateKey = privateKey
                    });
                }
                else if (algorithm is "AES")
                {
                    // 对称加密算法，与公钥使用相同的密钥
                    var key = GenerateSymmetricKey(algorithm);

                    return Ok(new
                    {
                        Success = true,
                        Algorithm = algorithm,
                        Key = key
                    });
                }
                else
                {
                    return BadRequest($"不支持的加密算法: {algorithm}");
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    Success = false,
                    Message = $"获取私钥时发生错误: {ex.Message}"
                });
            }
        }

        /// <summary>
        /// 获取所有可用的加密算法列表
        /// </summary>
        /// <returns>算法列表</returns>
        [HttpGet("algorithms")]
        [AllowAnonymous] // 允许匿名访问
        public IActionResult GetAvailableAlgorithms()
        {
            var algorithms = new[]
            {
                new { Name = "RSA", Type = "非对称加密", Description = "广泛使用的公钥加密算法" },
                new { Name = "AES", Type = "对称加密", Description = "高级加密标准，常用AES-256" },
            };

            return Ok(new
            {
                Success = true,
                Algorithms = algorithms
            });
        }

        /// <summary>
        /// 生成对称加密算法的密钥
        /// </summary>
        /// <param name="algorithm">加密算法</param>
        /// <returns>生成的密钥</returns>
        private string GenerateSymmetricKey(string algorithm)
        {
            var keyLength = algorithm switch
            {
                "AES" => 32, // AES-256需要32字节密钥
                _ => 16  // 默认16字节
            };

            var keyBytes = new byte[keyLength];
            RandomNumberGenerator.Fill(keyBytes);
            return Convert.ToBase64String(keyBytes); // 返回Base64编码的密钥
        }
    }
}