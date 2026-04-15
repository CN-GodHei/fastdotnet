using Fastdotnet.Core.Options;

namespace Fastdotnet.Core.Utils.Extensions
{
    /// <summary>
    /// 缓存服务扩展方法
    /// </summary>
    public static class CacheServiceExtensions
    {
        /// <summary>
        /// 添加混合缓存服务
        /// </summary>
        /// <param name="services">服务集合</param>
        /// <param name="configuration">配置</param>
        /// <returns>服务集合</returns>
        public static IServiceCollection AddHybridCacheService(this IServiceCollection services, IConfiguration configuration)
        {
            // 获取缓存配置
            var cacheSettings = configuration.GetSection(CacheSettings.SectionName).Get<CacheSettings>() ?? new CacheSettings();

            // 注册配置
            services.Configure<CacheSettings>(configuration.GetSection(CacheSettings.SectionName));

            // 添加混合缓存
            services.AddHybridCache();

            // 根据配置选择缓存类型
            if (string.Equals(cacheSettings.CacheType, "Redis", StringComparison.OrdinalIgnoreCase))
            {
                // 添加Redis分布式缓存
                services.AddStackExchangeRedisCache(options =>
                {
                    options.Configuration = cacheSettings.RedisConnectionString;
                    options.InstanceName = cacheSettings.RedisInstanceName;
                });
            }
            else
            {
                // 默认使用内存缓存作为分布式缓存（仅用于开发环境）
                services.AddDistributedMemoryCache();
            }

            // 注册缓存服务
            services.AddSingleton<IHybridCacheService, HybridCacheService>();

            return services;
        }
    }
}