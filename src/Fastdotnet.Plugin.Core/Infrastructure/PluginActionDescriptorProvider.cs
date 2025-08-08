using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using System.Linq;
using System.Reflection;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.ApplicationParts; // Add this using directive
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc; // Add this using directive

namespace Fastdotnet.Plugin.Core.Infrastructure
{
    public class PluginActionDescriptorProvider : IActionDescriptorProvider
    {
        private readonly PluginManager _pluginManager;
        private readonly ApplicationPartManager _applicationPartManager; // Add this field

        public PluginActionDescriptorProvider(PluginManager pluginManager, ApplicationPartManager applicationPartManager) // Modify constructor
        {
            _pluginManager = pluginManager;
            _applicationPartManager = applicationPartManager; // Assign it
        }

        public int Order => 999; // 确保在其他默认Provider之后执行

        public void OnProvidersExecuted(ActionDescriptorProviderContext context)
        {
            // 不做任何操作
        }

        public void OnProvidersExecuting(ActionDescriptorProviderContext context)
        {
            foreach (var actionDescriptor in context.Results)
            {
                if (actionDescriptor is ControllerActionDescriptor controllerActionDescriptor)
                {
                    var controllerType = controllerActionDescriptor.ControllerTypeInfo.AsType();

                    if (_pluginManager.IsTypeFromPluginAssembly(controllerType))
                    {
                        var pluginConfig = _pluginManager.GetPluginConfig(controllerType.Assembly);
                        if (pluginConfig != null)
                        {
                            // --- START: 为Swagger分组添加插件名称 ---
                            if (!controllerActionDescriptor.Properties.ContainsKey("PluginName"))
                            {
                                controllerActionDescriptor.Properties["PluginName"] =$"插件【{pluginConfig.name}】" ;
                            }
                            // --- END: 为Swagger分组添加插件名称 ---
                            var pluginId = pluginConfig.id.ToLowerInvariant();

                            if (controllerActionDescriptor.AttributeRouteInfo != null)
                            {
                                string originalRouter = controllerActionDescriptor.AttributeRouteInfo.Template.ToLower();
                                string newTemplate = string.Empty;
                                if (originalRouter.StartsWith("api/"))
                                {
                                    // 提取 api/ 之后的部分（例如：api/values/Get -> values/Get）
                                    string remainingPath = originalRouter.Substring("api/".Length);

                                    // 构造新的路由模板：api/plugins/{pluginCode}/{remainingPath}
                                    newTemplate = $"api/plugins/{pluginId}/{remainingPath}";

                                }
                                else
                                {
                                    newTemplate = $"api/plugins/{pluginId}/{originalRouter}";
                                }
                                // 更新路由模板
                                controllerActionDescriptor.AttributeRouteInfo.Template = newTemplate;
                            }
                            else
                            {
                                controllerActionDescriptor.AttributeRouteInfo = new AttributeRouteInfo
                                {
                                    Template = $"plugins/{pluginId}/{controllerActionDescriptor.ControllerName.ToLowerInvariant()}/{controllerActionDescriptor.ActionName.ToLowerInvariant()}"
                                };
                            }
                        }
                    }
                }
            }
        }
    }
}