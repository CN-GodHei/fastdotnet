using Fastdotnet.Core.Dtos.System;
using Fastdotnet.Core.IService;
using Microsoft.Extensions.Caching.Hybrid;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Fastdotnet.Core.Services.System
{
    /// <summary>
    /// 限流缓存服务实现
    /// </summary>
    public class RateLimitCacheService : IRateLimitCacheService
    {
        private readonly IHybridCacheService _hybridCacheService;
        private readonly ConcurrentDictionary<string, HashSet<string>> _blacklistCache;

        public RateLimitCacheService(IHybridCacheService hybridCacheService)
        {
            _hybridCacheService = hybridCacheService;
            _blacklistCache = new ConcurrentDictionary<string, HashSet<string>>();
        }

        /// <summary>
        /// 获取指定类型的黑名单集合
        /// </summary>
        public async Task<HashSet<string>> GetBlacklistAsync(string type)
        {
            var cacheKey = $"blacklist:{type}";
            var blacklist = await _hybridCacheService.GetAsync<HashSet<string>>(cacheKey);
            return blacklist ?? new HashSet<string>();
        }

        /// <summary>
        /// 检查值是否在黑名单中
        /// </summary>
        public async Task<bool> IsBlacklistedAsync(string type, string value)
        {
            var blacklist = await GetBlacklistAsync(type);
            return blacklist.Contains(value);
        }

        /// <summary>
        /// 添加黑名单条目到缓存
        /// </summary>
        public async Task AddToBlacklistAsync(string type, string value)
        {
            var cacheKey = $"blacklist:{type}";
            var blacklist = await GetBlacklistAsync(type);
            blacklist.Add(value);
            await _hybridCacheService.SetAsync(cacheKey, blacklist);
        }

        /// <summary>
        /// 从缓存中移除黑名单条目
        /// </summary>
        public async Task RemoveFromBlacklistAsync(string type, string value)
        {
            var cacheKey = $"blacklist:{type}";
            var blacklist = await GetBlacklistAsync(type);
            blacklist.Remove(value);
            await _hybridCacheService.SetAsync(cacheKey, blacklist);
        }

        /// <summary>
        /// 获取限流规则
        /// </summary>
        public async Task<FdRateLimitRuleDto?> GetRateLimitRuleAsync(string type, string key)
        {
            // 首先尝试获取具体的规则
            var specificCacheKey = $"ratelimit:rule:{type}:{key}";
            var specificRule = await _hybridCacheService.GetAsync<FdRateLimitRuleDto>(specificCacheKey);
            
            if (specificRule != null)
                return specificRule;

            // 如果没有具体的规则，尝试获取默认规则
            var defaultCacheKey = $"ratelimit:rule:{type}:default";
            return await _hybridCacheService.GetAsync<FdRateLimitRuleDto>(defaultCacheKey);
        }

        /// <summary>
        /// 设置限流规则到缓存
        /// </summary>
        public async Task SetRateLimitRuleAsync(string type, string key, FdRateLimitRuleDto dto)
        {
            var cacheKey = $"ratelimit:rule:{type}:{key}";
            var options = new HybridCacheEntryOptions
            {
                LocalCacheExpiration = TimeSpan.FromMinutes(5),
                Expiration = TimeSpan.FromHours(1)
            };
            await _hybridCacheService.SetAsync(cacheKey, dto, options);
        }

        /// <summary>
        /// 移除限流规则缓存
        /// </summary>
        public async Task RemoveRateLimitRuleAsync(string type, string key)
        {
            var cacheKey = $"ratelimit:rule:{type}:{key}";
            await _hybridCacheService.RemoveAsync(cacheKey);
        }

        /// <summary>
        /// 增加限流计数器
        /// </summary>
        public async Task<long> IncrementRateLimitCounterAsync(string type, string key, int windowSeconds)
        {
            var cacheKey = $"ratelimit:counter:{type}:{key}";
            var options = new HybridCacheEntryOptions
            {
                LocalCacheExpiration = TimeSpan.FromSeconds(windowSeconds),
                Expiration = TimeSpan.FromSeconds(windowSeconds)
            };

            // 获取当前计数
            var currentCount = await _hybridCacheService.GetAsync<long>(cacheKey);
            
            // 增加计数
            var newCount = currentCount + 1;
            
            // 更新缓存
            await _hybridCacheService.SetAsync(cacheKey, newCount, options);
            
            return newCount;
        }

        /// <summary>
        /// 重置限流计数器
        /// </summary>
        public async Task ResetRateLimitCounterAsync(string type, string key)
        {
            var cacheKey = $"ratelimit:counter:{type}:{key}";
            await _hybridCacheService.RemoveAsync(cacheKey);
        }

        /// <summary>
        /// 检查是否触发限流
        /// </summary>
        public async Task<bool> IsRateLimitedAsync(string type, string key)
        {
            // 获取限流规则
            var rule = await GetRateLimitRuleAsync(type, key);
            if (rule == null)
                return false;

            // 获取当前计数
            var cacheKey = $"ratelimit:counter:{type}:{key}";
            var currentCount = await _hybridCacheService.GetAsync<long>(cacheKey);

            // 检查是否超过限制
            return currentCount >= rule.PermitLimit;
        }
    }
}