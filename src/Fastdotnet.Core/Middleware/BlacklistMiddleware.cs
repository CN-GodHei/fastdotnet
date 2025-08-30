using Fastdotnet.Core.Services.System;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;

namespace Fastdotnet.Core.Middleware
{
    /// <summary>
    /// 黑名单中间件
    /// </summary>
    public class BlacklistMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<BlacklistMiddleware> _logger;

        public BlacklistMiddleware(RequestDelegate next, ILogger<BlacklistMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context, IRateLimitCacheService rateLimitCacheService)
        {
            // 获取客户端IP地址
            var clientIp = context.Connection.RemoteIpAddress?.ToString();
            
            // 这里可以扩展支持用户ID、API密钥等其他类型的黑名单检查
            
            // 检查是否在黑名单中
            if (!string.IsNullOrEmpty(clientIp))
            {
                if (await rateLimitCacheService.IsBlacklistedAsync("IP", clientIp))
                {
                    _logger.LogWarning("Blocked request from blacklisted IP: {ClientIp}", clientIp);
                    context.Response.StatusCode = StatusCodes.Status403Forbidden;
                    await context.Response.WriteAsync("Access denied due to blacklist.");
                    return;
                }
            }

            // 继续处理请求
            await _next(context);
        }
    }
}