using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fastdotnet.Core.Dtos
{
    /// <summary>
    /// 缓存配置设置
    /// </summary>
    public class CacheSettings
    {
        /// <summary>
        /// 配置节名称
        /// </summary>
        public const string SectionName = "CacheSettings";

        /// <summary>
        /// 缓存类型：MemoryCache 或 Redis
        /// </summary>
        public string CacheType { get; set; } = "MemoryCache";

        /// <summary>
        /// Redis连接字符串（仅在使用Redis时需要）
        /// </summary>
        public string RedisConnectionString { get; set; } = "";

        /// <summary>
        /// Redis实例名称前缀
        /// </summary>
        public string RedisInstanceName { get; set; } = "Fastdotnet:";

        /// <summary>
        /// 本地缓存过期时间（分钟）
        /// </summary>
        public int LocalCacheExpirationMinutes { get; set; } = 5;

        /// <summary>
        /// 分布式缓存过期时间（分钟）
        /// </summary>
        public int DistributedCacheExpirationMinutes { get; set; } = 30;
    }
}