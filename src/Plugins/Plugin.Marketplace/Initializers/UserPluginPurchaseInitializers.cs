using Fastdotnet.Core.Entities.System;
using Fastdotnet.Core.Initializers;
using Fastdotnet.Core.IService;
using Fastdotnet.Plugin.Marketplace.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fastdotnet.Plugin.Marketplace.Initializers
{
    public class UserPluginPurchaseInitializers : IApplicationInitializer
    {
        private readonly IRepository<UserPluginPurchase> _Repository;

        public UserPluginPurchaseInitializers(IRepository<UserPluginPurchase> repository)
        {
            _Repository = repository;
        }
        public async Task InitializeAsync()
        {
            if (await _Repository.ExistsAsync(b => b.Id != null))
            {
                return;
            }
            var blacklistEntries = new List<UserPluginPurchase>
            {
                new UserPluginPurchase
                {
                    UserId = "11438163155878918",
                    PluginId = "11375910391972869",
                    OrderId = "1111",
                    LicenseType = LicenseType.SingleServer,
                    Quantity = 1,
                    PurchasePrice = 10,
                    PurchaseDate = DateTime.Now,
                    Currency = "CNY",
                    UpdatesUntil = DateTime.Now.AddMonths(6),
                    IsLifetime = true,
                    Status = PurchaseStatus.Completed,
                    Notes = "初始化超级管理员购买插件记录",

                }
            };
            await _Repository.InsertRangeAsync(blacklistEntries);

        }
    }
}
