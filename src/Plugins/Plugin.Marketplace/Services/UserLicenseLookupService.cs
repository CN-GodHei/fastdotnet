using Fastdotnet.Plugin.Marketplace.Dto;
using Fastdotnet.Plugin.Marketplace.Entities;
using Fastdotnet.Plugin.Marketplace.IService;
using SqlSugar;
using System;

namespace Fastdotnet.Plugin.Marketplace.Services
{
    /// <summary>
    /// 基于数据库的用户授权信息查询服务实现。
    /// </summary>
    public class UserLicenseLookupService : IUserLicenseLookupService
    {
        private readonly ISqlSugarClient _db;

        public UserLicenseLookupService(ISqlSugarClient db)
        {
            _db = db;
        }

        /// <summary>
        /// 根据用户ID和插件ID获取用户的授权信息。
        /// </summary>
        /// <param name="userId">用户ID。</param>
        /// <param name="pluginId">插件ID。</param>
        /// <returns>如果找到有效的授权信息则返回 UserLicenseInfoDto 对象，否则返回 null。</returns>
        public UserLicenseInfoDto GetUserLicenseInfo(string userId, string pluginId)
        {
            // 1. 查询用户对该插件的有效购买记录
            // 这里假设一个用户对一个插件只有一条有效的购买记录，
            // 或者我们取最近的一条有效记录。
            // 实际逻辑可能更复杂，例如需要检查是否已退款、是否已过期等。
            var purchaseRecord = _db.Queryable<UserPluginPurchase>()
                .Where(p => p.UserId == userId && 
                            p.PluginId == pluginId && 
                            p.Status == PurchaseStatus.Completed) // 只查询已完成的购买
                .OrderBy(p => p.PurchaseDate, OrderByType.Desc) // 取最新的
                .First();

            if (purchaseRecord == null)
            {
                // 未找到购买记录
                return null;
            }

            // 2. 构造并返回 DTO
            // 这里直接使用购买记录中的信息来填充 DTO
            // 在更复杂的场景下，可能还需要查询插件信息表或其他相关表
            var licenseInfo = new UserLicenseInfoDto
            {
                UserId = purchaseRecord.UserId,
                PluginId = purchaseRecord.PluginId,
                LicenseType = purchaseRecord.LicenseType.ToString(), // 枚举转字符串
                IssueDate = purchaseRecord.PurchaseDate,
                UpdatesUntil = purchaseRecord.UpdatesUntil
                // 可以根据需要添加更多字段
            };

            return licenseInfo;
        }
    }
}