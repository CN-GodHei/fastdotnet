using System.Collections.Concurrent;
using Microsoft.AspNetCore.Http;

namespace Fastdotnet.Core.Plugin
{
    /// <summary>
    /// 插件分支信息 - 包含委托和服务提供者
    /// </summary>
    public class PluginBranchInfo
    {
        public RequestDelegate Delegate { get; set; }
        public IServiceProvider PluginServiceProvider { get; set; }
    }

    /// <summary>
    /// 插件分支注册表 - 存储插件注册的请求处理委托
    /// </summary>
    public class PluginBranchRegistry
    {
        // Key 是路径前缀（如 /elsa），Value 是插件分支信息
        private readonly ConcurrentDictionary<string, PluginBranchInfo> _branches = new(StringComparer.OrdinalIgnoreCase);

        /// <summary>
        /// 获取所有已注册的分支
        /// </summary>
        public IReadOnlyDictionary<string, PluginBranchInfo> Branches => _branches;

        /// <summary>
        /// 注册一个插件分支
        /// </summary>
        /// <param name="pathPrefix">路径前缀（如 /elsa-api）</param>
        /// <param name="handler">编译后的请求处理委托</param>
        /// <param name="serviceProvider">插件私有的 ServiceProvider</param>
        public void MapBranch(string pathPrefix, RequestDelegate handler, IServiceProvider serviceProvider)
        {
            if (string.IsNullOrEmpty(pathPrefix))
            {
                throw new ArgumentException("路径前缀不能为空", nameof(pathPrefix));
            }

            if (handler == null)
            {
                throw new ArgumentNullException(nameof(handler));
            }

            if (serviceProvider == null)
            {
                throw new ArgumentNullException(nameof(serviceProvider));
            }

            // 确保路径前缀以 / 开头
            var normalizedPrefix = pathPrefix.StartsWith("/") ? pathPrefix : "/" + pathPrefix;

            _branches.TryAdd(normalizedPrefix.ToLower(), new PluginBranchInfo
            {
                Delegate = handler,
                PluginServiceProvider = serviceProvider
            });

            Console.WriteLine($"✅ 插件分支已注册: {normalizedPrefix}");
        }

        /// <summary>
        /// 注销一个插件分支
        /// </summary>
        /// <param name="pathPrefix">路径前缀</param>
        public void UnmapBranch(string pathPrefix)
        {
            if (string.IsNullOrEmpty(pathPrefix))
            {
                return;
            }

            var normalizedPrefix = pathPrefix.StartsWith("/") ? pathPrefix : "/" + pathPrefix;

            if (_branches.TryRemove(normalizedPrefix.ToLower(), out _))
            {
                Console.WriteLine($"✅ 插件分支已注销: {normalizedPrefix}");
            }
        }

        /// <summary>
        /// 尝试获取匹配的插件分支信息
        /// </summary>
        /// <param name="path">请求路径</param>
        /// <param name="branchInfo">匹配的分支信息</param>
        /// <returns>是否找到匹配的分支</returns>
        public bool TryGetMatchingBranch(string path, out PluginBranchInfo? branchInfo)
        {
            branchInfo = null;

            if (string.IsNullOrEmpty(path))
            {
                return false;
            }

            var lowerPath = path.ToLower();

            // 查找最长匹配的前缀
            var matchedPrefix = _branches.Keys
                .Where(prefix => lowerPath.StartsWith(prefix))
                .OrderByDescending(prefix => prefix.Length)
                .FirstOrDefault();

            if (matchedPrefix != null)
            {
                return _branches.TryGetValue(matchedPrefix, out branchInfo);
            }

            return false;
        }
    }
}
