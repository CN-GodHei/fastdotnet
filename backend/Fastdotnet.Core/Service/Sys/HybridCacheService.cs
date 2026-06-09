using Fastdotnet.Core.Options;

namespace Fastdotnet.Core.Service.Sys
{
    /// <summary>
    /// 混合缓存服务实现（优化后版本）
    /// </summary>
    public class HybridCacheService : IHybridCacheService
    {
        private readonly HybridCache _hybridCache;
        private readonly IOptions<CacheSettings> _cacheSettings;

        // 💡 彻底移除了 _tagToKeysMap 和 _keyToTagsMap，消除内存泄漏与集群状态不一致的隐患。
        public HybridCacheService(HybridCache hybridCache, IOptions<CacheSettings> cacheSettings)
        {
            _hybridCache = hybridCache;
            _cacheSettings = cacheSettings;
        }

        /// <inheritdoc/>
        public async Task<T> GetOrCreateAsync<T>(string key, Func<Task<T>> factory, HybridCacheEntryOptions options = null, string[] tags = null)
        {
            // 如果没有提供选项，使用配置文件中的默认值
            var cacheOptions = options ?? CreateDefaultOptions();

            // 💡 .NET 9 的 HybridCache.GetOrCreateAsync 原生重载就支持传入 tags
            // 底层会自动在本地内存和分布式缓存（如 Redis）中建立标签索引
            return await _hybridCache.GetOrCreateAsync<T>(
                key,
                async (ct) => await factory(),
                cacheOptions,
                tags);
        }

        /// <inheritdoc/>
        public async Task SetAsync<T>(string key, T value, HybridCacheEntryOptions options = null, string[] tags = null)
        {
            var cacheOptions = options ?? CreateDefaultOptions();

            // 💡 移除自定义关联函数，直接交由原生底层托管
            await _hybridCache.SetAsync(key, value, cacheOptions, tags);
        }

        /// <inheritdoc/>
        public async Task<T> GetAsync<T>(string key)
        {
            // ⚠️ 修复原版缺陷：原版代码中如果缓存未命中，工厂返回了 default(T)，
            // 这会导致 HybridCache 把 default(T) 当成有效结果重新写入缓存，导致该 Key 被“空值”污染。
            // 
            // 💡 规避方案：在工厂中抛出特定异常。HybridCache 捕获到工厂异常时，
            // 会判定为加载失败，【不会】向缓存层执行写入操作，从而完美实现“只读”而不污染缓存。
            try
            {
                return await _hybridCache.GetOrCreateAsync<T>(
                    key,
                    async (ct) => throw new CacheMissException() // 强行触发未命中中断
                );
            }
            catch (CacheMissException)
            {
                // 缓存未命中，安全返回默认值，且不会污染缓存
                return default;
            }
            catch
            {
                // 其他异常（如反序列化失败等）安全返回默认值
                return default;
            }
        }

        /// <inheritdoc/>
        public async Task RemoveAsync(string key)
        {
            // 💡 原生方法会自动在本地和分布式缓存中同步删除该 Key
            await _hybridCache.RemoveAsync(key);
        }

        /// <inheritdoc/>
        public async Task RemoveByTagAsync(string[] tags)
        {
            // 💡 .NET 9 的 HybridCache.RemoveByTagAsync 原生支持传入 IEnumerable<string>
            // 它的底层实现是“逻辑作废（基于时间戳匹配）”，非常高效，不需要我们自己去删 Key
            await _hybridCache.RemoveByTagAsync(tags);
        }

        /// <summary>
        /// 创建默认缓存选项
        /// </summary>
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
        /// 自定义内部专用异常，用于无污染阻断缓存写入
        /// </summary>
        private class CacheMissException : Exception { }
    }
}