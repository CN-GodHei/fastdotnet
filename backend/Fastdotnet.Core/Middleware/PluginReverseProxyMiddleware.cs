using System;
using System.Diagnostics;
using System.Net.Http;
using System.Threading.Tasks;
using Fastdotnet.Core.Plugin;
using Microsoft.AspNetCore.Http;
using Yarp.ReverseProxy.Forwarder;

namespace Fastdotnet.Core.Middleware
{
    /// <summary>
    /// 插件反向代理中间件。
    /// 拦截发送到主程序的请求，如果命中 PluginReverseProxyRegistry 中的路由，
    /// 则通过 Yarp 的 IHttpForwarder 转发至底层的微型主机。
    /// </summary>
    public class PluginReverseProxyMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly PluginReverseProxyRegistry _registry;
        private readonly IHttpForwarder _forwarder;
        private readonly HttpMessageInvoker _httpClient;

        public PluginReverseProxyMiddleware(
            RequestDelegate next,
            PluginReverseProxyRegistry registry,
            IHttpForwarder forwarder)
        {
            _next = next;
            _registry = registry;
            _forwarder = forwarder;

            // 配置用于反代转发的高性能 HttpMessageInvoker
            _httpClient = new HttpMessageInvoker(new SocketsHttpHandler()
            {
                UseProxy = false,
                AllowAutoRedirect = false,
                AutomaticDecompression = System.Net.DecompressionMethods.None,
                UseCookies = false,
                // 根据实际本地服务环境调整超时和连接配置
                ActivityHeadersPropagator = DistributedContextPropagator.CreateDefaultPropagator()
            });
        }

        public async Task InvokeAsync(HttpContext context)
        {
            // 检查当前请求路径是否被注册到逃生舱
            if (_registry.TryGetMatch(context.Request.Path, out var matchedPrefix, out var targetUrl))
            {
                // 构建转发选项
                var requestConfig = new ForwarderRequestConfig
                {
                    ActivityTimeout = TimeSpan.FromSeconds(100)
                };

                // === 关键修复：截取掉匹配的前缀 ===
                // 因为目标微型主机并没有配置 /elsa 的 PathBase，它的路由都是基于根路径 / 的。
                // 所以我们需要把发往 /elsa/api/... 的请求，改写成 /api/... 再交给 Yarp 代理。
                var originalPath = context.Request.Path;
                var pathStr = originalPath.Value ?? "";
                if (pathStr.StartsWith(matchedPrefix!, StringComparison.OrdinalIgnoreCase))
                {
                    context.Items["PluginMatchedPrefix"] = matchedPrefix;
                    var remainingPath = pathStr.Substring(matchedPrefix!.Length);
                    // 如果只匹配到 "/elsa"，剩余的是空，需补全为 "/"
                    if (string.IsNullOrEmpty(remainingPath)) remainingPath = "/";
                    // 确保目标路径格式规范（例如不能少掉前面的斜杠）
                    if (!remainingPath.StartsWith("/")) remainingPath = "/" + remainingPath;
                    
                    context.Request.Path = remainingPath;
                }

                try
                {
                    var error = await _forwarder.SendAsync(
                        context,
                        targetUrl!,
                        _httpClient,
                        requestConfig,
                        new ForwarderTransformer() // 使用默认的 Transformer（此时它会读取改写后的 Path）
                    );

                    // 如果转发成功，Yarp 已经接管了 Response 写入
                    if (error != ForwarderError.None)
                    {
                        var errorFeature = context.GetForwarderErrorFeature();
                        Console.WriteLine($"[PluginProxy] 转发失败: {error}, 异常: {errorFeature?.Exception?.Message}");
                        context.Response.StatusCode = 502;
                    }
                }
                finally
                {
                    // 恢复原本的 Path，以免影响其他依赖原上下文组件的异常处理
                    context.Request.Path = originalPath;
                }

                return; // 请求已处理完毕
            }

            // 没有命中反代路由，继续正常的请求管线
            await _next(context);
        }

        /// <summary>
        /// 默认的转置器：配置头部等等
        /// </summary>
        private class ForwarderTransformer : HttpTransformer
        {
            public override async ValueTask TransformRequestAsync( 
                HttpContext httpContext, 
                HttpRequestMessage proxyRequest, 
                string destinationPrefix, 
                System.Threading.CancellationToken cancellationToken)
            {
                // 首先调用基类默认的转换（它会复制所有的基础头部并拼接 Path/Query）
                await base.TransformRequestAsync(httpContext, proxyRequest, destinationPrefix, cancellationToken);
                
                // 将被代理的主机头设置成原始头，让后端插件认为它是主程序
                proxyRequest.Headers.Host = httpContext.Request.Host.Value;
                
                // 增加标准的代理转发头部
                proxyRequest.Headers.TryAddWithoutValidation("X-Forwarded-Proto", httpContext.Request.Scheme);
                proxyRequest.Headers.TryAddWithoutValidation("X-Forwarded-Host", httpContext.Request.Host.Value);
                
                // 传入已被截取的前缀，以便内部应用（如 Elsa）如果支持，能自动拼接 BasePath
                var matchedPrefix = httpContext.Items["PluginMatchedPrefix"] as string;
                if (!string.IsNullOrEmpty(matchedPrefix))
                {
                    proxyRequest.Headers.TryAddWithoutValidation("X-Forwarded-Prefix", matchedPrefix);
                }
            }
        }
    }
}
