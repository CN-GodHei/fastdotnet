
namespace Fastdotnet.Core.Middleware
{
    public class PluginStaticFileMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly PluginStaticFileProviderRegistry _registry;

        public PluginStaticFileMiddleware(RequestDelegate next, PluginStaticFileProviderRegistry registry)
        {
            _next = next;
            _registry = registry;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            var path = context.Request.Path;

            // 检查是否有任何已注册的提供者可以处理此路径
            var bestMatchPrefix = _registry.GetBestMatchRequestPath(path.Value!);
            if (bestMatchPrefix != null)
            {
                var provider = _registry.GetProvider(bestMatchPrefix);
                if (provider != null)
                {
                    // 【跨源必需】插件前端资源天生是「跨源消费」的：
                    // 宿主页面跑在 18888/18887，而插件资源由后端 18889 提供（生产环境也可能是不同域名/CDN）。
                    // qiankun 3.0 会用 fetch 取 index.html、用 ESM import 取各 chunk 和 CSS，
                    // 没有 Access-Control-Allow-Origin 浏览器会直接拦掉，表现为
                    // "blocked by CORS policy: No 'Access-Control-Allow-Origin' header"。
                    //
                    // 这里刻意不复用 Program.cs 里那个带 AllowCredentials 的全局策略，原因有两点：
                    //   1. 那个策略目前只在 Development 下注册，生产环境根本没有 CORS；
                    //   2. 静态插件资源是公开文件、不需要凭证，回显 Origin 即可，不必支持 credentials。
                    ApplyCorsHeaders(context);

                    // 预检请求只需应答策略，不需要返回文件内容
                    if (HttpMethods.IsOptions(context.Request.Method))
                    {
                        ApplyPreflightHeaders(context);
                        context.Response.StatusCode = StatusCodes.Status204NoContent;
                        return;
                    }

                    // 计算子路径
                    var subpath = path.Value!.Substring(bestMatchPrefix.Length);
                    
                    // 如果子路径为空或是目录，尝试返回 index.html
                    if (string.IsNullOrEmpty(subpath) || subpath == "/")
                    {
                        subpath = "/index.html";
                    }
                    
                    try
                    {
                        var fileInfo = provider.GetFileInfo(subpath);

                        if (fileInfo.Exists && !fileInfo.IsDirectory)
                        {
                            // 文件存在，直接返回
                            await ServeFile(context, fileInfo);
                            return;
                        }
                        else if (_registry.IsSpaFallbackEnabled(bestMatchPrefix))
                        {
                            // 启用 SPA 回退，尝试返回 index.html
                            var indexFile = provider.GetFileInfo("/index.html");
                            if (indexFile.Exists)
                            {
                                await ServeFile(context, indexFile);
                                return;
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        // 记录错误但不中断请求，传递给下一个中间件
                        Console.WriteLine($"[PluginStaticFileMiddleware] Error serving file {subpath}: {ex.Message}");
                    }
                }
            }

            // 如果没有提供者可以处理该文件，则将请求传递给下一个中间件
            await _next(context);
        }

        /// <summary>
        /// 给插件静态资源加 CORS 响应头。
        /// 有 Origin 就回显（同时写 Vary，避免被中间缓存串味），没有 Origin 用 *。
        /// </summary>
        private static void ApplyCorsHeaders(HttpContext context)
        {
            var origin = context.Request.Headers.Origin.ToString();
            var headers = context.Response.Headers;

            headers["Access-Control-Allow-Origin"] = string.IsNullOrEmpty(origin) ? "*" : origin;
            headers.Append("Vary", "Origin");
            // 允许跨源加载（图片/CSS 等 no-cors 场景也不被 CORP 拦）
            headers["Cross-Origin-Resource-Policy"] = "cross-origin";
        }

        /// <summary>
        /// OPTIONS 预检应答：声明允许的方法与请求头。
        /// </summary>
        private static void ApplyPreflightHeaders(HttpContext context)
        {
            var headers = context.Response.Headers;
            headers["Access-Control-Allow-Methods"] = "GET, HEAD, OPTIONS";

            var requested = context.Request.Headers["Access-Control-Request-Headers"].ToString();
            headers["Access-Control-Allow-Headers"] = string.IsNullOrEmpty(requested) ? "*" : requested;
            headers["Access-Control-Max-Age"] = "600";
        }

        /// <summary>
        /// 提供静态文件
        /// </summary>
        private static async Task ServeFile(HttpContext context, IFileInfo fileInfo)
        {
            // 尝试确定内容类型
            var contentTypeProvider = new FileExtensionContentTypeProvider();
            if (!contentTypeProvider.TryGetContentType(fileInfo.Name, out var contentType))
            {
                contentType = "application/octet-stream";
            }

            context.Response.ContentType = contentType;
            
            // 允许 iframe 嵌入（关键！）
            context.Response.Headers["X-Frame-Options"] = "ALLOWALL";
            context.Response.Headers["Content-Security-Policy"] = "frame-ancestors *";
            
            await context.Response.SendFileAsync(fileInfo);
        }
    }
}
