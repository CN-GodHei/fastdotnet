using Microsoft.AspNetCore.Http;
using System.Text;

namespace Fastdotnet.WebApi.Middleware
{
    /// <summary>
    /// OIDC 调试中间件 - 记录 Token 端点的原始请求
    /// </summary>
    public class OidcDebugMiddleware
    {
        private readonly RequestDelegate _next;

        public OidcDebugMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            // 只拦截 Token 端点
            if (context.Request.Path.StartsWithSegments("/connect/token", StringComparison.OrdinalIgnoreCase))
            {
                Console.WriteLine($"[OIDC Debug] ===== TOKEN ENDPOINT REQUEST =====");
                Console.WriteLine($"[OIDC Debug] Method: {context.Request.Method}");
                Console.WriteLine($"[OIDC Debug] Path: {context.Request.Path}");
                Console.WriteLine($"[OIDC Debug] Content-Type: {context.Request.ContentType}");
                
                // 读取请求体
                if (context.Request.Body.CanRead && context.Request.ContentLength > 0)
                {
                    context.Request.EnableBuffering();
                    using var reader = new StreamReader(context.Request.Body, Encoding.UTF8, leaveOpen: true);
                    var body = await reader.ReadToEndAsync();
                    context.Request.Body.Position = 0;
                    
                    // 隐藏敏感信息
                    var sanitizedBody = body
                        .Replace("client_secret=[^&]*", "client_secret=[REDACTED]")
                        .Replace("code=[^&]*", "code=[REDACTED]")
                        .Replace("code_verifier=[^&]*", "code_verifier=[REDACTED]");
                    
                    Console.WriteLine($"[OIDC Debug] Body: {sanitizedBody}");
                }
                
                Console.WriteLine($"[OIDC Debug] ========================================");
            }

            await _next(context);
        }
    }
}
