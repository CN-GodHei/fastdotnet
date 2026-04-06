using Fastdotnet.Core.Entities.Sys;
using Fastdotnet.Service.IService.Sys;
using System;
using System.Collections.Generic;
using System.Text;

namespace Fastdotnet.Service.Initializers
{
    public class FdRoleInitializer : IApplicationInitializer
    //: IFdRoleInitializerService
    {
        private readonly IRepository<FdRole> _Repository;

        public FdRoleInitializer(IRepository<FdRole> Repository)
        {
            _Repository = Repository;
        }
        public int Order = 1000;
        public async Task InitializeAsync()
        {
            var entitys = new List<FdRole>
            {
                new FdRole{Name = "超级管理",Code = "SUPER_ADMIN", IsSystem = true,Description = "拥有系统所有权限", Belong= SystemCategory.Admin},
                new FdRole{Name = "普通管理",Code = "ADMIN_ROLE_001", IsSystem = true,Description = "默认管理(主要是管理能新加的管理员正常登录，具体的功能还是需要自己加角色)",IsDefault=true, Belong= SystemCategory.Admin},
                new FdRole{Name = "默认角色",Code = "APP_ROLE_001", IsSystem = true,Description = "APP端默认角色(主要是使新加用户能正常登录，具体的功能还是需要自己加角色)",IsDefault=true, Belong= SystemCategory.App},
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
