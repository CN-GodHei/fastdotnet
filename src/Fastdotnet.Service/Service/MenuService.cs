using Fastdotnet.Core.Entities.Admin;
using Fastdotnet.Core.Entities.App;
using Fastdotnet.Core.Entities.System;
using Fastdotnet.Core.IService;
using Fastdotnet.Service.IService;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Fastdotnet.Service.Service
{
    public class MenuService : IMenuService
    {
        private readonly IRepository<FdMenu> _menuRepository;
        private readonly IRepository<FdRoleMenu> _roleMenuRepository;
        private readonly IRepository<FdAdminUserRole> _adminUserRoleRepository;
        private readonly IRepository<FdAppUserRole> _appUserRoleRepository;
        private readonly IPermissionService _permissionService;

        public MenuService(
            IRepository<FdMenu> menuRepository,
            IRepository<FdRoleMenu> roleMenuRepository,
            IRepository<FdAdminUserRole> adminUserRoleRepository,
            IRepository<FdAppUserRole> appUserRoleRepository,
            IPermissionService permissionService)
        {
            _menuRepository = menuRepository;
            _roleMenuRepository = roleMenuRepository;
            _adminUserRoleRepository = adminUserRoleRepository;
            _appUserRoleRepository = appUserRoleRepository;
            _permissionService = permissionService;
        }

        public async Task<List<FdMenu>> GetUserMenusAsync(long userId, string category)
        {
            // 1. Get user's roles
            List<long> roleIds;
            if (category == "Admin")
            {
                var userRoles = await _adminUserRoleRepository.GetListAsync(ur => ur.AdminUserId == userId);
                roleIds = userRoles.Select(ur => ur.RoleId).ToList();
            }
            else
            {
                var userRoles = await _appUserRoleRepository.GetListAsync(ur => ur.AppUserId == userId);
                roleIds = userRoles.Select(ur => ur.RoleId).ToList();
            }

            if (!roleIds.Any())
            {
                return new List<FdMenu>();
            }

            // 2. Get menu ids from roles
            var roleMenus = await _roleMenuRepository.GetListAsync(rm => roleIds.Contains(rm.RoleId));
            var menuIds = roleMenus.Select(rm => rm.MenuId).Distinct().ToList();

            if (!menuIds.Any())
            {
                return new List<FdMenu>();
            }

            // 3. Get menus
            var allMenus = await _menuRepository.GetListAsync(m => menuIds.Contains(m.Id) && m.Category == category && m.IsEnabled);

            // 4. Filter menus by permission
            var userPermissions = await _permissionService.GetUserPermissionsAsync(userId, category);
            var accessibleMenus = allMenus
                .Where(m => string.IsNullOrEmpty(m.PermissionCode) || userPermissions.Contains(m.PermissionCode))
                .ToList();

            // 5. Build menu tree
            return BuildMenuTree(accessibleMenus, null);
        }

        private List<FdMenu> BuildMenuTree(List<FdMenu> allMenus, long? parentId)
        {
            return allMenus
                .Where(m => m.ParentId == parentId)
                .OrderBy(m => m.Sort)
                .Select(m => new FdMenu
                {
                    // You might need to map other properties as well
                    Id = m.Id,
                    Name = m.Name,
                    Code = m.Code,
                    Path = m.Path,
                    Icon = m.Icon,
                    ParentId = m.ParentId,
                    Sort = m.Sort,
                    Type = m.Type,
                    Module = m.Module,
                    Category = m.Category,
                    IsExternal = m.IsExternal,
                    ExternalUrl = m.ExternalUrl,
                    IsEnabled = m.IsEnabled,
                    PermissionCode = m.PermissionCode,
                    Children = BuildMenuTree(allMenus, m.Id) // Recursive call
                })
                .ToList();
        }
    }
}