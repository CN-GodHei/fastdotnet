using Fastdotnet.Core.Middleware;
using Microsoft.AspNetCore.Http;
using System;
using System.Threading.Tasks;

namespace PluginA.Middleware
{
    /// <summary>
    /// An example dynamic middleware from PluginA.
    /// </summary>
    public class PluginAMiddleware : IDynamicMiddleware
    {
        /// <summary>
        /// This method is executed for every HTTP request after the plugin is loaded.
        /// </summary>
        public async Task InvokeAsync(HttpContext context, RequestDelegate next)
        {
            // Print a message to the console to prove this middleware is being executed.
            Console.WriteLine($"[PluginA] Dynamic Middleware handling request: {context.Request.Path}");

            // Call the next middleware in the pipeline.
            await next(context);
        }
    }
}
