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

        public HybridCacheService(HybridCache hybridCache, IOptions<CacheSettings> cacheSettings)
        {
            _hybridCache = hybridCache;
            _cacheSettings = cacheSettings;
        }

        /// <inheritdoc/>
        public async Task<T> GetOrCreateAsync<T>(string key, Func<Task<T>> factory, HybridCacheEntryOptions options = null, string[] tags = null)
        {
            var cacheOptions = options ?? CreateDefaultOptions();

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

            await _hybridCache.SetAsync(key, value, cacheOptions, tags);
        }

        /// <inheritdoc/>
        public async Task<T?> GetAsync<T>(string key)
        {
            // 使用 CancellationToken 取消机制实现"只读"缓存查询。
            // 预先取消的 token 会让工厂立即抛出 OperationCanceledException，
            // HybridCache 检测到后不会把 default(T) 写入缓存，实现无污染的"只读"。
            try
            {
                using var cts = new CancellationTokenSource();
                cts.Cancel();

                return await _hybridCache.GetOrCreateAsync<T>(
                    key,
                    (ct) =>
                    {
                        ct.ThrowIfCancellationRequested();
                        return default!;
                    }
                );
            }
            catch (OperationCanceledException)
            {
                return default;
            }
        }

        /// <inheritdoc/>
        public async Task RemoveAsync(string key)
        {
            await _hybridCache.RemoveAsync(key);
        }

        /// <inheritdoc/>
        public async Task RemoveByTagAsync(string[] tags)
        {
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
    }
}
