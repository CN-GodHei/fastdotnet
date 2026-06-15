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
        public async Task<T> GetAsync<T>(string key)
        {
            // 通过 CancellationToken 参数取消操作，而非在工厂中抛异常。
            // factory 内抛异常会被 BackgroundFetchAsync 重新抛出导致逃逸；
            // 用预取消的 token 可以在 HybridCache 进入工厂前就取消，安全无污染。
            try
            {
                using var cts = new CancellationTokenSource();
                cts.Cancel();

                return await _hybridCache.GetOrCreateAsync<T>(
                    key,
                    _ => default!,
                    cancellationToken: cts.Token
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