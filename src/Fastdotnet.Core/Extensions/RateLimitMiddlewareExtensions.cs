using Fastdotnet.Core.Middleware;
using Microsoft.AspNetCore.Builder;

namespace Fastdotnet.Core.Extensions
{
    /// <summary>
    /// 限流中间件扩展方法
    /// </summary>
    public static class RateLimitMiddlewareExtensions
    {
        public static IApplicationBuilder UseBlacklist(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<BlacklistMiddleware>();
        }

        public static IApplicationBuilder UseRateLimit(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<RateLimitMiddleware>();
        }
    }
}