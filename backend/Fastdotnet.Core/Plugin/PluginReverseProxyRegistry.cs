using System;
using System.Collections.Concurrent;
using System.Linq;

namespace Fastdotnet.Core.Plugin
{
    /// <summary>
    /// 插件逃生舱（微主机代理）的路由注册表。
    /// 用于将底座的特定路由前缀映射到插件内建的微型 Http 服务器（如 Kestrel）。
    /// </summary>
    public class PluginReverseProxyRegistry
    {
        // Key: pathPrefix (e.g., "/elsa"), Value: targetUrl (e.g., "http://127.0.0.1:23456")
        private readonly ConcurrentDictionary<string, string> _proxyRoutes = new(StringComparer.OrdinalIgnoreCase);

        /// <summary>
        /// 获取所有已注册的代理路由
        /// </summary>
        public IReadOnlyDictionary<string, string> Routes => _proxyRoutes;

        /// <summary>
        /// 将特定路径前缀反向代理至目标微主机的 URL
        /// </summary>
        /// <param name="pathPrefix">路径前缀（如 /elsa）</param>
        /// <param name="targetUrl">微主机的根地址（如 http://127.0.0.1:18890）</param>
        public void MapProxy(string pathPrefix, string targetUrl)
        {
            if (string.IsNullOrEmpty(pathPrefix))
                throw new ArgumentException("路径前缀不能为空", nameof(pathPrefix));

            if (string.IsNullOrEmpty(targetUrl))
                throw new ArgumentException("目标地址不能为空", nameof(targetUrl));

            var normalizedPrefix = pathPrefix.StartsWith("/") ? pathPrefix : "/" + pathPrefix;
            
            // 确保 targetUrl 不以 / 结尾，方便后续拼接
            var normalizedTarget = targetUrl.TrimEnd('/');

            _proxyRoutes[normalizedPrefix.ToLower()] = normalizedTarget;
            
            Console.WriteLine($"✅ 插件逃生舱反代已注册: {normalizedPrefix} -> {normalizedTarget}");
        }

        /// <summary>
        /// 注销特定前缀的反代路由
        /// </summary>
        /// <param name="pathPrefix">路径前缀</param>
        public void UnmapProxy(string pathPrefix)
        {
            if (string.IsNullOrEmpty(pathPrefix)) return;

            var normalizedPrefix = pathPrefix.StartsWith("/") ? pathPrefix : "/" + pathPrefix;
            
            if (_proxyRoutes.TryRemove(normalizedPrefix.ToLower(), out var removedTarget))
            {
                Console.WriteLine($"✅ 插件逃生舱反代已注销: {normalizedPrefix} -> {removedTarget}");
            }
        }

        /// <summary>
        /// 尝试匹配当前请求路径是否命中任何代理前缀
        /// </summary>
        /// <param name="path">请求路径</param>
        /// <param name="matchedPrefix">匹配到的最长前缀</param>
        /// <param name="targetUrl">命中前缀对应的目标服务器地址</param>
        /// <returns>是否匹配成功</returns>
        public bool TryGetMatch(string path, out string? matchedPrefix, out string? targetUrl)
        {
            matchedPrefix = null;
            targetUrl = null;

            if (string.IsNullOrEmpty(path)) return false;

            var lowerPath = path.ToLower();

            // 寻找最长匹配的前缀
            matchedPrefix = _proxyRoutes.Keys
                .Where(prefix => lowerPath.StartsWith(prefix))
                .OrderByDescending(prefix => prefix.Length)
                .FirstOrDefault();

            if (matchedPrefix != null)
            {
                return _proxyRoutes.TryGetValue(matchedPrefix, out targetUrl);
            }

            return false;
        }
    }
}
