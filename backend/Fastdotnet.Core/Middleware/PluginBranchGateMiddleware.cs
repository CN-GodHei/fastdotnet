using Fastdotnet.Core.Plugin;
using Microsoft.AspNetCore.Http;
using System.IO;
using System.Text;

namespace Fastdotnet.Core.Middleware
{
    /// <summary>
    /// 插件分支网关中间件 - 将特定路径的请求转发到插件私有管道
    /// </summary>
    public class PluginBranchGateMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly PluginBranchRegistry _registry;

        public PluginBranchGateMiddleware(RequestDelegate next, PluginBranchRegistry registry)
        {
            _next = next;
            _registry = registry;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            var path = context.Request.Path.Value;

            // 检查当前请求路径是否匹配任何插件注册的前缀
            if (!string.IsNullOrEmpty(path) &&
                _registry.TryGetMatchingBranch(path, out var branchInfo) &&
                branchInfo != null)
            {
                Console.WriteLine($"🔍 [PluginBranchGate] 匹配到插件分支: {path}");
                Console.WriteLine($"   - 原始 Path: {context.Request.Path}");
                Console.WriteLine($"   - 原始 PathBase: {context.Request.PathBase}");

                // 备份原有的服务提供者
                var originalServices = context.RequestServices;

                try
                {
                    // --- 关键点：将当前请求的服务容器切换为插件私有的容器 ---
                    context.RequestServices = branchInfo.PluginServiceProvider;
                    Console.WriteLine($"   - 已切换 RequestServices 到插件容器");

                    // 修正路径（去掉前缀，让插件能识别）
                    var matchedPrefix = _registry.Branches.Keys
                        .FirstOrDefault(prefix => path.ToLower().StartsWith(prefix));

                    if (!string.IsNullOrEmpty(matchedPrefix))
                    {
                        // 设置 PathBase
                        context.Request.PathBase = new PathString(matchedPrefix);

                        // ✅ 尝试1：保留完整路径（不截取）- Elsa 可能期待完整路径 /elsa/api/...
                        context.Request.Path = new PathString(path);

                        /*
                        // ✅ 尝试2：截取前缀（当前禁用）
                        var remainingPath = path.Substring(matchedPrefix.Length);
                        
                        // 如果剩余路径为空，设置为根路径
                        if (string.IsNullOrEmpty(remainingPath))
                        {
                            context.Request.Path = new PathString("/");
                        }
                        else
                        {
                            // 确保剩余路径以 / 开头
                            var normalizedPath = remainingPath.StartsWith("/") ? remainingPath : "/" + remainingPath;
                            context.Request.Path = new PathString(normalizedPath);
                        }
                        */

                        Console.WriteLine($"   - 修正后 Path: {context.Request.Path}");
                        Console.WriteLine($"   - 修正后 PathBase: {context.Request.PathBase}");
                        Console.WriteLine($"   - 原始完整路径: {path}");
                    }

                    Console.WriteLine($"   - 开始执行插件管道...");

                    // 🔍 调试：记录响应状态码
                    var originalBodyStream = context.Response.Body;
                    using var responseBody = new MemoryStream();
                    context.Response.Body = responseBody;

                    // 将请求交给插件管道处理
                    await branchInfo.Delegate(context);

                    // 读取响应状态码
                    var statusCode = context.Response.StatusCode;
                    responseBody.Seek(0, SeekOrigin.Begin);
                    var reader = new StreamReader(responseBody);
                    var responseContent = await reader.ReadToEndAsync();

                    // 恢复原始流
                    responseBody.Seek(0, SeekOrigin.Begin);
                    await responseBody.CopyToAsync(originalBodyStream);
                    context.Response.Body = originalBodyStream;

                    Console.WriteLine($"   ✅ 插件管道执行完成");
                    Console.WriteLine($"   - 响应状态码: {statusCode}");
                    if (statusCode == 404)
                    {
                        Console.WriteLine($"   ⚠️ 警告：返回404，说明 Elsa 内部没有匹配到路由");
                        Console.WriteLine($"   - 响应内容: {responseContent}");
                    }
                }
                catch (Exception ex)
                {
                    // 记录插件管道执行异常
                    Console.WriteLine($"❌ 插件分支管道执行失败 [{path}]: {ex.Message}");
                    Console.WriteLine($"堆栈跟踪: {ex.StackTrace}");

                    // 可以选择返回错误响应或重新抛出异常
                    // 这里选择重新抛出，让全局异常处理器处理
                    throw;
                }
                finally
                {
                    // 还原，防止影响主程序后续逻辑
                    context.RequestServices = originalServices;
                    Console.WriteLine($"   - 已还原 RequestServices");
                }

                return; // 处理完直接返回，不走主程序后续管道
            }

            // 不匹配则继续走主程序逻辑
            //Console.WriteLine($"⚪ [PluginBranchGate] 未匹配插件分支: {path}");
            await _next(context);
        }
    }
}
