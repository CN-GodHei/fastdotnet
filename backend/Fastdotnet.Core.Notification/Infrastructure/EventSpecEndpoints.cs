using System.Text;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;

namespace Fastdotnet.Core.Notification.Infrastructure;

/// <summary>
/// 契约可视化端点注册扩展。
/// 使用前置中间件直接处理 /_events/* 请求。
/// </summary>
public static class EventSpecEndpoints
{
    public static void MapEventSpecEndpoints(this IApplicationBuilder app)
    {
        var generator = app.ApplicationServices.GetRequiredService<AsyncApiSpecGenerator>();

        app.Use(async (context, next) =>
        {
            var path = context.Request.Path.Value?.TrimEnd('/');

            if (string.Equals(path, "/_events/spec", StringComparison.OrdinalIgnoreCase))
            {
                var spec = generator.GenerateSpec();
                context.Response.ContentType = "application/json; charset=utf-8";
                await context.Response.WriteAsync(spec, Encoding.UTF8);
                return;
            }

            if (string.Equals(path, "/_events/ui", StringComparison.OrdinalIgnoreCase))
            {
                var specUrl = $"{context.Request.Scheme}://{context.Request.Host}/_events/spec";
                context.Response.ContentType = "text/html; charset=utf-8";
                await context.Response.WriteAsync(BuildHtmlPage(specUrl), Encoding.UTF8);
                return;
            }

            await next();
        });
    }

    private static string BuildHtmlPage(string specUrl)
    {
        return $@"<!DOCTYPE html>
<html lang=""zh-CN"">
<head>
    <meta charset=""UTF-8"">
    <meta name=""viewport"" content=""width=device-width, initial-scale=1.0"">
    <title>Fastdotnet 事件契约浏览器</title>
    <script src=""https://cdn.jsdelivr.net/npm/@asyncapi/web-component@1.4.0/lib/asyncapi-web-component.js""></script>
    <style>
        * {{ margin: 0; padding: 0; box-sizing: border-box; }}
        body {{ font-family: -apple-system, BlinkMacSystemFont, 'Segoe UI', sans-serif; }}
        .header {{
            background: linear-gradient(135deg, #667eea 0%, #764ba2 100%);
            color: #fff; padding: 24px 32px; display: flex; align-items: center; gap: 16px;
        }}
        .header h1 {{ font-size: 22px; font-weight: 600; }}
        .header a {{ color: #d4c4fb; font-size: 14px; text-decoration: none; }}
        .header a:hover {{ text-decoration: underline; }}
        .stat {{ display: flex; gap: 24px; margin-left: auto; font-size: 13px; opacity: 0.9; }}
        .stat span {{ display: flex; align-items: center; gap: 6px; }}
        asyncapi-component {{ padding: 0; }}
        .footer {{
            text-align: center; padding: 16px; color: #888; font-size: 12px;
            border-top: 1px solid #eee;
        }}
    </style>
</head>
<body>
    <div class=""header"">
        <h1>📨 Fastdotnet 事件契约浏览器</h1>
        <a href=""{specUrl}"" target=""_blank"">下载 AsyncAPI 规范</a>
        <div class=""stat"">
            <span>🔐 HMAC-SHA256 签名验证</span>
            <span>📋 CloudEvents 1.0</span>
            <span>🔄 At-least-once 投递</span>
        </div>
    </div>
    <asyncapi-component
        specUrl=""{specUrl}""
        schemaID=""payload""
        show={{""info"":true,""servers"":true,""operations"":true,""messages"":true,""schemas"":true}}>
    </asyncapi-component>
    <div class=""footer"">
        Powered by Fastdotnet.Core.Notification · AsyncAPI 2.6 · CloudEvents 1.0
    </div>
</body>
</html>";
    }
}
