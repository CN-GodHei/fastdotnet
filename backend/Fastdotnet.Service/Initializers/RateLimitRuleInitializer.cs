

using Fastdotnet.Core.Entities.Sys;

namespace Fastdotnet.Service.Initializers
{
    /// <summary>
    /// 限流规则数据初始化器
    /// </summary>
    public class RateLimitRuleInitializer : IApplicationInitializer
    {
        private readonly IRepository<FdRateLimitRule, string> _rateLimitRuleRepository;
        private readonly ILogger<RateLimitRuleInitializer> _logger;

        public RateLimitRuleInitializer(
            IRepository<FdRateLimitRule, string> rateLimitRuleRepository,
            ILogger<RateLimitRuleInitializer> logger)
        {
            _rateLimitRuleRepository = rateLimitRuleRepository;
            _logger = logger;
        }

        public async Task InitializeAsync()
        {
            //_logger.LogInformation("Start: Initializing Rate Limit Rule data...");

            if (await _rateLimitRuleRepository.ExistsAsync(r => r.Id != null))
            {
                //_logger.LogInformation("Rate limit rule data already seeded. Skipping initialization.");
                return;
            }

            var rateLimitRules = new List<FdRateLimitRule>
            {
                // ==================== 全局限流规则 (Global Rate Limiting) ====================
                // 这些规则限制整个系统范围内对特定路由的访问频率
                
                new FdRateLimitRule 
                { 
                    Type = "Route", 
                    Key = "/api/auth/login", 
                    PermitLimit = 1000, 
                    WindowSeconds = 60, 
                    Description = "登录接口全局限流规则：每分钟最多1000次请求（整个系统范围内）",
                    IsSystem = true
                },
                new FdRateLimitRule 
                { 
                    Type = "Route", 
                    Key = "/api/users/register", 
                    PermitLimit = 100, 
                    WindowSeconds = 60, 
                    Description = "注册接口全局限流规则：每分钟最多100次请求（整个系统范围内）",
                    IsSystem = true
                },

                // ==================== 客户端限流规则 (Per-Client Rate Limiting) ====================
                // 这些规则限制特定客户端对系统的访问频率

                // IP地址限流规则
                new FdRateLimitRule 
                { 
                    Type = "IP", 
                    Key = "default", 
                    PermitLimit = 100, 
                    WindowSeconds = 60, 
                    Description = "默认IP限流规则：每个IP地址每分钟最多100次请求",
                    IsSystem = true
                },
                new FdRateLimitRule 
                { 
                    Type = "IP", 
                    Key = "strict", 
                    PermitLimit = 10, 
                    WindowSeconds = 60, 
                    Description = "严格IP限流规则：每个IP地址每分钟最多10次请求",
                    IsSystem = true
                },

                // 用户ID限流规则
                new FdRateLimitRule 
                { 
                    Type = "UserId", 
                    Key = "default", 
                    PermitLimit = 1000, 
                    WindowSeconds = 3600, 
                    Description = "默认用户ID限流规则：每个用户每小时最多1000次请求",
                    IsSystem = true
                },
                new FdRateLimitRule 
                { 
                    Type = "UserId", 
                    Key = "premium", 
                    PermitLimit = 10000, 
                    WindowSeconds = 3600, 
                    Description = "高级用户ID限流规则：每个高级用户每小时最多10000次请求",
                    IsSystem = true
                },

                // API密钥限流规则
                new FdRateLimitRule 
                { 
                    Type = "ApiKey", 
                    Key = "default", 
                    PermitLimit = 1000, 
                    WindowSeconds = 3600, 
                    Description = "默认API密钥限流规则：每个API密钥每小时最多1000次请求",
                    IsSystem = true
                },
                new FdRateLimitRule 
                { 
                    Type = "ApiKey", 
                    Key = "premium", 
                    PermitLimit = 10000, 
                    WindowSeconds = 3600, 
                    Description = "高级API密钥限流规则：每个高级API密钥每小时最多10000次请求",
                    IsSystem = true
                }
            };

            await _rateLimitRuleRepository.InsertRangeAsync(rateLimitRules);

            //_logger.LogInformation("Finish: Rate Limit Rule data initialization complete.");
        }
    }
}