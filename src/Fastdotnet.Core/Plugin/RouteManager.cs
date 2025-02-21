using System;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ApplicationParts;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.Core;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;

namespace Fastdotnet.Core.Plugin
{
    public class RouteManager
    {
        private readonly IEndpointRouteBuilder _endpointRouteBuilder;
        private readonly IServiceProvider _serviceProvider;
        private readonly ApplicationPartManager _partManager;
        private readonly Dictionary<string, List<IEndpointConventionBuilder>> _pluginEndpoints;
        private readonly Dictionary<string, AssemblyPart> _pluginParts;

        private string GetRouteKey(string pluginId, string template)
        {
            return $"{pluginId}:{template}";
        }

        public RouteManager(IEndpointRouteBuilder endpointRouteBuilder, IServiceProvider serviceProvider)
        {
            _endpointRouteBuilder = endpointRouteBuilder;
            _serviceProvider = serviceProvider;
            _pluginEndpoints = new Dictionary<string, List<IEndpointConventionBuilder>>();
            _pluginParts = new Dictionary<string, AssemblyPart>();
            _partManager = serviceProvider.GetRequiredService<ApplicationPartManager>();
        }

        public void RegisterPluginController(Type controllerType, string pluginId)
        {
            if (controllerType == null)
                throw new ArgumentNullException(nameof(controllerType));

            var assembly = controllerType.Assembly;
            var assemblyPart = new AssemblyPart(assembly);
            
            // 添加到ApplicationPartManager
            if (!_pluginParts.ContainsKey(pluginId))
            {
                _partManager.ApplicationParts.Add(assemblyPart);
                _pluginParts.Add(pluginId, assemblyPart);

                // 重新加载控制器特性提供程序
                var feature = new ControllerFeature();
                _partManager.PopulateFeature(feature);
            }

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

                    var endpoint = _endpointRouteBuilder.MapMethods(
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

                    var routeKey = GetRouteKey(pluginId, template);
                    if (!_pluginEndpoints.ContainsKey(routeKey))
                    {
                        _pluginEndpoints[routeKey] = new List<IEndpointConventionBuilder>();
                    }
                    _pluginEndpoints[routeKey].Add(endpoint);
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
            if (_pluginParts.TryGetValue(pluginId, out var assemblyPart))
            {
                // 从ApplicationPartManager中移除插件程序集
                _partManager.ApplicationParts.Remove(assemblyPart);
                _pluginParts.Remove(pluginId);

                // 重新加载控制器特性提供程序
                var feature = new ControllerFeature();
                _partManager.PopulateFeature(feature);

                // 从现有的DataSources中移除与插件相关的端点
                var newDataSources = new List<EndpointDataSource>();
                foreach (var dataSource in _endpointRouteBuilder.DataSources)
                {
                    var endpoints = dataSource.Endpoints
                        .Where(e => !(e is RouteEndpoint routeEndpoint && 
                                    routeEndpoint.RoutePattern.RawText.Contains($"/api/plugins/{pluginId}/")))
                        .ToList();

                    if (endpoints.Count > 0)
                    {
                        newDataSources.Add(new DefaultEndpointDataSource(endpoints));
                    }
                }

                // 清除并重新添加过滤后的DataSources
                _endpointRouteBuilder.DataSources.Clear();
                foreach (var dataSource in newDataSources)
                {
                    _endpointRouteBuilder.DataSources.Add(dataSource);
                }

                // 从插件端点字典中移除相关记录
                var keysToRemove = _pluginEndpoints.Keys
                    .Where(key => key.StartsWith($"{pluginId}:"))
                    .ToList();

                foreach (var key in keysToRemove)
                {
                    _pluginEndpoints.Remove(key);
                }
            }
        }
    }
}