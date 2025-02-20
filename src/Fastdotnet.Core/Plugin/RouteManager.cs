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

        private async Task HandleActionResult(object? result, HttpContext context)
        {
            if (result == null)
            {
                return;
            }

            // 处理Task类型的结果
            if (result is Task task)
            {
                await task;
                if (task.GetType().IsGenericType)
                {
                    result = ((dynamic)task).Result;
                }
                else
                {
                    return;
                }
            }

            // 处理IActionResult类型的结果
            if (result is IActionResult actionResult)
            {
                await actionResult.ExecuteResultAsync(new ActionContext
                {
                    HttpContext = context
                });
                return;
            }

            // 处理其他类型的结果（序列化为JSON）
            await context.Response.WriteAsJsonAsync(result);
        }

        public void UnregisterPluginController(Type controllerType, string pluginId)
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
                    var dataSource = _endpointRouteBuilder.DataSources.FirstOrDefault();
                    if (dataSource != null)
                    {
                        var endpoints = dataSource.Endpoints
                            .Where(e => {
                                var routeEndpoint = e as RouteEndpoint;
                                return routeEndpoint?.RoutePattern?.RawText == template;
                            })
                            .ToList();

                        foreach (var endpoint in endpoints)
                        {
                            dataSource.GetType()
                                .GetMethod("Remove", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)?
                                .Invoke(dataSource, new object[] { endpoint });
                        }
                    }
                }
            }
        }
    }
}