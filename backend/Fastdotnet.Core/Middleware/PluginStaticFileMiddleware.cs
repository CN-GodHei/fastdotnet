
namespace Fastdotnet.Core.Middleware
{
    public class PluginStaticFileMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly PluginStaticFileProviderRegistry _registry;
        private readonly IContentTypeProvider _contentTypeProvider;

        public PluginStaticFileMiddleware(RequestDelegate next, PluginStaticFileProviderRegistry registry)
        {
            _next = next;
            _registry = registry;
            _contentTypeProvider = new FileExtensionContentTypeProvider();
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
                    var fileInfo = provider.GetFileInfo(subpath);

                    if (fileInfo.Exists && !fileInfo.IsDirectory)
                    {
                        // 尝试确定内容类型
                        if (!_contentTypeProvider.TryGetContentType(fileInfo.Name, out var contentType))
                        {
                            contentType = "application/octet-stream";
                        }

                        context.Response.ContentType = contentType;

                        // 使用流式传输文件内容
                        await context.Response.SendFileAsync(fileInfo);
                        return; // 文件已处理，终止管道
                    }
                }
            }

            // 如果没有提供者可以处理该文件，则将请求传递给下一个中间件
            await _next(context);
        }
    }
}
