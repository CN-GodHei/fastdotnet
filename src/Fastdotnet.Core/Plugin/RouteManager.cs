using System;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Core;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;

namespace Fastdotnet.Core.Plugin
{
    public class RouteManager
    {
        private readonly IEndpointRouteBuilder _endpointRouteBuilder;
        private readonly IServiceProvider _serviceProvider;

        public RouteManager(IEndpointRouteBuilder endpointRouteBuilder, IServiceProvider serviceProvider)
        {
            _endpointRouteBuilder = endpointRouteBuilder;
            _serviceProvider = serviceProvider;
        }

        public void RegisterPluginController(Type controllerType, string pluginId)
        {
            if (controllerType == null)
                throw new ArgumentNullException(nameof(controllerType));

            var controllerName = controllerType.Name.Replace("Controller", "");
            var methods = controllerType.GetMethods(BindingFlags.Public | BindingFlags.Instance)
                .Where(m => m.DeclaringType == controllerType);

            foreach (var method in methods)
            {
                var httpMethodAttribute = method.GetCustomAttributes()
                    .FirstOrDefault(a => a is HttpMethodAttribute) as HttpMethodAttribute;

                if (httpMethodAttribute != null)
                {
                    var template = $"/api/plugins/{pluginId}/{controllerName}/{method.Name}";
                    var httpMethod = httpMethodAttribute.HttpMethods.First();

                    _endpointRouteBuilder.MapMethods(
                        template,
                        new[] { httpMethod },
                        async context =>
                        {
                            using var scope = context.RequestServices.CreateScope();
                            var controller = ActivatorUtilities.CreateInstance(scope.ServiceProvider, controllerType) as ControllerBase;
                            if (controller != null)
                            {
                                controller.ControllerContext = new ControllerContext
                                {
                                    HttpContext = context
                                };

                                var result = method.Invoke(controller, Array.Empty<object>());
                                await HandleActionResult(result, context);
                            }
                        });
                }
            }
        }

        private static async Task HandleActionResult(object result, HttpContext context)
        {
            var actionContext = new ActionContext
            {
                HttpContext = context,
                RouteData = context.GetRouteData(),
                ActionDescriptor = new Microsoft.AspNetCore.Mvc.Controllers.ControllerActionDescriptor()
            };

            if (result is Task<IActionResult> taskResult)
            {
                var actionResult = await taskResult;
                await actionResult.ExecuteResultAsync(actionContext);
            }
            else if (result is IActionResult actionResult)
            {
                await actionResult.ExecuteResultAsync(actionContext);
            }
            else if (result != null)
            {
                await new JsonResult(result).ExecuteResultAsync(actionContext);
            }
        }
    }
}