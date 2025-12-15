using System.Threading.Tasks;
using Fastdotnet.Core.Initializers;
using Fastdotnet.Core.IService;
using Fastdotnet.Core.Entities.System;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Linq;
using System;

namespace Fastdotnet.Service.Initializers
{
    /// <summary>
    /// 黑名单数据初始化器
    /// </summary>
    public class BlacklistInitializer : IApplicationInitializer
    {
        private readonly IRepository<FdBlacklist, string> _blacklistRepository;
        private readonly ILogger<BlacklistInitializer> _logger;

        public BlacklistInitializer(
            IRepository<FdBlacklist, string> blacklistRepository,
            ILogger<BlacklistInitializer> logger)
        {
            _blacklistRepository = blacklistRepository;
            _logger = logger;
        }

        public async Task InitializeAsync()
        {
            //_logger.LogInformation("Start: Initializing Blacklist data...");

            if (await _blacklistRepository.ExistsAsync(b => b.Id != null))
            {
                //_logger.LogInformation("Blacklist data already seeded. Skipping initialization.");
                return;
            }

            var blacklistEntries = new List<FdBlacklist>
            {
                // 示例：添加一些常见的恶意IP地址到黑名单
                new FdBlacklist 
                { 
                    Type = "IP", 
                    Value = "192.168.1.100", 
                    Reason = "已知的恶意IP地址",
                    IsSystem = true // 标记为系统预设的黑名单条目
                },
                new FdBlacklist 
                { 
                    Type = "IP", 
                    Value = "10.0.0.1", 
                    Reason = "已知的扫描器IP地址",
                    IsSystem = true // 标记为系统预设的黑名单条目
                },
                new FdBlacklist 
                { 
                    Type = "IP", 
                    Value = "172.16.0.50", 
                    Reason = "已知的攻击源IP地址",
                    IsSystem = true // 标记为系统预设的黑名单条目
                },

                // 示例：添加一些恶意用户ID到黑名单
                new FdBlacklist 
                { 
                    Type = "User", 
                    Value = "user-malicious-001", 
                    Reason = "已知的恶意用户",
                    IsSystem = true // 标记为系统预设的黑名单条目
                },
                new FdBlacklist 
                { 
                    Type = "User", 
                    Value = "user-spammer-002", 
                    Reason = "已知的垃圾信息发布者",
                    IsSystem = true // 标记为系统预设的黑名单条目
                },

                // 示例：添加一些恶意API密钥到黑名单
                new FdBlacklist 
                { 
                    Type = "ApiKey", 
                    Value = "api-key-compromised-001", 
                    Reason = "已泄露的API密钥",
                    IsSystem = true // 标记为系统预设的黑名单条目
                },
                new FdBlacklist 
                { 
                    Type = "ApiKey", 
                    Value = "api-key-abused-002", 
                    Reason = "滥用的API密钥",
                    IsSystem = true // 标记为系统预设的黑名单条目
                }
            };

            await _blacklistRepository.InsertRangeAsync(blacklistEntries);

            //_logger.LogInformation("Finish: Blacklist data initialization complete.");
        }
    }
}