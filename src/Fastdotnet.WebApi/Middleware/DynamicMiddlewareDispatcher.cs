using Fastdotnet.Core.Middleware;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Fastdotnet.WebApi.Middleware
{
    /// <summary>
    /// A dispatcher middleware that executes dynamic middlewares from a central registry.
    /// This enables hot-pluggable plugins to participate in the HTTP request pipeline.
    /// </summary>
    public class DynamicMiddlewareDispatcher
    {
        private readonly RequestDelegate _next;
        private readonly DynamicMiddlewareRegistry _registry;

        public DynamicMiddlewareDispatcher(RequestDelegate next, DynamicMiddlewareRegistry registry)
        {
            _next = next;
            _registry = registry;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            var middlewareTypes = _registry.GetMiddlewareTypes().ToList();

            if (middlewareTypes.Any())
            {
                // Build and execute a temporary pipeline from the registered middleware types.
                RequestDelegate pipeline = BuildPipeline(middlewareTypes, context, _next);
                await pipeline(context);
            }
            else
            {
                // If no dynamic middlewares are registered, just call the next middleware in the main pipeline.
                await _next(context);
            }
        }

        private static RequestDelegate BuildPipeline(IReadOnlyList<Type> middlewareTypes, HttpContext httpContext, RequestDelegate final)
        {
            RequestDelegate pipeline = final;
            for (int i = middlewareTypes.Count - 1; i >= 0; i--)
            {
                var middlewareType = middlewareTypes[i];
                var next = pipeline;
                pipeline = async context =>
                {
                    // ActivatorUtilities.CreateInstance creates an instance of the middleware,
                    // injecting services from the current request's service provider.
                    var middleware = (IDynamicMiddleware)ActivatorUtilities.CreateInstance(context.RequestServices, middlewareType);
                    await middleware.InvokeAsync(context, next);
                };
            }
            return pipeline;
        }
    }
}
