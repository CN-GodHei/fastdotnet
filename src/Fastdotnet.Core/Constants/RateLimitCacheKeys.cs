namespace Fastdotnet.Core.Constants
{
    /// <summary>
    /// 限流缓存键常量
    /// </summary>
    public static class RateLimitCacheKeys
    {
        public const string BLACKLIST_PREFIX = "blacklist:";
        public const string RATE_LIMIT_RULE_PREFIX = "ratelimit:rule:";
        public const string RATE_LIMIT_COUNTER_PREFIX = "ratelimit:counter:";
    }
}