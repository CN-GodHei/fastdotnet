using Fastdotnet.Core.Entities.App;
using Fastdotnet.Core.Entities.System;
using Fastdotnet.Core.Enum;
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
    public class FdRoleInitializer : IApplicationInitializer
    {
        private readonly IRepository<FdRole> _Repository;

        public FdRoleInitializer(IRepository<FdRole> Repository)
        {
            _Repository = Repository;
        }
        const string superAdminRoleCode = "SUPER_ADMIN";
        const string APP_NOMAL_ROLECODE = "APP_ROLE_001";

        public async Task InitializeAsync()
        {
            var entitys = new List<FdRole>
            {
                new FdRole{Name = "超级角色",Code = superAdminRoleCode, Category = "Admin",IsSystem = true,Description = "拥有系统所有权限", Belong= SystemCategory.Admin},
                new FdRole{Name = "普通角色",Code = APP_NOMAL_ROLECODE, Category = "APP",IsSystem = true,Description = "APP端普通角色", Belong= SystemCategory.App},
            };

            // 直接使用条件查询已存在的项
            var existing = await _Repository.GetListAsync(m => entitys.Select(c => c.Code).Contains(m.Code));
            var existingIds = existing.Select(m => m.Code).ToHashSet();

            // 只插入不存在的项
            var ToInsert = entitys.Where(c => !existingIds.Contains(c.Code)).ToList();

            if (ToInsert.Any())
            {
                await _Repository.InsertRangeAsync(ToInsert);
            }
        }
    }
}
