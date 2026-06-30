using Fastdotnet.Core.Attributes;
using Fastdotnet.Core.Utils;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Caching.Hybrid;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Fastdotnet.WebApi.Middleware
{
    /// <summary>
    /// 加密中间件：处理请求参数解密和响应数据加密
    /// </summary>
    public class EncryptionMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly IEncryptionKeyService _encryptionKeyService;
        private readonly IHttpContextAccessor _contextAccessor;
        private readonly ILogger<EncryptionMiddleware> _logger;
        
        public EncryptionMiddleware(RequestDelegate next, IEncryptionKeyService encryptionKeyService, IHttpContextAccessor contextAccessor, ILogger<EncryptionMiddleware> logger)
        {
            _next = next;
            _encryptionKeyService = encryptionKeyService;
            _contextAccessor = contextAccessor;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            // 排除 SignalR 相关路径，避免干扰 negotiate 和 WebSocket 连接
            if (context.Request.Path.StartsWithSegments("/universalhub", StringComparison.OrdinalIgnoreCase))
            {
                await _next(context);
                return;
            }

            //if (context.Resource is Microsoft.AspNetCore.Mvc.Filters.AuthorizationFilterContext mvcContext)
            //{

            //}
            var endpoint = context.GetEndpoint();
            if (endpoint != null)
            {
                var controllerActionDescriptor = endpoint.Metadata.GetMetadata<ControllerActionDescriptor>();

                if (controllerActionDescriptor != null)
                {
                    var methodInfo = controllerActionDescriptor.MethodInfo;
                    var controllerTypeInfo = controllerActionDescriptor.ControllerTypeInfo;

                    // 检查是否需要对请求参数进行解密
                    if (ShouldDecryptRequest(methodInfo, controllerTypeInfo))
                    {

                        // 读取并解密请求体
                        context.Request.EnableBuffering();
                        var requestBody = await ReadRequestBody(context.Request);
                        if (!string.IsNullOrEmpty(requestBody))
                        {
                            try
                            {
                                var decryptedBody = await DecryptRequestBody(requestBody);
                                await RewriteRequestBody(context.Request, decryptedBody);
                            }
                            catch (CryptographicException ex)
                            {
                                // 密钥不匹配（可能是公钥过期）
                                context.Response.StatusCode = 498; // 自定义状态码：Token/Key Expired
                                context.Response.ContentType = "application/json; charset=utf-8";
                                
                                var errorResponse = new
                                {
                                    Code = 498,
                                    Message = "加密密钥已过期，请刷新公钥后重试",
                                    NeedRefreshPublicKey = true
                                };
                                
                                // 使用 JsonSerializerOptions 避免中文转义
                                var jsonOptions = new System.Text.Json.JsonSerializerOptions
                                {
                                    Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping
                                };
                                
                                await context.Response.WriteAsync(
                                    System.Text.Json.JsonSerializer.Serialize(errorResponse, jsonOptions)
                                );
                                return;
                            }
                            catch (Exception ex)
                            {
                                context.Response.StatusCode = 400;
                                await context.Response.WriteAsync($"请求参数解密失败: {ex.Message}");
                                return;
                            }
                        }
                    }
                    // 保存原始响应流
                    var originalResponseBody = context.Response.Body;

                    using var newResponseBody = new MemoryStream();
                    context.Response.Body = newResponseBody;
                    // 执行下一个中间件
                    await _next(context);



                    // 检查是否需要对响应数据进行加密
                    if (ShouldEncryptResponse(methodInfo, controllerTypeInfo))
                    {
                        newResponseBody.Seek(0, SeekOrigin.Begin);
                        var responseBody = await new StreamReader(newResponseBody).ReadToEndAsync();

                        if (!string.IsNullOrEmpty(responseBody) && IsJsonResponse(responseBody))
                        {
                            try
                            {
                                var encryptedBody = await EncryptResponseBody(responseBody);

                                // 将私钥添加到响应头，供客户端解密使用
                                await AddPublicKeyToResponseHeader(context);

                                // 清空原始响应流并写入加密后的内容
                                context.Response.Body = originalResponseBody; // 恢复原始响应流
                                context.Response.ContentLength = null; // 重置内容长度
                                await context.Response.WriteAsync(encryptedBody);
                            }
                            catch (Exception ex)
                            {
                                // 发生错误时，恢复原始响应流
                                context.Response.Body = originalResponseBody;
                                context.Response.StatusCode = 500;
                                await context.Response.WriteAsync($"响应数据加密失败: {ex.Message}");
                                return;
                            }
                        }
                        else
                        {
                            // 如果不是JSON响应或为空，也需要将内容写回到原始响应流
                            context.Response.Body = originalResponseBody;
                            newResponseBody.Seek(0, SeekOrigin.Begin);
                            await newResponseBody.CopyToAsync(originalResponseBody);
                        }
                    }
                    else
                    {
                        // 如果不需要加密响应，将原响应内容复制回原始响应流
                        context.Response.Body = originalResponseBody;
                        newResponseBody.Seek(0, SeekOrigin.Begin);
                        await newResponseBody.CopyToAsync(originalResponseBody);
                    }
                }
            }
            else
            {
                // 如果没有端点信息，直接执行下一个中间件
                await _next(context);
            }
        }

        /// <summary>
        /// 将私钥添加到响应头
        /// </summary>
        private async Task AddPublicKeyToResponseHeader(HttpContext context)
        {
            var (success, privateKey) = await GetResponseEncryptionKeyAsync( true); // true 表示获取私钥
            if (success && !string.IsNullOrEmpty(privateKey))
            {
                // 将私钥或对称密钥添加到响应头
                context.Response.Headers[$"X-Rsa-PrivateKey"] = privateKey;
            }
        }

        /// <summary>
        /// 检查是否需要解密请求
        /// </summary>
        private bool ShouldDecryptRequest(MethodInfo methodInfo, Type controllerTypeInfo)
        {
            return EncryptionAttributeHelper.IsRequestEncryptionEnabled(methodInfo) ||
                   EncryptionAttributeHelper.IsRequestEncryptionEnabled(controllerTypeInfo);
        }

        /// <summary>
        /// 检查是否需要加密响应
        /// </summary>
        private bool ShouldEncryptResponse(MethodInfo methodInfo, Type controllerTypeInfo)
        {
            return EncryptionAttributeHelper.IsResponseEncryptionEnabled(methodInfo) ||
                   EncryptionAttributeHelper.IsResponseEncryptionEnabled(controllerTypeInfo);
        }


        /// <summary>
        /// 获取响应加密算法
        /// </summary>
        private string GetResponseEncryptionAlgorithm(MethodInfo methodInfo, Type controllerTypeInfo)
        {
            return EncryptionAttributeHelper.GetResponseEncryptionAlgorithm(methodInfo) ??
                   EncryptionAttributeHelper.GetResponseEncryptionAlgorithm(controllerTypeInfo) ??
                   "RSA";
        }

        /// <summary>
        /// 读取请求体
        /// </summary>
        private async Task<string> ReadRequestBody(HttpRequest request)
        {
            request.Body.Seek(0, SeekOrigin.Begin);
            using var reader = new StreamReader(request.Body, Encoding.UTF8);
            var body = await reader.ReadToEndAsync();
            request.Body.Seek(0, SeekOrigin.Begin);
            return body;
        }

        /// <summary>
        /// 重写请求体
        /// </summary>
        private async Task RewriteRequestBody(HttpRequest request, string newBodyContent)
        {
            var newBody = Encoding.UTF8.GetBytes(newBodyContent);
            request.Body = new MemoryStream(newBody);
            request.ContentLength = newBody.Length;
        }

        /// <summary>
        /// 解密请求体
        /// </summary>
        private async Task<string> DecryptRequestBody(string encryptedBody)
        {
            //var encryptionService = new EncryptionService();
            // 移除可能的引号
            var trimmedBody = encryptedBody.Trim('"');
            
            // 获取加密算法
            var endpoint = _contextAccessor.HttpContext?.GetEndpoint();
            var algorithm = "RSA"; // 默认
            if (endpoint != null)
            {
                var controllerActionDescriptor = endpoint.Metadata.GetMetadata<ControllerActionDescriptor>();
                if (controllerActionDescriptor != null)
                {
                    algorithm = EncryptionAttributeHelper.GetRequestEncryptionAlgorithm(controllerActionDescriptor.MethodInfo) ??
                               EncryptionAttributeHelper.GetRequestEncryptionAlgorithm(controllerActionDescriptor.ControllerTypeInfo) ??
                               "RSA";
                }
            }
            
            // 从缓存中获取私钥解密请求参数
            var (success, key) = await GetResponseEncryptionKeyAsync(true); // true 表示获取私钥
            if (!success || string.IsNullOrEmpty(key))
            {
                throw new InvalidOperationException($"无法获取{algorithm}解密密钥");
            }

            // 根据算法选择解密方式
            if (algorithm.ToUpper() == "HYBRID" || algorithm.ToUpper().Contains("AES"))
            {
                // 混合解密（RSA + AES）
                //Console.WriteLine($"[EncryptionMiddleware] 正在使用私钥解密，私钥: {key}");
                return Core.Utils.HybridEncryptionUtils.Decrypt(trimmedBody, key);
            }
            else
            {
                // RSA解密需要私钥
                return CryptographyUtils.RSADecrypt(trimmedBody, key);
            }
        }

        /// <summary>
        /// 加密响应体
        /// </summary>
        private async Task<string> EncryptResponseBody(string responseBody)
        {
            // 响应加密使用 AES 对称加密
            // 生成随机 AES 密钥和 IV
            using var aes = System.Security.Cryptography.Aes.Create();
            aes.KeySize = 256;
            aes.GenerateKey();
            aes.GenerateIV();

            byte[] aesKey = aes.Key;
            byte[] aesIV = aes.IV;

            // 用 AES 加密响应数据
            byte[] plainBytes = System.Text.Encoding.UTF8.GetBytes(responseBody);
            byte[] encryptedData;

            using (var encryptor = aes.CreateEncryptor())
            {
                encryptedData = encryptor.TransformFinalBlock(plainBytes, 0, plainBytes.Length);
            }

            // 将 AES 密钥和 IV 编码为 Base64，并通过响应头传递
            var keyBase64 = Convert.ToBase64String(aesKey);
            var ivBase64 = Convert.ToBase64String(aesIV);
            
            _contextAccessor.HttpContext?.Response.Headers.Append("X-Encryption-Key", keyBase64);
            _contextAccessor.HttpContext?.Response.Headers.Append("X-Encryption-IV", ivBase64);
            _contextAccessor.HttpContext?.Response.Headers.Append("X-Encryption-Algorithm", "AES-256-CBC");

            // 返回 Base64 编码的加密数据
            return Convert.ToBase64String(encryptedData);
        }

        /// <summary>
        /// 获取请求参数加密密钥（使用配置中的固定密钥）
        /// </summary>
        private string GetRequestParamKey(string algorithm, bool isForDecryption)
        {
            var configuration = _contextAccessor.HttpContext?.RequestServices.GetService<IConfiguration>();
            if (configuration != null)
            {
                var section = configuration.GetSection($"RequestParamEncryption");
                if (section.Value != null || section.GetChildren().Any())
                {
                    if (isForDecryption)
                    {
                        // 解密时使用私钥
                        return section["PrivateKey"];
                    }
                    else
                    {
                        // 加密时使用公钥
                        return section["PublicKey"];
                    }
                }
            }

            return "";
        }

        /// <summary>
        /// 从缓存中获取响应加密密钥（使用缓存中的动态密钥）isForDecryption true表示私钥
        /// </summary>
        private async Task<(bool success, string key)> GetResponseEncryptionKeyAsync(bool isForDecryption)
        {
            try
            {
                string key;
                if (isForDecryption)
                {
                    // 获取私钥
                    key = await _encryptionKeyService.GetOrCreatePrivateKeyAsync();
                }
                else
                {
                    // 获取公钥
                    key = await _encryptionKeyService.GetOrCreatePublicKeyAsync();
                }
                
                return (true, key);
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "[EncryptionMiddleware] 获取密钥失败");
                return (false, "");
            }
        }

        /// <summary>
        /// 生成对称加密算法的密钥
        /// </summary>
        private string GenerateSymmetricKey(string algorithm)
        {
            var keyLength = algorithm.ToUpper() switch
            {
                "AES" => 32, // AES-256需要32字节密钥
                _ => 16  // 默认16字节
            };

            var keyBytes = new byte[keyLength];
            RandomNumberGenerator.Fill(keyBytes);
            return Convert.ToBase64String(keyBytes); // 返回Base64编码的密钥
        }

        /// <summary>
        /// 检查是否为JSON响应
        /// </summary>
        private bool IsJsonResponse(string content)
        {
            content = content.Trim();
            return (content.StartsWith("{") && content.EndsWith("}")) ||
                   (content.StartsWith("[") && content.EndsWith("]"));
        }
    }
}