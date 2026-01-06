using Fastdotnet.Core.Entities.Admin;
using Fastdotnet.Core.Entities.App;
using Fastdotnet.Core.Enum;
using Fastdotnet.Core.IService;
using Fastdotnet.Core.IService.Sys;
using Microsoft.Extensions.Caching.Hybrid;
using Microsoft.Extensions.Caching.Memory;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Fastdotnet.Core.Service.Sys
{
    public class UserDisplayNameService : IUserDisplayNameService
    {
        //private readonly IBaseRepository<FdAdminUser> _adminUserRepository;
        //private readonly IBaseRepository<FdAppUser> _appUserRepository;
        private readonly IBaseService<FdAdminUser> _adminUserService;
        private readonly IBaseService<FdAppUser> _appUserService;
        private readonly IHybridCacheService _cache;

        // ⚠️ 最大单次查询用户数（防 DoS）
        private const int MaxBatchSize = 100;

        public UserDisplayNameService(
            //IBaseRepository<FdAdminUser> adminUserRepository,
            //IBaseRepository<FdAppUser> appUserRepository,
            IBaseService<FdAdminUser> adminUserService,
            IBaseService<FdAppUser> appUserService,
            IHybridCacheService cache)
        {
            //_adminUserRepository = adminUserRepository;
            //_appUserRepository = appUserRepository;
            _cache = cache;
            _adminUserService = adminUserService;
            _appUserService = appUserService;
        }

        public async Task<Dictionary<string, string>> GetDisplayNamesAsync(
            IEnumerable<string> userIds,
            SystemCategory systemCategory)
        {
            if (userIds == null) return new();

            var uniqueIds = userIds
                .Where(id => !string.IsNullOrWhiteSpace(id))
                .Distinct()
                .Take(MaxBatchSize)
                .ToList();

            if (!uniqueIds.Any()) return new();

            var cacheKey = GenerateCacheKey(systemCategory, uniqueIds);

            var cacheOptions = new HybridCacheEntryOptions
            {
                //SlidingExpiration = TimeSpan.FromMinutes(10),
                //AbsoluteExpirationRelativeToNow = TimeSpan.FromHours(1)
                Expiration = TimeSpan.FromMinutes(10),
                LocalCacheExpiration = TimeSpan.FromHours(1)
            };

            return await _cache.GetOrCreateAsync<Dictionary<string, string>>(
                cacheKey,
                async () =>
                {
                    Dictionary<string, string> nameMap;

                    switch (systemCategory)
                    {
                        case SystemCategory.Admin:
                            //var adminUsers = await _adminUserRepository.GetListAsync(
                            var adminUsers = await _adminUserService.GetListAsync(
                                u => uniqueIds.Contains(u.Id),
                                u => new { u.Id, u.Name }
                            );
                            nameMap = adminUsers.ToDictionary(x => x.Id, x => x.Name?.Trim() ?? "管理员");
                            break;

                        case SystemCategory.App:
                            //var appUsers = await _appUserRepository.GetListAsync(
                            var appUsers = await _appUserService.GetListAsync(
                                u => uniqueIds.Contains(u.Id),
                                u => new { u.Id, u.Nickname }
                            );
                            nameMap = appUsers.ToDictionary(x => x.Id, x => x.Nickname?.Trim() ?? "用户");
                            break;

                        default:
                            throw new ArgumentOutOfRangeException(nameof(systemCategory));
                    }

                    foreach (var id in uniqueIds)
                    {
                        if (!nameMap.ContainsKey(id))
                        {
                            nameMap[id] = systemCategory == SystemCategory.Admin
                                ? "已删除管理员"
                                : "已注销用户";
                        }
                    }

                    return nameMap;
                },
                cacheOptions
            );
        }

        // ✅ 5. 安全生成缓存键（避免 Key 超长）
        private static string GenerateCacheKey(SystemCategory category, List<string> userIds)
        {
            // 排序保证相同集合生成相同 key
            var sortedIds = string.Join(",", userIds.OrderBy(x => x));
            var rawKey = $"UserNames_{category}_{sortedIds}";

            // 如果 key 太长（>200字符），用 SHA256 哈希
            if (rawKey.Length > 200)
            {
                using var sha256 = SHA256.Create();
                var hashBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(rawKey));
                var hash = Convert.ToHexString(hashBytes).ToLowerInvariant();
                return $"UserNames_{category}_hash_{hash}";
            }

            return rawKey;
        }
    }
}