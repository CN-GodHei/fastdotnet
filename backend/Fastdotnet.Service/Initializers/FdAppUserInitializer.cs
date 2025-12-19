using Fastdotnet.Core.Entities.App;
using Fastdotnet.Core.Entities.System;
using Fastdotnet.Core.Initializers;
using Fastdotnet.Core.IService;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Org.BouncyCastle.Math.EC.ECCurve;

namespace Fastdotnet.Service.Initializers
{
    public class FdAppUserInitializer : IApplicationInitializer
    {
        private readonly IRepository<FdAppUser> _Repository;

        public FdAppUserInitializer(IRepository<FdAppUser> Repository)
        {
            _Repository = Repository;
        }

        public async Task InitializeAsync()
        {
            //if (await _Repository.ExistsAsync(a => a.Id != null))
            //{
            //    return;
            //}
            var entitys = new List<FdAppUser>
            {
                new FdAppUser { Id = "12055423131517957", Username = "admintest", Nickname = "AdminTest", Email = "fastdotnet@test.com", Password = "123456", PhoneNumber = "159****7417" }
            };

            // 直接使用条件查询已存在的项
            var existing = await _Repository.GetListAsync(m => entitys.Select(c => c.Id).Contains(m.Id));
            var existingIds = existing.Select(m => m.Id).ToHashSet();

            // 只插入不存在的项
            var ToInsert = entitys.Where(c => !existingIds.Contains(c.Id)).ToList();

            if (ToInsert.Any())
            {
                await _Repository.InsertRangeAsync(ToInsert);
            }
        }
    }
}
