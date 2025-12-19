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
    public class FdAppUserInitializer : IApplicationInitializer
    {
        private readonly IRepository<FdAppUser> _Repository;
        private readonly IRepository<FdAppUserRole> _UserRoleRepository;
        private readonly IRepository<FdRole> _fdroleRepository;

        public FdAppUserInitializer(IRepository<FdAppUser> Repository, IRepository<FdAppUserRole> userRoleRepository, IRepository<FdRole> fdroleRepository)
        {
            _Repository = Repository;
            _UserRoleRepository = userRoleRepository;
            _fdroleRepository = fdroleRepository;
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
            const string superAdminRoleCode = "SUPER_ADMIN";
            const string APP_NOMAL_ROLECODE = "APP_ROLE_001";
            var _entitys = new List<FdRole>
            {
                //new FdRole{Name = "超级角色",Code = superAdminRoleCode, Category = "Admin",IsSystem = true,Description = "拥有系统所有权限", Belong= SystemCategory.Admin},
                new FdRole{Name = "普通角色",Code = APP_NOMAL_ROLECODE, Category = "APP",IsSystem = true,Description = "APP端普通角色", Belong= SystemCategory.App},
            };

            // 直接使用条件查询已存在的项
            var existingrole = await _fdroleRepository.GetListAsync(m => _entitys.Select(c => c.Code).Contains(m.Code));
            var existingRoleIds = existingrole.Select(m => m.Code).ToHashSet();

            // 只插入不存在的项
            var RoleToInsert = _entitys.Where(c => !existingIds.Contains(c.Code)).ToList();

            if (ToInsert.Any())
            {
                await _fdroleRepository.InsertRangeAsync(RoleToInsert);
            }
            var userrole = await _UserRoleRepository.GetFirstAsync(x => x.AppUserId == entitys.FirstOrDefault().Id);
            if (userrole == null)
            {
                var temp = _fdroleRepository.GetFirstAsync(x => x.Code == APP_NOMAL_ROLECODE).GetAwaiter().GetResult();
                await _UserRoleRepository.InsertAsync(new FdAppUserRole { RoleId = temp.Id, AppUserId = entitys.FirstOrDefault().Id });
            }
        }
    }
}
