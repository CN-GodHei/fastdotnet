using Fastdotnet.Plugin.Contracts;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.ApplicationParts;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.Routing;
using System;
using System.Linq;

namespace Fastdotnet.Plugin.Core.Infrastructure
{
    public class PluginActionDescriptorProvider : IActionDescriptorProvider
    {
        private readonly PluginManager _pluginManager;
        private readonly ApplicationPartManager _applicationPartManager;

        public PluginActionDescriptorProvider(PluginManager pluginManager, ApplicationPartManager applicationPartManager)
        {
            _pluginManager = pluginManager;
            _applicationPartManager = applicationPartManager;
        }

        public int Order => 999;

        public void OnProvidersExecuted(ActionDescriptorProviderContext context)
        {
        }

        public void OnProvidersExecuting(ActionDescriptorProviderContext context)
        {
            foreach (var actionDescriptor in context.Results)
            {
                if (actionDescriptor is not ControllerActionDescriptor controllerActionDescriptor)
                {
                    continue;
                }

                var controllerType = controllerActionDescriptor.ControllerTypeInfo.AsType();

                if (_pluginManager.IsTypeFromPluginAssembly(controllerType))
                {
                    PluginConfig pluginConfig = null;
                    foreach (var config in _pluginManager.GetLoadedPluginConfigs())
                    {
                        var assembly = _pluginManager.GetPluginAssembly(config.id);
                        if (assembly == controllerType.Assembly)
                        {
                            pluginConfig = config;
                            break;
                        }
                    }

                    if (pluginConfig != null)
                    {
                        if (!controllerActionDescriptor.Properties.ContainsKey("PluginName"))
                        {
                            controllerActionDescriptor.Properties["PluginName"] = $"插件【{pluginConfig.name}】";
                        }

                        var pluginId = pluginConfig.id.ToLowerInvariant();
                        string newTemplate;

                        if (controllerActionDescriptor.AttributeRouteInfo != null)
                        {
                            var originalTemplate = controllerActionDescriptor.AttributeRouteInfo.Template ?? string.Empty;

                            var pathSegment = originalTemplate.StartsWith("api/", StringComparison.OrdinalIgnoreCase)
                                ? originalTemplate.Substring("api/".Length)
                                : originalTemplate;

                            newTemplate = $"api/plugins/{pluginId}/{pathSegment.TrimStart('/')}";
                            controllerActionDescriptor.AttributeRouteInfo.Template = newTemplate;
                        }
                        else
                        {
                            var controllerName = controllerActionDescriptor.ControllerName;
                            var actionName = controllerActionDescriptor.ActionName;
                            newTemplate = $"api/plugins/{pluginId}/{controllerName}/{actionName}";
                            
                            controllerActionDescriptor.AttributeRouteInfo = new AttributeRouteInfo
                            {
                                Template = newTemplate
                            };
                        }
                    }
                }
            }
        }
    }
}