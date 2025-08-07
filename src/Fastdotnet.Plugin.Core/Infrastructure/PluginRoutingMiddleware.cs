using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Controllers;
using System.Threading.Tasks;

namespace Fastdotnet.Plugin.Core.Infrastructure
{
    public class PluginRoutingMiddleware
    {
        private readonly RequestDelegate _next;

        public PluginRoutingMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context, PluginManager pluginManager)
        {
            var endpoint = context.GetEndpoint();
            if (endpoint != null)
            {
                var controllerActionDescriptor = endpoint.Metadata.GetMetadata<ControllerActionDescriptor>();
                if (controllerActionDescriptor != null)
                {
                    var controllerType = controllerActionDescriptor.ControllerTypeInfo;
                    
                    // Check if the controller's assembly is from a currently loaded plugin
                    if (!pluginManager.IsPluginLoaded(controllerType.Assembly))
                    {
                        context.Response.StatusCode = StatusCodes.Status404NotFound;
                        return;
                    }
                }
            }

            await _next(context);
        }
    }
}
