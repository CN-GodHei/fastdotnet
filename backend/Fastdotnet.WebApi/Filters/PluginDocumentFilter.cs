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
                            
                            // 如果没有ApiUsageScopeAttribute，则移除该API
                            if (apiScope == null)
                            {
                                pathsToRemove.Add(path.Key);
                            }
                            else if (!ShouldKeepApi(scope, apiScope.Value))
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
                                var pathPluginId = pathSegments[3].ToLower().Substring(1);
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

            // 移除未被任何剩余 API 路径引用的 schema 类型
            RemoveUnusedSchemas(swaggerDoc);
        }

        /// <summary>
        /// 移除 Swagger 文档中未被剩余 API 路径引用的 schema 类型
        /// 避免生成多余的模型类型定义
        /// </summary>
        private void RemoveUnusedSchemas(OpenApiDocument swaggerDoc)
        {
            if (swaggerDoc.Components?.Schemas == null || swaggerDoc.Components.Schemas.Count == 0)
                return;

            // 收集所有被引用到的 schema ID
            var referencedSchemas = new HashSet<string>();

            foreach (var pathItem in swaggerDoc.Paths.Values)
            {
                foreach (var operation in pathItem.Operations.Values)
                {
                    // 检查参数
                    foreach (var parameter in operation.Parameters)
                    {
                        CollectSchemaRefs(parameter.Schema, referencedSchemas);
                    }

                    // 检查请求体
                    if (operation.RequestBody?.Content != null)
                    {
                        foreach (var mediaType in operation.RequestBody.Content.Values)
                        {
                            CollectSchemaRefs(mediaType.Schema, referencedSchemas);
                        }
                    }

                    // 检查响应
                    if (operation.Responses != null)
                    {
                        foreach (var response in operation.Responses.Values)
                        {
                            if (response.Content != null)
                            {
                                foreach (var mediaType in response.Content.Values)
                                {
                                    CollectSchemaRefs(mediaType.Schema, referencedSchemas);
                                }
                            }
                        }
                    }
                }
            }

            // 传递闭包：如果 schema A 引用了 schema B，B 也需要保留
            var processed = new HashSet<string>();
            var queue = new Queue<string>(referencedSchemas);

            while (queue.Count > 0)
            {
                var schemaId = queue.Dequeue();
                if (!processed.Add(schemaId))
                    continue;

                if (swaggerDoc.Components.Schemas.TryGetValue(schemaId, out var schema))
                {
                    CollectSchemaRefs(schema, referencedSchemas);
                }
            }

            // 移除未被引用的 schema
            var schemasToRemove = swaggerDoc.Components.Schemas.Keys
                .Where(id => !referencedSchemas.Contains(id))
                .ToList();

            foreach (var id in schemasToRemove)
            {
                swaggerDoc.Components.Schemas.Remove(id);
            }

            if (schemasToRemove.Count > 0)
            {
                Console.WriteLine($"[Swagger] 文档 '{swaggerDoc.Info?.Title}' 移除了 {schemasToRemove.Count} 个未引用的 schema 类型");
            }
        }

        /// <summary>
        /// 递归收集 schema 中的 $ref 引用
        /// </summary>
        private void CollectSchemaRefs(OpenApiSchema? schema, HashSet<string> refs)
        {
            if (schema == null) return;

            // 检查直接引用
            if (schema.Reference != null && !string.IsNullOrEmpty(schema.Reference.Id))
            {
                refs.Add(schema.Reference.Id);
            }

            // 检查 OneOf
            if (schema.OneOf != null)
            {
                foreach (var sub in schema.OneOf)
                    CollectSchemaRefs(sub, refs);
            }

            // 检查 AnyOf
            if (schema.AnyOf != null)
            {
                foreach (var sub in schema.AnyOf)
                    CollectSchemaRefs(sub, refs);
            }

            // 检查 AllOf
            if (schema.AllOf != null)
            {
                foreach (var sub in schema.AllOf)
                    CollectSchemaRefs(sub, refs);
            }

            // 检查数组类型
            if (schema.Items != null)
            {
                CollectSchemaRefs(schema.Items, refs);
            }

            // 检查 AdditionalProperties
            if (schema.AdditionalProperties != null)
            {
                CollectSchemaRefs(schema.AdditionalProperties, refs);
            }

            // 递归检查属性
            if (schema.Properties != null)
            {
                foreach (var prop in schema.Properties.Values)
                {
                    CollectSchemaRefs(prop, refs);
                }
            }
        }
        
        /// <summary>
        /// 获取API的使用范围
        /// </summary>
        /// <returns>如果找到ApiUsageScopeAttribute则返回对应的枚举值，否则返回null</returns>
        private ApiUsageScopeEnum? GetApiUsageScope(ApiDescription apiDesc)
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
                            var scopeValue = scopeProperty.GetValue(attr);
                            if (scopeValue != null)
                                return (ApiUsageScopeEnum)scopeValue;
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
                                var scopeValue = scopeProperty.GetValue(attr);
                                if (scopeValue != null)
                                    return (ApiUsageScopeEnum)scopeValue;
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
            
            // 没有找到ApiUsageScopeAttribute，返回null
            return null;
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