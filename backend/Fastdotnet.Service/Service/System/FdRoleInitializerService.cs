using Fastdotnet.Service.IService.Sys;
using System;
using System.Collections.Generic;
using System.Text;

namespace Fastdotnet.Service.Service.System
{
    public class FdRoleInitializerService : IFdRoleInitializerService
    {
        private readonly IRepository<FdRole> _Repository;

        public FdRoleInitializerService(IRepository<FdRole> Repository)
        {
            _Repository = Repository;
        }
        public async Task RoleInitializer()
        {
            var entitys = new List<FdRole>
            {
                new FdRole{Name = "超级管理",Code = "SUPER_ADMIN", IsSystem = true,Description = "拥有系统所有权限", Belong= SystemCategory.Admin},
                new FdRole{Name = "普通管理",Code = "ADMIN_ROLE_001", IsSystem = true,Description = "普通管理",IsDefault=true, Belong= SystemCategory.Admin},
                new FdRole{Name = "默认角色",Code = "APP_ROLE_001", IsSystem = true,Description = "APP端默认角色",IsDefault=true, Belong= SystemCategory.App},
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
