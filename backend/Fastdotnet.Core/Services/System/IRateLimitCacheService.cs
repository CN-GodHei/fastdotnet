
namespace Fastdotnet.Core.Services.System
{
    /// <summary>
    /// 限流缓存服务接口
    /// </summary>
    public interface IRateLimitCacheService
    {
        /// <summary>
        /// 获取指定类型的黑名单集合
        /// </summary>
        Task<HashSet<string>> GetBlacklistAsync(string type);

        /// <summary>
        /// 检查值是否在黑名单中
        /// </summary>
        Task<bool> IsBlacklistedAsync(string type, string value);

        /// <summary>
        /// 添加黑名单条目到缓存
        /// </summary>
        Task AddToBlacklistAsync(string type, string value);

        /// <summary>
        /// 从缓存中移除黑名单条目
        /// </summary>
        Task RemoveFromBlacklistAsync(string type, string value);

        /// <summary>
        /// 获取限流规则
        /// </summary>
        Task<FdRateLimitRuleDto?> GetRateLimitRuleAsync(string type, string key);

        /// <summary>
        /// 设置限流规则到缓存
        /// </summary>
        Task SetRateLimitRuleAsync(string type, string key, FdRateLimitRuleDto dto);

        /// <summary>
        /// 移除限流规则缓存
        /// </summary>
        Task RemoveRateLimitRuleAsync(string type, string key);

        /// <summary>
        /// 增加限流计数器
        /// </summary>
        Task<long> IncrementRateLimitCounterAsync(string type, string key, int windowSeconds);

        /// <summary>
        /// 重置限流计数器
        /// </summary>
        Task ResetRateLimitCounterAsync(string type, string key);

        /// <summary>
        /// 检查是否触发限流
        /// </summary>
        Task<bool> IsRateLimitedAsync(string type, string key);
    }
}