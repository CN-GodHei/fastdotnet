using System.Threading.Tasks;
using Fastdotnet.Core.Initializers;
using Fastdotnet.Core.IService;
using Fastdotnet.Core.Entities.System;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Linq;

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
            _logger.LogInformation("Start: Initializing Rate Limit Rule data...");

            if (await _rateLimitRuleRepository.ExistsAsync(r => r.Id != null))
            {
                _logger.LogInformation("Rate limit rule data already seeded. Skipping initialization.");
                return;
            }

            var rateLimitRules = new List<FdRateLimitRule>
            {
                // 示例：添加一些默认的IP限流规则
                new FdRateLimitRule 
                { 
                    Type = "IP", 
                    Key = "default-ip-rule", 
                    PermitLimit = 100, 
                    WindowSeconds = 60, 
                    Description = "默认IP限流规则：每分钟最多100次请求",
                    IsSystem = true // 标记为系统预设的限流规则
                },
                new FdRateLimitRule 
                { 
                    Type = "IP", 
                    Key = "strict-ip-rule", 
                    PermitLimit = 10, 
                    WindowSeconds = 60, 
                    Description = "严格IP限流规则：每分钟最多10次请求",
                    IsSystem = true // 标记为系统预设的限流规则
                },

                // 示例：添加一些默认的用户ID限流规则
                new FdRateLimitRule 
                { 
                    Type = "UserId", 
                    Key = "default-user-rule", 
                    PermitLimit = 1000, 
                    WindowSeconds = 3600, 
                    Description = "默认用户ID限流规则：每小时最多1000次请求",
                    IsSystem = true // 标记为系统预设的限流规则
                },
                new FdRateLimitRule 
                { 
                    Type = "UserId", 
                    Key = "premium-user-rule", 
                    PermitLimit = 10000, 
                    WindowSeconds = 3600, 
                    Description = "高级用户ID限流规则：每小时最多10000次请求",
                    IsSystem = true // 标记为系统预设的限流规则
                },

                // 示例：添加一些默认的API密钥限流规则
                new FdRateLimitRule 
                { 
                    Type = "ApiKey", 
                    Key = "default-apikey-rule", 
                    PermitLimit = 1000, 
                    WindowSeconds = 3600, 
                    Description = "默认API密钥限流规则：每小时最多1000次请求",
                    IsSystem = true // 标记为系统预设的限流规则
                },
                new FdRateLimitRule 
                { 
                    Type = "ApiKey", 
                    Key = "premium-apikey-rule", 
                    PermitLimit = 10000, 
                    WindowSeconds = 3600, 
                    Description = "高级API密钥限流规则：每小时最多10000次请求",
                    IsSystem = true // 标记为系统预设的限流规则
                },

                // 示例：添加一些默认的路由限流规则
                new FdRateLimitRule 
                { 
                    Type = "Route", 
                    Key = "/api/auth/login", 
                    PermitLimit = 10, 
                    WindowSeconds = 60, 
                    Description = "登录接口限流规则：每分钟最多10次请求",
                    IsSystem = true // 标记为系统预设的限流规则
                },
                new FdRateLimitRule 
                { 
                    Type = "Route", 
                    Key = "/api/users/register", 
                    PermitLimit = 5, 
                    WindowSeconds = 60, 
                    Description = "注册接口限流规则：每分钟最多5次请求",
                    IsSystem = true // 标记为系统预设的限流规则
                }
            };

            await _rateLimitRuleRepository.InsertRangeAsync(rateLimitRules);

            _logger.LogInformation("Finish: Rate Limit Rule data initialization complete.");
        }
    }
}