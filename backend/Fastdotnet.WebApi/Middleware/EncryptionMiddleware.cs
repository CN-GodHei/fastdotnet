using Fastdotnet.Core.Attributes;
using Fastdotnet.Core.Utils;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Caching.Hybrid;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Text.Json.Serialization;

namespace Fastdotnet.WebApi.Middleware
{
    /// <summary>
    /// 加密中间件：处理请求参数解密和响应数据加密
    /// </summary>
    public class EncryptionMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly IHybridCacheService _cacheService;
        private readonly IHttpContextAccessor _contextAccessor;

        public EncryptionMiddleware(RequestDelegate next, IHybridCacheService cacheService, IHttpContextAccessor contextAccessor)
        {
            _next = next;
            _cacheService = cacheService;
            _contextAccessor = contextAccessor;
        }

        public async Task InvokeAsync(HttpContext context)
        {
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
                        var algorithm = "SM2"; // 固定使用SM2算法

                        // 读取并解密请求体
                        context.Request.EnableBuffering();
                        var requestBody = await ReadRequestBody(context.Request);
                        if (!string.IsNullOrEmpty(requestBody))
                        {
                            try
                            {
                                var decryptedBody = DecryptRequestBody(requestBody, algorithm);
                                await RewriteRequestBody(context.Request, decryptedBody);
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

                        var algorithm = GetResponseEncryptionAlgorithm(methodInfo, controllerTypeInfo);

                        newResponseBody.Seek(0, SeekOrigin.Begin);
                        var responseBody = await new StreamReader(newResponseBody).ReadToEndAsync();

                        if (!string.IsNullOrEmpty(responseBody) && IsJsonResponse(responseBody))
                        {
                            try
                            {
                                var encryptedBody = await EncryptResponseBody(responseBody, algorithm);

                                context.Response.Body.Seek(0, SeekOrigin.Begin);
                                context.Response.Body.SetLength(0);
                                await context.Response.WriteAsync(encryptedBody);

                                // 将公钥添加到响应头，供客户端解密使用
                                //await AddPublicKeyToResponseHeader(context, algorithm);
                            }
                            catch (Exception ex)
                            {
                                context.Response.StatusCode = 500;
                                await context.Response.WriteAsync($"响应数据加密失败: {ex.Message}");
                                return;
                            }
                        }
                    }
                    else
                    {
                        // 如果不需要加密响应，将原响应内容复制回原始响应流
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
        /// 将公钥添加到响应头
        /// </summary>
        private async Task AddPublicKeyToResponseHeader(HttpContext context, string algorithm)
        {
            var (success, publicKey) = await GetResponseEncryptionKeyAsync(algorithm, false); // false 表示获取公钥
            if (success && !string.IsNullOrEmpty(publicKey))
            {
                // 将公钥或对称密钥添加到响应头
                context.Response.Headers[$"X-{algorithm}-PublicKey"] = publicKey;
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
        /// 获取请求加密算法
        /// </summary>
        private string GetRequestEncryptionAlgorithm(MethodInfo methodInfo, Type controllerTypeInfo)
        {
            return EncryptionAttributeHelper.GetRequestEncryptionAlgorithm(methodInfo) ??
                   EncryptionAttributeHelper.GetRequestEncryptionAlgorithm(controllerTypeInfo) ??
                   "SM2";
        }

        /// <summary>
        /// 获取响应加密算法
        /// </summary>
        private string GetResponseEncryptionAlgorithm(MethodInfo methodInfo, Type controllerTypeInfo)
        {
            return EncryptionAttributeHelper.GetResponseEncryptionAlgorithm(methodInfo) ??
                   EncryptionAttributeHelper.GetResponseEncryptionAlgorithm(controllerTypeInfo) ??
                   "SM2";
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
        private string DecryptRequestBody(string encryptedBody, string algorithm)
        {
            var encryptionService = new EncryptionService();
            // 移除可能的引号
            var trimmedBody = encryptedBody.Trim('"');
            // 使用配置中的固定密钥解密请求参数
            var key = GetRequestParamKey("SM2", true); // 固定使用SM2算法，true 表示获取解密密钥（私钥）
            if (string.IsNullOrEmpty(key))
            {
                throw new InvalidOperationException($"无法获取{algorithm}解密密钥");
            }

            switch (algorithm.ToUpper())
            {
                case "SM2":
                    // SM2解密需要私钥
                    return encryptionService.DecryptWithSM2(trimmedBody, key);

                case "AES":
                    // AES解密需要密钥
                    return encryptionService.DecryptWithAES(trimmedBody, key);

                case "RSA":
                    // RSA解密需要私钥
                    return encryptionService.DecryptWithRSA(trimmedBody, key);

                default:
                    throw new NotSupportedException($"不支持的解密算法: {algorithm}");
            }
        }

        /// <summary>
        /// 加密响应体
        /// </summary>
        private async Task<string> EncryptResponseBody(string responseBody, string algorithm)
        {
            var encryptionService = new EncryptionService();

            // 从缓存中获取响应加密密钥
            var (success, key) = await GetResponseEncryptionKeyAsync(algorithm, false); // false 表示获取加密密钥（公钥或对称密钥）
            if (!success)
            {
                throw new InvalidOperationException($"无法获取{algorithm}加密密钥");
            }

            // 解析响应体以检查是否为ApiResult格式
            using var jsonDoc = JsonDocument.Parse(responseBody);
            var root = jsonDoc.RootElement;

            // 检查是否包含ApiResult的基本字段：Code、Msg、Data
            if (root.TryGetProperty("Code", out _) && 
                root.TryGetProperty("Msg", out _) && 
                root.TryGetProperty("Data", out var dataProperty))
            {
                // 这是一个ApiResult格式的响应，只加密Data字段
                var originalData = dataProperty.ToString();
                string encryptedData;

                switch (algorithm.ToUpper())
                {
                    case "SM2":
                        // SM2加密需要公钥
                        encryptedData = encryptionService.EncryptWithSM2(originalData, key);
                        break;

                    case "AES":
                        // AES加密需要密钥
                        encryptedData = encryptionService.EncryptWithAES(originalData, key);
                        break;

                    case "RSA":
                        // RSA加密需要公钥
                        encryptedData = encryptionService.EncryptWithRSA(originalData, key);
                        break;

                    case "SM4":
                        // SM4加密需要密钥
                        encryptedData = encryptionService.EncryptWithSM4(originalData, key);
                        break;

                    default:
                        throw new NotSupportedException($"不支持的加密算法: {algorithm}");
                }

                // 重建响应体，保持Code和Msg不变，只替换Data为加密内容
                using var doc = JsonDocument.Parse(responseBody);
                var jsonElement = doc.RootElement;
                var jsonObject = new JsonObject();
                
                foreach (var property in jsonElement.EnumerateObject())
                {
                    if (property.Name.Equals("Data", StringComparison.OrdinalIgnoreCase))
                    {
                        jsonObject[property.Name] = JsonValue.Create(encryptedData);
                    }
                    else
                    {
                        jsonObject[property.Name] = JsonNode.Parse(property.Value.GetRawText());
                    }
                }

                return jsonObject.ToJsonString();
            }
            else
            {
                // 不是ApiResult格式，按原来的方式加密整个响应体
                switch (algorithm.ToUpper())
                {
                    case "SM2":
                        // SM2加密需要公钥
                        return encryptionService.EncryptWithSM2(responseBody, key);

                    case "AES":
                        // AES加密需要密钥
                        return encryptionService.EncryptWithAES(responseBody, key);

                    case "RSA":
                        // RSA加密需要公钥
                        return encryptionService.EncryptWithRSA(responseBody, key);

                    case "SM4":
                        // SM4加密需要密钥
                        return encryptionService.EncryptWithSM4(responseBody, key);

                    default:
                        throw new NotSupportedException($"不支持的加密算法: {algorithm}");
                }
            }
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
        /// 从缓存中获取响应加密密钥（使用缓存中的动态密钥）
        /// </summary>
        private async Task<(bool success, string key)> GetResponseEncryptionKeyAsync(string algorithm, bool isForDecryption)
        {
            var keyType = isForDecryption ? "PrivateKey" : "PublicKey";
            var cacheKey = $"Fastdotnet_Encryption_Response_{algorithm}_{keyType}";

            // 首先尝试从缓存获取密钥
            var cachedKey = await _cacheService.GetAsync<string>(cacheKey);
            if (!string.IsNullOrEmpty(cachedKey))
            {
                return (true, cachedKey);
            }

            // 如果缓存中没有密钥，生成新的密钥对并存储到缓存
            var encryptionService = new EncryptionService();
            try
            {
                if (algorithm.ToUpper() is "SM2" or "RSA")
                {
                    // 非对称加密算法
                    var (publicKey, privateKey) = encryptionService.GenerateKeyPair(
                        algorithm.ToUpper() switch
                        {
                            "SM2" => EncryptionService.AlgorithmType.SM2,
                            "RSA" => EncryptionService.AlgorithmType.RSA,
                            _ => throw new NotSupportedException($"不支持的加密算法: {algorithm}")
                        });

                    // 将密钥对存入缓存，使用配置中的过期时间
                    var options = new HybridCacheEntryOptions
                    {
                        Expiration = TimeSpan.FromHours(24),                 // 24小时后过期
                        LocalCacheExpiration = TimeSpan.FromHours(24)          // 本地缓存24小时过期
                    };

                    await _cacheService.SetAsync($"Fastdotnet_Encryption_Response_{algorithm}_PublicKey", publicKey, options);
                    await _cacheService.SetAsync($"Fastdotnet_Encryption_Response_{algorithm}_PrivateKey", privateKey, options);

                    // 返回请求的密钥类型
                    var requestedKey = isForDecryption ? privateKey : publicKey;
                    return (true, requestedKey);
                }
                else if (algorithm.ToUpper() is "AES" or "SM4")
                {
                    // 对称加密算法，生成固定长度的密钥
                    var key = GenerateSymmetricKey(algorithm);

                    // 将密钥存入缓存，使用配置中的过期时间
                    var options = new HybridCacheEntryOptions
                    {
                        Expiration = TimeSpan.FromHours(24),                 // 24小时后过期
                        LocalCacheExpiration = TimeSpan.FromHours(24)          // 本地缓存24小时过期
                    };

                    var symmetricCacheKey = $"Fastdotnet_Encryption_Response_{algorithm}_{keyType}";
                    await _cacheService.SetAsync(symmetricCacheKey, key, options);

                    return (true, key);
                }

                return (false, "");
            }
            catch (Exception)
            {
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
                "SM4" => 16, // SM4需要16字节密钥
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