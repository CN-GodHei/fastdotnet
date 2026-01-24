using AutoMapper;
using Fastdotnet.Core.Dtos.System;
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
        private readonly IMapper _mapper;
        public MenuService(
            IRepository<FdMenu> menuRepository,
            IRepository<FdRoleMenu> roleMenuRepository,
            IRepository<FdAdminUserRole> adminUserRoleRepository,
            IRepository<FdAppUserRole> appUserRoleRepository,
            IPermissionService permissionService,
            IMapper mapper)
        {
            _menuRepository = menuRepository;
            _roleMenuRepository = roleMenuRepository;
            _adminUserRoleRepository = adminUserRoleRepository;
            _appUserRoleRepository = appUserRoleRepository;
            _permissionService = permissionService;
            _mapper = mapper;
        }

        public async Task<List<FdMenuDto>> GetUserMenusAsync(string userId, string category)
        {
            // 1. Get user's roles
            List<string> roleIds;
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
                return new List<FdMenuDto>();
            }

            // 2. Get menu ids from roles
            var roleMenus = await _roleMenuRepository.GetListAsync(rm => roleIds.Contains(rm.RoleId));
            var menuIds = roleMenus.Select(rm => rm.MenuId).Distinct().ToList();

            if (!menuIds.Any())
            {
                return new List<FdMenuDto>();
            }

            // 3. Get menus
            var allMenus = await _menuRepository.GetListAsync(m => menuIds.Contains(m.Id) && m.Category == category && m.IsEnabled);

            // 4. Filter menus by permission
            //var userPermissions = await _permissionService.GetUserPermissionsAsync(userId, category);
            //var accessibleMenus = allMenus
            //    .Where(m => string.IsNullOrEmpty(m.PermissionCode) || userPermissions.Contains(m.PermissionCode))
            //    .ToList();

            // 5. Build menu tree
            return await BuildMenuTree(allMenus, null);
        }

        public async Task<List<FdMenuDto>> BuildMenuTree(List<FdMenu> allMenus, string? parentCode)
        {
            // 预处理：构建父子关系字典，处理 null ParentCode
            var menuDict = allMenus
                .GroupBy(m => m.ParentCode ?? string.Empty) // 将 null 转换为 ""
                .ToDictionary(g => g.Key, g => g.OrderBy(m => m.Sort).ToList());

            // 递归构建树
            async Task<List<FdMenuDto>> BuildTreeRecursive(string? currentParentCode)
            {
                // 将 null 转换为 "" 以匹配字典中的键
                var key = currentParentCode ?? string.Empty;

                // 快速查找子菜单
                if (!menuDict.TryGetValue(key, out var childMenus))
                {
                    return new List<FdMenuDto>();
                }

                // 使用 AutoMapper 和异步处理
                var tasks = childMenus.Select(async m => {
                    // 使用 AutoMapper 映射基本属性
                    var menuDto = _mapper.Map<FdMenuDto>(m);
                    // 递归构建子菜单
                    menuDto.Children = await BuildTreeRecursive(m.Code);
                    return menuDto;
                });

                // 等待所有任务完成
                var menuDtos = await Task.WhenAll(tasks);
                return menuDtos.ToList();
            }

            return await BuildTreeRecursive(parentCode);
        }

    }
}