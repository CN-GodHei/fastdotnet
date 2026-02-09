using System;
using System.Collections.Generic;
using System.Text;

namespace Fastdotnet.Service.App
{
    public class AppUserService : IAppUserService
    {
        private readonly IRepository<FdRole> _roleRepository;
        private readonly IRepository<FdAppUserRole> _AppUserRoleRepository;
        private readonly IRepository<FdMenuButton> _menuButtonRepository;
        private readonly IRepository<FdRoleMenuButton> _roleMenuButtonRepository;

        public AppUserService(IRepository<FdAppUserRole> AppUserRoleRepository,
            IRepository<FdRole> roleRepository,
            IRepository<FdMenuButton> menuButtonRepository,
            IRepository<FdRoleMenuButton> roleMenuButtonRepository
            )
        {
            _AppUserRoleRepository = AppUserRoleRepository;
            _roleRepository = roleRepository;
            _menuButtonRepository = menuButtonRepository;
            _roleMenuButtonRepository = roleMenuButtonRepository;
        }
        public async Task<List<string>> GetUserButtonPermissionsAsync(string userId)
        {
            // 1. 获取用户的角色
            var userRoles = await GetUserRoleRelationsAsync(userId);
            var roleIds = userRoles.Select(ur => ur.RoleId).ToList();

            if (!roleIds.Any()) return new List<string>();

            // 2. 获取角色分配的菜单按钮权限
            var roleMenuButtons = await _roleMenuButtonRepository.GetListAsync(rmb => roleIds.Contains(rmb.RoleId));

            if (!roleMenuButtons.Any()) return new List<string>();

            // 3. 获取具体的菜单按钮信息
            var menuButtonIds = roleMenuButtons.Select(rmb => rmb.MenuButtonId).ToList();
            var menuButtons = await _menuButtonRepository.GetListAsync(mb => menuButtonIds.Contains(mb.Id));

            // 4. 返回按钮权限码列表
            return menuButtons.Select(mb => mb.Code).ToList();
        }

        public async Task<List<FdAppUserRole>> GetUserRoleRelationsAsync(string userId)
        {
            var userExistRole = await _AppUserRoleRepository.GetListAsync(ur => ur.AppUserId == userId);
            var DefaultRole = await _roleRepository.GetListAsync(r => r.IsDefault && r.Belong== SystemCategory.App);
            return new List<FdAppUserRole> {
                new FdAppUserRole { AppUserId = userId, RoleId = DefaultRole.FirstOrDefault()?.Id }
            }.Union(userExistRole).ToList();
        }
    }
}
