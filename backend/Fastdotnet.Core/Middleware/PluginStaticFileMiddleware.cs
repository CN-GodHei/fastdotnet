
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
            var bestMatchPrefix = _registry.GetBestMatchRequestPath(path.Value);
            if (bestMatchPrefix != null)
            {
                var provider = _registry.GetProvider(bestMatchPrefix);
                if (provider != null)
                {
                    // 计算子路径
                    var subpath = path.Value.Substring(bestMatchPrefix.Length);
                    
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
            
            await context.Response.SendFileAsync(fileInfo);
        }
    }
}
