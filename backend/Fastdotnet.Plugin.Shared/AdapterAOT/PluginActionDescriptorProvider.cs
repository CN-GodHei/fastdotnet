
namespace Fastdotnet.Plugin.Shared.AdapterAOT
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
                    PluginInfo pluginConfig = null;
                    foreach (var config in _pluginManager.GetLoadedPluginInfos())
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
                        //if (!controllerActionDescriptor.Properties.ContainsKey("PluginName"))
                        //{
                        //    controllerActionDescriptor.Properties["PluginName"] = $"插件【{pluginConfig.name}】";
                        //}

                        var pluginId = pluginConfig.id.ToLowerInvariant();
                        string newTemplate;

                        if (controllerActionDescriptor.AttributeRouteInfo != null)
                        {
                            var originalTemplate = controllerActionDescriptor.AttributeRouteInfo.Template ?? string.Empty;

                            var pathSegment = originalTemplate.StartsWith("api/", StringComparison.OrdinalIgnoreCase)
                                ? originalTemplate.Substring("api/".Length)
                                : originalTemplate;

                            // 根据方法或控制器上的ApiUsageScope特性决定路由前缀
                            var apiScope = GetApiUsageScope(controllerActionDescriptor.MethodInfo, controllerType);
                            var scopePrefix = GetScopePrefix(apiScope);

                            newTemplate = $"api/plugins/{scopePrefix}{"p"}{pluginId}/{pathSegment.TrimStart('/')}";
                            controllerActionDescriptor.AttributeRouteInfo.Template = newTemplate;
                        }
                        else
                        {
                            var controllerName = controllerActionDescriptor.ControllerName;
                            var actionName = controllerActionDescriptor.ActionName;
                            
                            // 根据方法或控制器上的ApiUsageScope特性决定路由前缀
                            var apiScope = GetApiUsageScope(controllerActionDescriptor.MethodInfo, controllerType);
                            var scopePrefix = GetScopePrefix(apiScope);
                            
                            newTemplate = $"api/plugins/{scopePrefix}{"p"}{pluginId}/{controllerName}/{actionName}";

                            controllerActionDescriptor.AttributeRouteInfo = new AttributeRouteInfo
                            {
                                Template = newTemplate
                            };
                        }
                    }
                }
            }
        }
        
        /// <summary>
        /// 获取API的使用范围（方法级别优先于控制器级别）
        /// </summary>
        private Core.Enum.ApiUsageScopeEnum GetApiUsageScope(MethodInfo methodInfo, Type controllerType)
        {
            // 首先检查方法上的ApiUsageScope特性
            var methodAttributes = methodInfo.GetCustomAttributes(true);
            foreach (var attr in methodAttributes)
            {
                if (attr.GetType().Name == "ApiUsageScopeAttribute")
                {
                    var scopeProperty = attr.GetType().GetProperty("Scope");
                    if (scopeProperty != null)
                    {
                        return (Core.Enum.ApiUsageScopeEnum)scopeProperty.GetValue(attr);
                    }
                }
            }
            
            // 如果方法上没有，则检查控制器上的ApiUsageScope特性（包括继承自基类的特性）
            var controllerTypeInfo = IntrospectionExtensions.GetTypeInfo(controllerType);
            
            // 遍历控制器类型及其基类查找ApiUsageScope特性
            while (controllerTypeInfo != null)
            {
                var controllerAttributes = controllerTypeInfo.GetCustomAttributes(true);
                foreach (var attr in controllerAttributes)
                {
                    if (attr.GetType().Name == "ApiUsageScopeAttribute")
                    {
                        var scopeProperty = attr.GetType().GetProperty("Scope");
                        if (scopeProperty != null)
                        {
                            return (Core.Enum.ApiUsageScopeEnum)scopeProperty.GetValue(attr);
                        }
                    }
                }
                
                // 继续检查基类，确保不传递null给GetTypeInfo方法
                if (controllerTypeInfo.BaseType != null)
                {
                    controllerTypeInfo = IntrospectionExtensions.GetTypeInfo(controllerTypeInfo.BaseType);
                }
                else
                {
                    controllerTypeInfo = null;
                }
            }
            
            // 默认返回Both，表示两端通用
            return Core.Enum.ApiUsageScopeEnum.Both;
        }
        
        /// <summary>
        /// 根据API使用范围获取路由前缀
        /// </summary>
        private string GetScopePrefix(Core.Enum.ApiUsageScopeEnum apiScope)
        {
            return apiScope switch
            {
                Core.Enum.ApiUsageScopeEnum.AdminOnly => "admin/",
                Core.Enum.ApiUsageScopeEnum.AppOnly => "app/",
                Core.Enum.ApiUsageScopeEnum.Both => "shared/",
                _ => ""
            };
        }
    }
}