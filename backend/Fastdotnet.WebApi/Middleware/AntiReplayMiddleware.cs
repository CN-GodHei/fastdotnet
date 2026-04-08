using Fastdotnet.Core.Plugin;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.Extensions.Caching.Hybrid;
using NetTaste;
using System.Security.Cryptography;
using System.Text;

namespace Fastdotnet.WebApi.Middleware
{
    /// <summary>
    /// 防重放攻击中间件
    /// 验证请求头中的 X-Timestamp、X-Nonce、X-Signature
    /// </summary>
    public class AntiReplayMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<AntiReplayMiddleware> _logger;
        private const int TIMESTAMP_WINDOW_SECONDS = 300; // 时间窗口：5 分钟（±2.5 分钟）

        public AntiReplayMiddleware(RequestDelegate next, ILogger<AntiReplayMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context, IHybridCacheService cacheService, PluginReverseProxyRegistry proxyRegistry)
        {
//#if DEBUG
//            await _next(context);
//            return;
//#endif
            // 跳过特定路径（如登录、获取公钥等不需要防重放的接口）
            if (ShouldSkipAntiReplay(context))
            {
                await _next(context);
                return;
            }
            //特殊节点跳过,比如signalr的握手,无法手动标识跳过的属性
            string[] specialPath = ["negotiate"];
                        
            // 检查当前请求路径是否包含特殊路径
            var requestPath = context.Request.Path.Value?.ToLowerInvariant();
            if (!string.IsNullOrEmpty(requestPath) && specialPath.Any(path => requestPath.Contains(path)))
            {
                // _logger.LogDebug("跳过防重放验证(特殊路径): {Path}", context.Request.Path);
                await _next(context);
                return;
            }
            
            // 跳过插件静态资源文件(如 /plugins/{pluginId}/app/*)
            // 注意:插件API接口路径为 /api/plugins/...,需要防重放保护
            if (!string.IsNullOrEmpty(requestPath) && 
                requestPath.StartsWith("/plugins/", StringComparison.OrdinalIgnoreCase) &&
                !requestPath.StartsWith("/api/plugins/", StringComparison.OrdinalIgnoreCase))
            {
                // _logger.LogDebug("跳过防重放验证(插件静态资源): {Path}", context.Request.Path);
                await _next(context);
                return;
            }

            // 跳过被反向代理的路径（如插件微主机代理 /fdelsa/*）
            // 这些请求会被转发到内部微主机，由微主机自己处理认证和授权
            if (proxyRegistry != null && proxyRegistry.TryGetMatch(context.Request.Path.ToString(), out _, out _))
            {
                // _logger.LogDebug("跳过防重放验证(反向代理路径): {Path}", context.Request.Path);
                await _next(context);
                return;
            }

            // 获取请求头中的防重放字段
            var timestampStr = context.Request.Headers["X-Timestamp"].ToString();
            var nonce = context.Request.Headers["X-Nonce"].ToString();
            var signature = context.Request.Headers["X-Signature"].ToString();

            // 1. 验证必填字段
            if (string.IsNullOrEmpty(timestampStr) || string.IsNullOrEmpty(nonce))
            {
                context.Response.StatusCode = StatusCodes.Status409Conflict;
                context.Response.ContentType = "application/json; charset=utf-8";
                var errorResult = new ApiResult<object>
                {
                    Code = 409,
                    Msg = "缺少必要的防重放请求头：X-Timestamp, X-Nonce"
                };
                await context.Response.WriteAsJsonAsync(errorResult);
                return;
            }

            // 2. 验证时间戳有效性（防止旧请求重放）
            if (!long.TryParse(timestampStr, out long timestamp))
            {
                context.Response.StatusCode = StatusCodes.Status409Conflict;
                context.Response.ContentType = "application/json; charset=utf-8";
                var errorResult = new ApiResult<object>
                {
                    Code = 409,
                    Msg = "X-Timestamp 格式无效，应为 Unix 时间戳（秒）"
                };
                await context.Response.WriteAsJsonAsync(errorResult);
                return;
            }

            var requestTime = DateTimeOffset.FromUnixTimeSeconds(timestamp).UtcDateTime;
            var now = DateTime.UtcNow;
            var timeDiff = (now - requestTime).TotalSeconds;

            // 检查时间窗口（允许前后 2.5 分钟的误差）
            if (Math.Abs(timeDiff) > TIMESTAMP_WINDOW_SECONDS / 2)
            {
                context.Response.StatusCode = StatusCodes.Status409Conflict;
                context.Response.ContentType = "application/json; charset=utf-8";
                var errorResult = new ApiResult<object>
                {
                    Code = 409,
                    Msg = $"请求时间戳已过期，允许的误差范围为{TIMESTAMP_WINDOW_SECONDS / 2}秒"
                };
                await context.Response.WriteAsJsonAsync(errorResult);
                return;
            }

            // 3. 验证 Nonce 是否已使用（防重放核心逻辑）
            var nonceCacheKey = $"anti_replay:nonce:{nonce}";
            var existingNonce = await cacheService.GetAsync<string>(nonceCacheKey);

            if (!string.IsNullOrEmpty(existingNonce))
            {
                _logger.LogWarning("检测到重放攻击：Nonce={Nonce}, IP={IP}, Path={Path}",
                    nonce, GetClientIp(context), context.Request.Path);

                context.Response.StatusCode = StatusCodes.Status409Conflict;
                context.Response.ContentType = "application/json; charset=utf-8";
                var errorResult = new ApiResult<object>
                {
                    Code = 409,
                    Msg = "请求已处理，请勿重复提交（Nonce 已使用）"
                };
                await context.Response.WriteAsJsonAsync(errorResult);
                return;
            }

            // 4. 将 Nonce 写入缓存，设置过期时间（略大于时间窗口）
            var cacheOptions = new HybridCacheEntryOptions
            {
                Expiration = TimeSpan.FromSeconds(TIMESTAMP_WINDOW_SECONDS * 2) // 缓存 10 分钟
            };

            await cacheService.SetAsync(nonceCacheKey, nonce, cacheOptions);

            // 5. 验证签名（可选，如果需要）
            if (!string.IsNullOrEmpty(signature))
            {
                var isValidSignature = await VerifySignatureAsync(context, signature, cacheService);
                if (!isValidSignature)
                {
                    context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                    context.Response.ContentType = "application/json; charset=utf-8";
                    var errorResult = new ApiResult<object>
                    {
                        Code = 401,
                        Msg = "签名验证失败"
                    };
                    await context.Response.WriteAsJsonAsync(errorResult);
                    return;
                }
            }

            // 6. 继续执行后续中间件
            await _next(context);
        }

        /// <summary>
        /// 判断是否跳过防重放验证
        /// </summary>
        private bool ShouldSkipAntiReplay(HttpContext context)
        {
            // 跳过 OPTIONS 预检请求
            if (context.Request.Method.Equals("OPTIONS", StringComparison.OrdinalIgnoreCase))
            {
                return true;
            }

            // 跳过 OIDC Discovery 端点（公开配置信息）
            var path = context.Request.Path.Value?.ToLowerInvariant();
            if (path != null && path.StartsWith("/.well-known/"))
            {
                return true;
            }

            // 跳过 OIDC 授权和令牌端点（这些端点有自己的安全机制）
            if (path != null && (path.StartsWith("/connect/authorize") || 
                                 path.StartsWith("/connect/token") ||
                                 path.StartsWith("/connect/userinfo") ||
                                 path.StartsWith("/connect/introspect") ||
                                 path.StartsWith("/connect/revoke") ||
                                 path.StartsWith("/connect/logout")))
            {
                return true;
            }

            // 检查当前路由的 ActionDescriptor 是否有 SkipAntiReplayAttribute 标记
            var endpoint = context.GetEndpoint();
            if (endpoint != null)
            {
                // 检查方法上是否有特性
                var methodAttribute = endpoint.Metadata.GetMetadata<SkipAntiReplayAttribute>();
                if (methodAttribute != null)
                {
                    _logger.LogDebug("跳过防重放验证：{Path}, 原因：{Reason}",
                        context.Request.Path, methodAttribute.Reason);
                    return true;
                }

                // 检查控制器类上是否有特性
                var controllerActionDescriptor = endpoint.Metadata.GetMetadata<ControllerActionDescriptor>();
                if (controllerActionDescriptor != null)
                {
                    var controllerType = controllerActionDescriptor.ControllerTypeInfo;
                    var classAttribute = controllerType.GetCustomAttribute<SkipAntiReplayAttribute>();
                    if (classAttribute != null)
                    {
                        _logger.LogDebug("跳过防重放验证（控制器级别）: {Path}, 原因：{Reason}",
                            context.Request.Path, classAttribute.Reason);
                        return true;
                    }
                }
            }

            return false;
        }

        /// <summary>
        /// 验证签名
        /// </summary>
        private async Task<bool> VerifySignatureAsync(HttpContext context, string providedSignature, IHybridCacheService cacheService)
        {
            try
            {
                // 获取客户端 IP
                var clientIp = GetClientIp(context);

                // 构建签名字符串：Timestamp + Nonce + Method + Path + Body(可选)
                var timestamp = context.Request.Headers["X-Timestamp"].ToString();
                var nonce = context.Request.Headers["X-Nonce"].ToString();
                var method = context.Request.Method;
                var path = context.Request.Path.ToString();

                // 读取请求体（如果有的话）
                string body = "";
                if (context.Request.ContentLength > 0 &&
                    (method.Equals("POST", StringComparison.OrdinalIgnoreCase) ||
                     method.Equals("PUT", StringComparison.OrdinalIgnoreCase)))
                {
                    context.Request.EnableBuffering();
                    using (var reader = new StreamReader(context.Request.Body, Encoding.UTF8, leaveOpen: true))
                    {
                        body = await reader.ReadToEndAsync();
                        context.Request.Body.Position = 0;
                    }
                }

                // 构建待签名字符串
                // 使用 Base64 编码避免 Path/Body 中包含特殊字符（如 | ）导致解析问题
                // 去掉开头的 / （与前端保持一致，前端 btoa 时不包含开头的 /）
                if (path.StartsWith('/'))
                {
                    path = path.Substring(1);
                }
                var pathEncoded = Convert.ToBase64String(Encoding.UTF8.GetBytes(path));
                var bodyEncoded = Convert.ToBase64String(Encoding.UTF8.GetBytes(body));
                var signContent = $"{timestamp}|{nonce}|{method}|{pathEncoded}|{bodyEncoded}";
                //var signContent = $"{timestamp}|{nonce}|{method}|{path}|{body}";
                //Console.WriteLine(path);
                //Console.WriteLine(signContent);
                // TODO: 这里需要根据你的签名算法进行验证
                // 方案 1：使用固定的密钥（适合内部系统）
                // var secretKey = "your-secret-key-here";
                // var expectedSignature = ComputeHmacSha256(signContent, secretKey);

                // 方案 2：从缓存获取每个客户端的密钥（推荐）
                var clientSecretKey = await GetClientSecretKeyAsync(clientIp, cacheService);
                if (string.IsNullOrEmpty(clientSecretKey))
                {
                    // 如果是新客户端，可以生成一个密钥并返回给前端
                    _logger.LogWarning("未找到客户端密钥：IP={IP}", clientIp);
                    return false;
                }

                var expectedSignature = ComputeHmacSha256(signContent, clientSecretKey);

                return providedSignature.Equals(expectedSignature, StringComparison.OrdinalIgnoreCase);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "签名验证失败");
                return false;
            }
        }

        /// <summary>
        /// 获取客户端密钥（可以从配置或缓存中获取）
        /// </summary>
        private async Task<string> GetClientSecretKeyAsync(string clientIp, IHybridCacheService cacheService)
        {
            var cacheKey = $"client_secret:{clientIp}";
            var secretKey = await cacheService.GetAsync<string>(cacheKey);

            if (string.IsNullOrEmpty(secretKey))
            {
                // TODO: 这里可以从数据库或配置文件中加载客户端密钥
                // 示例：返回一个固定密钥（实际应该根据客户端 IP 或其他标识获取）
                // 建议为每个客户端生成独立的密钥
                secretKey = "default-secret-key-change-in-production";

                // 缓存起来
                await cacheService.SetAsync(cacheKey, secretKey, new HybridCacheEntryOptions
                {
                    Expiration = TimeSpan.FromHours(24)
                });
            }

            return secretKey;
        }

        /// <summary>
        /// 计算 HMAC-SHA256 签名
        /// </summary>
        private string ComputeHmacSha256(string message, string secretKey)
        {
            using (var hmac = new HMACSHA256(Encoding.UTF8.GetBytes(secretKey)))
            {
                var hashBytes = hmac.ComputeHash(Encoding.UTF8.GetBytes(message));
                return Convert.ToBase64String(hashBytes);
            }
        }

        /// <summary>
        /// 获取客户端 IP 地址
        /// </summary>
        private static string GetClientIp(HttpContext context)
        {
            // 优先从 X-Forwarded-For 获取（支持反向代理）
            if (context.Request.Headers.TryGetValue("X-Forwarded-For", out var forwardedFor))
            {
                var firstIp = forwardedFor.ToString().Split(',')[0].Trim();
                if (!string.IsNullOrWhiteSpace(firstIp) && IsValidIpAddress(firstIp))
                {
                    return firstIp;
                }
            }

            // 回退到 RemoteIpAddress
            var remoteIp = context.Connection.RemoteIpAddress;
            if (remoteIp == null)
                return "unknown";

            // 处理 IPv4 映射的 IPv6 地址
            if (remoteIp.IsIPv4MappedToIPv6)
            {
                remoteIp = remoteIp.MapToIPv4();
            }

            return remoteIp.ToString();
        }

        /// <summary>
        /// 简单校验是否为有效 IP
        /// </summary>
        private static bool IsValidIpAddress(string ip)
        {
            return !string.IsNullOrWhiteSpace(ip) &&
                   (System.Net.IPAddress.TryParse(ip, out _) || ip.Equals("localhost", StringComparison.OrdinalIgnoreCase));
        }
    }
}
