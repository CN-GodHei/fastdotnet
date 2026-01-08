using System.Collections.Concurrent;

namespace Fastdotnet.Plugin.Contracts
{
    /// <summary>
    /// 插件信息缓存，用于在插件加载时存储插件信息并在插件内部访问
    /// </summary>
    public static class PluginInfoCache
    {
        // 存储插件ID到插件信息的映射
        private static readonly ConcurrentDictionary<string, PluginInfo> _pluginInfoMap = new();
        
        // 存储程序集名称到插件ID的映射
        private static readonly ConcurrentDictionary<string, string> _assemblyToPluginIdMap = new();

        /// <summary>
        /// 存储插件信息
        /// </summary>
        /// <param name="pluginId">插件ID</param>
        /// <param name="pluginInfo">插件信息</param>
        public static void StorePluginInfo(string pluginId, PluginInfo pluginInfo)
        {
            _pluginInfoMap[pluginId] = pluginInfo;
        }

        /// <summary>
        /// 存储插件程序集与插件ID的映射关系
        /// </summary>
        /// <param name="assemblyName">程序集名称</param>
        /// <param name="pluginId">插件ID</param>
        public static void StoreAssemblyMapping(string assemblyName, string pluginId)
        {
            if (string.IsNullOrEmpty(pluginId))
            {
                // 如果插件ID为空，则移除映射
                _assemblyToPluginIdMap.TryRemove(assemblyName, out _);
            }
            else
            {
                // 否则添加或更新映射
                _assemblyToPluginIdMap[assemblyName] = pluginId;
            }
        }

        /// <summary>
        /// 获取插件信息
        /// </summary>
        /// <param name="pluginId">插件ID</param>
        /// <returns>插件信息</returns>
        public static PluginInfo GetPluginInfo(string pluginId)
        {
            _pluginInfoMap.TryGetValue(pluginId, out var pluginInfo);
            return pluginInfo;
        }

        /// <summary>
        /// 通过程序集名称获取插件信息
        /// </summary>
        /// <param name="assemblyName">程序集名称</param>
        /// <returns>插件信息</returns>
        public static PluginInfo GetPluginInfoByAssembly(string assemblyName)
        {
            if (_assemblyToPluginIdMap.TryGetValue(assemblyName, out var pluginId))
            {
                return GetPluginInfo(pluginId);
            }
            return null;
        }

        /// <summary>
        /// 通过程序集名称获取插件ID
        /// </summary>
        /// <param name="assemblyName">程序集名称</param>
        /// <returns>插件ID</returns>
        public static string GetPluginIdByAssembly(string assemblyName)
        {
            _assemblyToPluginIdMap.TryGetValue(assemblyName, out var pluginId);
            return pluginId;
        }

        /// <summary>
        /// 清除指定插件的缓存
        /// </summary>
        /// <param name="pluginId">插件ID</param>
        public static void RemovePluginInfo(string pluginId)
        {
            _pluginInfoMap.TryRemove(pluginId, out _);
        }

        /// <summary>
        /// 清除所有缓存
        /// </summary>
        public static void Clear()
        {
            _pluginInfoMap.Clear();
            _assemblyToPluginIdMap.Clear();
        }
    }
}