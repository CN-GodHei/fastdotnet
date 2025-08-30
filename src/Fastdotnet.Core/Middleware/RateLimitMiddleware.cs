using Fastdotnet.Core.Dtos.System;
using Fastdotnet.Core.Entities.System;
using Fastdotnet.Core.IService;
using Fastdotnet.Core.Services.System;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;

namespace Fastdotnet.Core.Middleware
{
    /// <summary>
    /// 限流中间件
    /// </summary>
    public class RateLimitMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<RateLimitMiddleware> _logger;

        public RateLimitMiddleware(RequestDelegate next, ILogger<RateLimitMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context, IRepository<FdBlacklist> rateLimitRuleRepository, IRateLimitCacheService rateLimitCacheService)
        {
            // 获取客户端IP地址
            var clientIp = context.Connection.RemoteIpAddress?.ToString();
            
            // 这里可以扩展支持用户ID、API密钥等其他类型的限流检查
            
            // 检查是否触发限流
            if (!string.IsNullOrEmpty(clientIp))
            {
                // 首先检查缓存中是否有触发限流
                if (await rateLimitCacheService.IsRateLimitedAsync("IP", clientIp))
                {
                    _logger.LogWarning("Rate limit exceeded for IP: {ClientIp}", clientIp);
                    context.Response.StatusCode = StatusCodes.Status429TooManyRequests;
                    await context.Response.WriteAsync("Rate limit exceeded.");
                    return;
                }
            }

            // 继续处理请求
            await _next(context);
        }
    }
}