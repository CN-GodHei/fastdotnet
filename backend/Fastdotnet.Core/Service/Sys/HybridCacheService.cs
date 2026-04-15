using Fastdotnet.Core.Options;

namespace Fastdotnet.Core.Service.Sys
{
    /// <summary>
    /// 混合缓存服务实现
    /// </summary>
    public class HybridCacheService : IHybridCacheService
    {
        private readonly HybridCache _hybridCache;
        private readonly IOptions<CacheSettings> _cacheSettings;
        private readonly ConcurrentDictionary<string, HashSet<string>> _tagToKeysMap;
        private readonly ConcurrentDictionary<string, HashSet<string>> _keyToTagsMap;

        public HybridCacheService(HybridCache hybridCache, IOptions<CacheSettings> cacheSettings)
        {
            _hybridCache = hybridCache;
            _cacheSettings = cacheSettings;
            _tagToKeysMap = new ConcurrentDictionary<string, HashSet<string>>();
            _keyToTagsMap = new ConcurrentDictionary<string, HashSet<string>>();
        }

        /// <inheritdoc/>
        public async Task<T> GetOrCreateAsync<T>(string key, Func<Task<T>> factory, HybridCacheEntryOptions options = null, string[] tags = null)
        {
            // 如果没有提供选项，使用配置文件中的默认值
            var cacheOptions = options ?? CreateDefaultOptions();

            var result = await _hybridCache.GetOrCreateAsync(key, async (ct) =>
            {
                return await factory();
            }, cacheOptions);

            // 关联标签和键
            if (tags != null && tags.Length > 0)
            {
                AssociateTagsWithKey(key, tags);
            }

            return result;
        }

        /// <inheritdoc/>
        public async Task SetAsync<T>(string key, T value, HybridCacheEntryOptions options = null, string[] tags = null)
        {
            // 如果没有提供选项，使用配置文件中的默认值
            var cacheOptions = options ?? CreateDefaultOptions();

            await _hybridCache.SetAsync(key, value, cacheOptions, tags);

            // 关联标签和键
            if (tags != null && tags.Length > 0)
            {
                AssociateTagsWithKey(key, tags);
            }
        }

        /// <inheritdoc/>
        public async Task<T> GetAsync<T>(string key)
        {
            // HybridCache没有直接的GetAsync方法，我们需要使用GetOrCreateAsync但提供一个不会实际执行的工厂方法
            // 这里我们实现一个合理的Get方法，如果缓存中没有则返回默认值
            try
            {
                return await _hybridCache.GetOrCreateAsync<T>(key, async (ct) =>
                {
                    // 如果缓存中没有找到，返回默认值
                    return default(T);
                });
            }
            catch
            {
                // 如果发生异常，返回默认值
                return default(T);
            }
        }

        /// <inheritdoc/>
        public async Task RemoveAsync(string key)
        {
            await _hybridCache.RemoveAsync(key);

            // 移除标签关联
            if (_keyToTagsMap.TryGetValue(key, out var tags))
            {
                foreach (var tag in tags)
                {
                    if (_tagToKeysMap.TryGetValue(tag, out var keys))
                    {
                        keys.Remove(key);
                        // 如果标签下没有键了，也移除标签
                        if (keys.Count == 0)
                        {
                            _tagToKeysMap.TryRemove(tag, out _);
                        }
                    }
                }
                _keyToTagsMap.TryRemove(key, out _);
            }
        }

        /// <inheritdoc/>
        public async Task RemoveByTagAsync(string[] tags)
        {
            // 使用HybridCache的原生RemoveByTagAsync方法
            await _hybridCache.RemoveByTagAsync(tags);
            for (var i = 0; i < tags.Length; i++)
            {
                string tag = tags[i];
                // 更新我们自己的标签映射
                if (_tagToKeysMap.TryGetValue(tag, out var keys))
                {
                    // 移除所有关联的键的标签引用
                    foreach (var key in keys.ToList())
                    {
                        if (_keyToTagsMap.TryGetValue(key, out var keyTags))
                        {
                            keyTags.Remove(tag);
                            if (keyTags.Count == 0)
                            {
                                _keyToTagsMap.TryRemove(key, out _);
                            }
                        }
                    }

                    // 清空标签映射
                    _tagToKeysMap.TryRemove(tag, out _);
                }
            }

        }

        /// <summary>
        /// 创建默认缓存选项
        /// </summary>
        /// <returns>默认缓存选项</returns>
        private HybridCacheEntryOptions CreateDefaultOptions()
        {
            var settings = _cacheSettings.Value;
            return new HybridCacheEntryOptions
            {
                Expiration = TimeSpan.FromMinutes(settings.DistributedCacheExpirationMinutes),
                LocalCacheExpiration = TimeSpan.FromMinutes(settings.LocalCacheExpirationMinutes)
            };
        }

        /// <summary>
        /// 关联标签和键
        /// </summary>
        /// <param name="key">缓存键</param>
        /// <param name="tags">标签数组</param>
        private void AssociateTagsWithKey(string key, string[] tags)
        { 
            // 记录键到标签的映射
            var keyTags = _keyToTagsMap.GetOrAdd(key, _ => new HashSet<string>());
            foreach (var tag in tags)
            {
                keyTags.Add(tag);

                // 记录标签到键的映射
                var tagKeys = _tagToKeysMap.GetOrAdd(tag, _ => new HashSet<string>());
                tagKeys.Add(key);
            }
        }
    }
}