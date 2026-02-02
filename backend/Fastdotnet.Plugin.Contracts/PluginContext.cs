
namespace Fastdotnet.Plugin.Contracts
{
    /// <summary>
    /// 插件上下文，用于插件内部获取自身信息
    /// </summary>
    public static class PluginContext
    {
        /// <summary>
        /// 获取当前插件的信息
        /// </summary>
        /// <returns>当前插件信息</returns>
        public static PluginInfo GetCurrentPluginInfo()
        {
            // 获取调用此方法的插件程序集
            var callingAssembly = Assembly.GetCallingAssembly();
            var assemblyName = callingAssembly.GetName().Name;
            
            // 首先尝试从缓存获取插件信息
            var cachedInfo = PluginInfoCache.GetPluginInfoByAssembly(assemblyName);
            if (cachedInfo != null)
            {
                return cachedInfo;
            }
            
            // 尝试通过插件程序集路径获取插件ID
            var pluginIdFromPath = GetPluginIdFromAssemblyPath(assemblyName, callingAssembly);
            if (!string.IsNullOrEmpty(pluginIdFromPath))
            {
                var pluginInfoFromCache = PluginInfoCache.GetPluginInfo(pluginIdFromPath);
                if (pluginInfoFromCache != null)
                {
                    // 如果通过路径获取到了插件ID，但程序集映射不存在，则添加映射
                    PluginInfoCache.StoreAssemblyMapping(assemblyName, pluginIdFromPath);
                    return pluginInfoFromCache;
                }
            }
            
            // 如果缓存中没有，尝试从程序集属性中获取插件信息
            var pluginInfo = GetPluginInfoFromAssembly(callingAssembly);
            
            // 如果无法从程序集属性获取，则尝试从配置文件加载
            if (pluginInfo == null)
            {
                pluginInfo = GetPluginInfoFromFile();
            }
            
            // 如果成功获取到插件信息，则缓存它
            if (pluginInfo != null)
            {
                PluginInfoCache.StoreAssemblyMapping(assemblyName, pluginInfo.id);
                PluginInfoCache.StorePluginInfo(pluginInfo.id, pluginInfo);
            }
            
            return pluginInfo;
        }

        /// <summary>
        /// 从程序集路径中推断插件ID
        /// </summary>
        /// <param name="assemblyName">程序集名称</param>
        /// <param name="assembly">程序集实例</param>
        /// <returns>插件ID</returns>
        private static string GetPluginIdFromAssemblyPath(string assemblyName, Assembly assembly)
        {
            try
            {
                var assemblyLocation = assembly.Location;
                var directory = Path.GetDirectoryName(assemblyLocation);
                
                // 插件路径通常为 plugins/{pluginId}/...
                var pluginsDir = Path.GetDirectoryName(Path.GetDirectoryName(directory));
                
                // 检查是否在Plugins目录下
                if (Path.GetFileName(pluginsDir).Equals("plugins", StringComparison.OrdinalIgnoreCase))
                {
                    // 返回插件目录名作为插件ID
                    return Path.GetFileName(directory);
                }
                
                // 如果不是标准插件目录结构，尝试从更深层级获取
                var dirInfo = new DirectoryInfo(directory);
                while (dirInfo?.Parent != null)
                {
                    if (dirInfo.Parent.Name.Equals("Plugins", StringComparison.OrdinalIgnoreCase) ||
                        dirInfo.Parent.Name.Equals("plugins", StringComparison.OrdinalIgnoreCase))
                    {
                        return dirInfo.Name;
                    }
                    dirInfo = dirInfo.Parent;
                }
            }
            catch
            {
                // 如果出现异常，静默处理并返回null
            }
            
            return null;
        }

        /// <summary>
        /// 从程序集中获取插件信息
        /// </summary>
        /// <param name="assembly">插件程序集</param>
        /// <returns>插件信息</returns>
        private static PluginInfo GetPluginInfoFromAssembly(Assembly assembly)
        {
            var idAttr = assembly.GetCustomAttribute<AssemblyMetadataAttribute>()?.Value;
            var nameAttr = assembly.GetCustomAttribute<AssemblyTitleAttribute>()?.Title;
            var descAttr = assembly.GetCustomAttribute<AssemblyDescriptionAttribute>()?.Description;
            var versionAttr = assembly.GetCustomAttribute<AssemblyVersionAttribute>()?.Version;
            var companyAttr = assembly.GetCustomAttribute<AssemblyCompanyAttribute>()?.Company;

            // 如果没有从程序集属性获取到足够的信息，则返回null
            if (string.IsNullOrEmpty(idAttr) || string.IsNullOrEmpty(nameAttr))
            {
                return null;
            }

            return new PluginInfo
            {
                id = idAttr,
                name = nameAttr,
                description = descAttr ?? "",
                version = versionAttr ?? "",
                author = companyAttr ?? "",
                enabled = true,
                dependencies = new System.Collections.Generic.List<string>(),
                tags = new System.Collections.Generic.List<string>(),
                entryPoint = assembly.ManifestModule.Name
            };
        }

        /// <summary>
        /// 从插件配置文件获取插件信息
        /// </summary>
        /// <returns>插件信息</returns>
        private static PluginInfo GetPluginInfoFromFile()
        {
            // 获取当前工作目录或程序集所在目录
            var assemblyLocation = Assembly.GetCallingAssembly().Location;
            var assemblyDir = Path.GetDirectoryName(assemblyLocation) ?? "";
            
            // 查找同级目录下的plugin.json文件
            var pluginJsonPath = Path.Combine(assemblyDir, "plugin.json");
            
            if (!File.Exists(pluginJsonPath))
            {
                // 如果在程序集目录下找不到，尝试向上搜索
                var currentDir = new DirectoryInfo(assemblyDir);
                while (currentDir != null)
                {
                    pluginJsonPath = Path.Combine(currentDir.FullName, "plugin.json");
                    if (File.Exists(pluginJsonPath))
                    {
                        break;
                    }
                    
                    // 继续向上搜索一级
                    currentDir = currentDir.Parent;
                    if (currentDir == null || currentDir.Name == "Plugins") // 避免搜索得太远
                    {
                        break;
                    }
                }
                
                // 如果仍未找到，返回null
                if (!File.Exists(pluginJsonPath))
                {
                    return null;
                }
            }
            
            try
            {
                var jsonContent = File.ReadAllText(pluginJsonPath);
                var pluginInfo = JsonSerializer.Deserialize<PluginInfo>(jsonContent, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });
                
                return pluginInfo;
            }
            catch
            {
                return null;
            }
        }
    }
}