using System.Text;
using Fastdotnet.WebApi.Services.EventBus;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;

namespace Fastdotnet.WebApi.Extensions;

/// <summary>
/// 事件目录可视化端点
/// 提供 AsyncAPI 2.6 规范的 spec 端点和 UI 页面
/// 
/// 与 Fastdotnet.Core.Notification 的 /_events/* 端点独立：
///   /_events/spec + /_events/ui → 框架对外推送 (Webhook/SignalR)
///   /api/eventcatalog/spec + /api/eventcatalog/ui → 插件间通信事件目录
/// </summary>
public static class EventCatalogEndpoints
{
    public static void MapEventCatalogEndpoints(this IApplicationBuilder app)
    {
        var registry = app.ApplicationServices.GetRequiredService<EventRegistry>();

        app.Use(async (context, next) =>
        {
            var path = context.Request.Path.Value?.TrimEnd('/');

            // AsyncAPI 2.6 规范 JSON
            if (string.Equals(path, "/api/eventcatalog/spec", StringComparison.OrdinalIgnoreCase))
            {
                // 每次请求时重新扫描最新的程序集状态（支持热插拔）
                registry.ScanAllAssemblies();

                var spec = registry.GenerateAsyncApiSpec(
                    title: "Fastdotnet 插件事件目录",
                    version: "1.0.0");

                context.Response.ContentType = "application/json; charset=utf-8";
                await context.Response.WriteAsync(spec, Encoding.UTF8);
                return;
            }

            // AsyncAPI UI 可视化页面
            if (string.Equals(path, "/api/eventcatalog/ui", StringComparison.OrdinalIgnoreCase))
            {
                var specUrl = $"{context.Request.Scheme}://{context.Request.Host}/api/eventcatalog/spec";
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
    <title>Fastdotnet 插件事件目录</title>
    <script src=""https://cdn.jsdelivr.net/npm/@asyncapi/web-component@1.4.0/lib/asyncapi-web-component.js""></script>
    <style>
        * {{ margin: 0; padding: 0; box-sizing: border-box; }}
        body {{ font-family: -apple-system, BlinkMacSystemFont, 'Segoe UI', sans-serif; }}
        .header {{
            background: linear-gradient(135deg, #10b981 0%, #059669 100%);
            color: #fff; padding: 24px 32px; display: flex; align-items: center; gap: 16px;
        }}
        .header h1 {{ font-size: 22px; font-weight: 600; }}
        .header a {{ color: #d1fae5; font-size: 14px; text-decoration: none; }}
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
        <h1>📡 插件事件目录</h1>
        <a href=""{specUrl}"" target=""_blank"">下载 AsyncAPI 规范</a>
        <div class=""stat"">
            <span>🔌 跨插件通信</span>
            <span>📋 AsyncAPI 2.6</span>
            <span>🔄 热插拔感知</span>
        </div>
    </div>
    <asyncapi-component
        specUrl=""{specUrl}""
        schemaID=""payload""
        show={{""info"":true,""servers"":true,""operations"":true,""messages"":true,""schemas"":true}}>
    </asyncapi-component>
    <div class=""footer"">
        Powered by Fastdotnet EventBus · AsyncAPI 2.6 · 事件Key格式: {{pluginId}}.{{eventName}}
    </div>
</body>
</html>";
    }
}
