
using Fastdotnet.Core.Entities.Sys;

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

        public async Task InvokeAsync(HttpContext context, IRepository<FdRateLimitRule> rateLimitRuleRepository, IRateLimitCacheService rateLimitCacheService)
        {
            // 获取客户端IP地址
            var clientIp = context.Connection.RemoteIpAddress?.ToString();
            
            // 获取当前请求的路径
            var requestPath = context.Request.Path.ToString();
            
            // 这里可以扩展支持用户ID、API密钥等其他类型的限流检查
            
            // 1. 首先检查全局限流规则（针对路由路径）
            if (!string.IsNullOrEmpty(requestPath))
            {
                if (await IsRateLimitedForRouteAsync(rateLimitCacheService, requestPath))
                {
                    _logger.LogWarning("Rate limit exceeded for route: {RequestPath}", requestPath);
                    context.Response.StatusCode = StatusCodes.Status429TooManyRequests;
                    await context.Response.WriteAsync("Rate limit exceeded for this route.");
                    return;
                }
            }
            
            // 2. 检查客户端限流规则（针对IP地址）
            if (!string.IsNullOrEmpty(clientIp))
            {
                if (await IsRateLimitedForClientAsync(rateLimitCacheService, clientIp))
                {
                    _logger.LogWarning("Rate limit exceeded for IP: {ClientIp}", clientIp);
                    context.Response.StatusCode = StatusCodes.Status429TooManyRequests;
                    await context.Response.WriteAsync("Rate limit exceeded for this client.");
                    return;
                }
            }

            // 继续处理请求
            await _next(context);
        }

        /// <summary>
        /// 检查路由是否触发限流
        /// </summary>
        private async Task<bool> IsRateLimitedForRouteAsync(IRateLimitCacheService rateLimitCacheService, string route)
        {
            // 获取路由的限流规则
            var rule = await rateLimitCacheService.GetRateLimitRuleAsync("Route", route);
            if (rule == null)
            {
                // 如果没有具体的路由规则，尝试获取默认路由规则
                rule = await rateLimitCacheService.GetRateLimitRuleAsync("Route", "default");
                if (rule == null)
                    return false; // 没有限流规则，不限流
            }

            // 增加计数器
            var currentCount = await rateLimitCacheService.IncrementRateLimitCounterAsync("Route", route, rule.WindowSeconds);
            
            // 检查是否超过限制
            return currentCount > rule.PermitLimit;
        }

        /// <summary>
        /// 检查客户端是否触发限流
        /// </summary>
        private async Task<bool> IsRateLimitedForClientAsync(IRateLimitCacheService rateLimitCacheService, string clientIp)
        {
            // 获取IP地址的限流规则
            var rule = await rateLimitCacheService.GetRateLimitRuleAsync("IP", clientIp);
            if (rule == null)
            {
                // 如果没有具体的IP规则，尝试获取默认IP规则
                rule = await rateLimitCacheService.GetRateLimitRuleAsync("IP", "default");
                if (rule == null)
                    return false; // 没有限流规则，不限流
            }

            // 增加计数器
            var currentCount = await rateLimitCacheService.IncrementRateLimitCounterAsync("IP", clientIp, rule.WindowSeconds);
            
            // 检查是否超过限制
            return currentCount > rule.PermitLimit;
        }
    }
}