
namespace Fastdotnet.Core.Middleware
{
    /// <summary>
    /// Defines a contract for middleware that can be dynamically discovered and executed by the host application.
    /// This allows hot-pluggable plugins to participate in the HTTP request pipeline.
    /// </summary>
    public interface IDynamicMiddleware
    {
        /// <summary>
        /// Handles the HTTP request.
        /// </summary>
        /// <param name="context">The HttpContext for the current request.</param>
        /// <param name="next">The next middleware in the pipeline.</param>
        /// <returns>A Task that represents the completion of the middleware execution.</returns>
        Task InvokeAsync(HttpContext context, RequestDelegate next);
    }
}
