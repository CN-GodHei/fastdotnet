using Fastdotnet.Core.Entities.Sys;
using Fastdotnet.Core.Service.Sys;
using Fastdotnet.Service.IService.Sys;
using System;
using System.Collections.Generic;
using System.Text;

namespace Fastdotnet.Service.Initializers
{
    public class FdRoleMenuInitializer : IApplicationInitializer
    //: IFdRoleInitializerService
    {
        private readonly IRepository<FdRoleMenu> _Repository;
        private readonly IRepository<FdRole> _RoleRepository;
        private readonly IRepository<FdMenu> _MenuRepository;

        public FdRoleMenuInitializer(IRepository<FdRoleMenu> Repository, IRepository<FdRole> roleRepository, IRepository<FdMenu> menuRepository)
        {
            _Repository = Repository;
            _RoleRepository = roleRepository;
            _MenuRepository = menuRepository;
        }
        public int Order = 1050;
        public async Task InitializeAsync()
        {
            // 直接使用条件查询已存在的项
            var DefaultRole = await _RoleRepository.GetFirstAsync(w => w.IsDefault == true && w.Belong == SystemCategory.App);
            var Menu = await _MenuRepository.GetFirstAsync(w => w.Code == "MENU_CODE_11366281228129285");
            FdRoleMenu fdRoleMenu = new FdRoleMenu()
            {
                MenuId = Menu.Id,
                RoleId = DefaultRole.Id,
            };
            var existing = await _Repository.GetFirstAsync(w => w.MenuId == fdRoleMenu.MenuId && w.RoleId == fdRoleMenu.RoleId);
            if (existing == null)
            {
                _ = await _Repository.InsertAsync(fdRoleMenu);
            }
        }
    }
}
