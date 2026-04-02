
namespace Fastdotnet.Core.Middleware
{
    /// <summary>
    /// 定义了一个契约，用于主应用程序动态发现和执行的中间件。
    /// 这允许热插拔的插件参与 HTTP 请求管道。
    /// </summary>
    public interface IDynamicMiddleware
    {
        /// <summary>
        /// 处理 HTTP 请求。
        /// </summary>
        /// <param name="context">当前请求的 HttpContext。</param>
        /// <param name="next">管道中的下一个中间件。</param>
        /// <returns>表示中间件执行完成的任务。</returns>
        Task InvokeAsync(HttpContext context, RequestDelegate next);
    }
}
