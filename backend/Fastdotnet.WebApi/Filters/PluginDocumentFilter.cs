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
            // 文档名称格式: "main" 或 "plugin-插件名"
            var docName = context.DocumentName;
            
            if (docName == "main")
            {
                // 主系统文档 - 移除所有插件API
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
                    }
                }
            }
            else if (docName.StartsWith("plugin-"))
            {
                // 提取插件名称
                var currentPluginId = docName.Substring("plugin-".Length);
                
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
                                var pathPluginId = pathSegments[2].ToLower();
                                if (pathPluginId != currentPluginId)
                                {
                                    pathsToRemove.Add(path.Key);
                                }
                            }
                        }
                        else
                        {
                            // 非插件API也移除
                            pathsToRemove.Add(path.Key);
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
    }
}