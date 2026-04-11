namespace Fastdotnet.Core.Middleware
{
    public class PluginStaticFileProviderRegistry
    {
        private readonly ConcurrentDictionary<string, IFileProvider> _providers = new ConcurrentDictionary<string, IFileProvider>(StringComparer.OrdinalIgnoreCase);
        private readonly HashSet<string> _spaFallbackPaths = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

        /// <summary>
        /// 注册静态文件路径
        /// </summary>
        /// <param name="requestPath">请求路径前缀，如 /plugins/myplugin/admin</param>
        /// <param name="physicalPath">物理路径</param>
        /// <param name="enableSpaFallback">是否启用 SPA 回退（文件不存在时返回 index.html）</param>
        public void Register(string requestPath, string physicalPath, bool enableSpaFallback = false)
        {
            if (Directory.Exists(physicalPath))
            {
                var fileProvider = new PhysicalFileProvider(physicalPath);
                _providers[requestPath] = fileProvider;
                
                if (enableSpaFallback)
                {
                    _spaFallbackPaths.Add(requestPath.ToLowerInvariant());
                }
            }
        }

        public void Unregister(string requestPath)
        {
            _providers.TryRemove(requestPath, out _);
            _spaFallbackPaths.Remove(requestPath.ToLowerInvariant());
        }

        public IFileProvider? GetProvider(string requestPath)
        {
            // Find the longest matching prefix
            var bestMatch = _providers.Keys
                .Where(prefix => requestPath.StartsWith(prefix, StringComparison.OrdinalIgnoreCase))
                .OrderByDescending(prefix => prefix.Length)
                .FirstOrDefault();

            if (bestMatch != null && _providers.TryGetValue(bestMatch, out var provider))
            {
                return provider;
            }

            return null;
        }

        public string? GetBestMatchRequestPath(string requestPath)
        {
            return _providers.Keys
                .Where(prefix => requestPath.StartsWith(prefix, StringComparison.OrdinalIgnoreCase))
                .OrderByDescending(prefix => prefix.Length)
                .FirstOrDefault();
        }

        /// <summary>
        /// 检查指定路径是否启用了 SPA 回退
        /// </summary>
        public bool IsSpaFallbackEnabled(string requestPath)
        {
            return _spaFallbackPaths.Contains(requestPath.ToLowerInvariant());
        }
    }
}
