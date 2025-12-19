using Fastdotnet.Core.Enum;
using Microsoft.AspNetCore.Mvc.ApiExplorer;

namespace Fastdotnet.WebApi.Filters
{
    /// <summary>
    /// 根据插件信息过滤Swagger文档内容的过滤器
    /// </summary>
    public class PluginDocumentFilter : IDocumentFilter
    {
        public void Apply(OpenApiDocument swaggerDoc, DocumentFilterContext context)
        {
            var pathsToRemove = new List<string>();

            // 从文档名称判断是主系统文档还是插件文档
            // 文档名称格式: "main-admin"/"main-app" 或 "plugin-插件名-admin"/"plugin-插件名-app"
            var docName = context.DocumentName;

            if (docName == "main-admin" || docName == "main-app")
            {
                // 主系统文档 - 移除所有插件API，并根据作用域过滤主系统API
                var scope = docName.Split('-')[1]; // "admin" 或 "app"
                
                foreach (var path in swaggerDoc.Paths)
                {
                    // 查找对应的API描述
                    var apiDesc = context.ApiDescriptions.FirstOrDefault(d => $"/{d.RelativePath}" == path.Key);
                    if (apiDesc != null)
                    {
                        // 检查路径是否包含插件路由前缀
                        if (apiDesc.RelativePath != null &&
                            apiDesc.RelativePath.StartsWith("api/plugins/", StringComparison.OrdinalIgnoreCase))
                        {
                            pathsToRemove.Add(path.Key);
                        }
                        else
                        {
                            // 检查主系统API的作用域
                            var apiScope = GetApiUsageScope(apiDesc);
                            if (!ShouldKeepApi(scope, apiScope))
                            {
                                pathsToRemove.Add(path.Key);
                            }
                        }
                    }
                }
            }
            else if (docName.StartsWith("plugin-"))
            {
                // 提取插件名称和作用域
                var nameParts = docName.Split('-');
                var scope = nameParts[nameParts.Length - 1]; // "admin" 或 "app"
                var currentPluginId = string.Join("-", nameParts.Skip(1).Take(nameParts.Length - 2)); // 插件ID
                                                                                                      // 提取插件名称
                                                                                                      //var currentPluginId = docName.Substring("plugin-".Length);

                // 插件文档 - 只保留当前插件的API
                foreach (var path in swaggerDoc.Paths)
                {
                    var apiDesc = context.ApiDescriptions.FirstOrDefault(d => $"/{d.RelativePath}" == path.Key);
                    if (apiDesc != null)
                    {
                        // 检查是否为插件API
                        if (apiDesc.RelativePath != null &&
                            apiDesc.RelativePath.StartsWith("api/plugins/", StringComparison.OrdinalIgnoreCase))
                        {
                            // 提取路径中的插件ID: api/plugins/{pluginId}/...
                            var pathSegments = apiDesc.RelativePath.Split('/');
                            if (pathSegments.Length > 2 && pathSegments[1] == "plugins")
                            {
                                var pathPluginId = pathSegments[3].ToLower();
                                if (pathPluginId != currentPluginId)
                                {
                                    pathsToRemove.Add(path.Key);
                                }
                                string routerscope = pathSegments[2].ToLower();
                                if (routerscope != scope && routerscope != "shared")
                                {
                                    pathsToRemove.Add(path.Key);
                                }
                            }
                        }
                        else
                        {
                            if (apiDesc.RelativePath != null &&
                            apiDesc.RelativePath.StartsWith("api/auth/", StringComparison.OrdinalIgnoreCase))
                            {

                            }
                            else
                            {
                                // 非插件API也移除
                                pathsToRemove.Add(path.Key);
                            }
                        }
                    }
                }
            }

            // 执行移除操作
            foreach (var path in pathsToRemove)
            {
                swaggerDoc.Paths.Remove(path);
            }
        }
        
        /// <summary>
        /// 获取API的使用范围
        /// </summary>
        private ApiUsageScopeEnum GetApiUsageScope(ApiDescription apiDesc)
        {
            // 尝试从控制器或动作方法上的特性获取作用域
            if (apiDesc.ActionDescriptor is ControllerActionDescriptor controllerActionDescriptor)
            {
                // 检查动作方法上的特性
                var actionAttributes = controllerActionDescriptor.MethodInfo.GetCustomAttributes(true);
                foreach (var attr in actionAttributes)
                {
                    if (attr.GetType().Name == "ApiUsageScopeAttribute")
                    {
                        var scopeProperty = attr.GetType().GetProperty("Scope");
                        if (scopeProperty != null)
                        {
                            return (ApiUsageScopeEnum)scopeProperty.GetValue(attr);
                        }
                    }
                }
                
                // 检查控制器上的特性（包括继承自基类的特性）
                var controllerTypeInfo = controllerActionDescriptor.ControllerTypeInfo;
                
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
                                return (ApiUsageScopeEnum)scopeProperty.GetValue(attr);
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
            }
            
            // 默认返回Both，表示两端通用
            return ApiUsageScopeEnum.Both;
        }
        
        /// <summary>
        /// 判断是否应该保留API
        /// </summary>
        private bool ShouldKeepApi(string docScope, ApiUsageScopeEnum apiScope)
        {
            // 如果API标记为两端通用，则保留
            if (apiScope == ApiUsageScopeEnum.Both)
                return true;
                
            // 根据文档作用域和API作用域判断是否保留
            if (docScope == "admin")
            {
                return apiScope == ApiUsageScopeEnum.AdminOnly || apiScope == ApiUsageScopeEnum.Both;
            }
            else if (docScope == "app")
            {
                return apiScope == ApiUsageScopeEnum.AppOnly || apiScope == ApiUsageScopeEnum.Both;
            }
            
            // 默认保留
            return true;
        }
    }
}