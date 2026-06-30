using Fastdotnet.Core.IService;
using Fastdotnet.Core.Entities.Sys;
using Fastdotnet.Service.IService.App;
using Fastdotnet.Service.IService.Sys;

namespace Fastdotnet.Service.Service.App
{
    public class AppUserService : IAppUserService
    {
        private readonly IRepository<FdAppUser> _appUserRepository;
        private readonly IRepository<FdAppUserRole> _appUserRoleRepository;
        private readonly IBaseService<FdRole> _roleService;
        private readonly IBaseService<FdMenuButton> _menuButtonService;
        private readonly IBaseService<FdRoleMenuButton> _roleMenuButtonService;
        private readonly IPasswordService _passwordService;

        public AppUserService(
            IRepository<FdAppUser> appUserRepository,
            IRepository<FdAppUserRole> appUserRoleRepository,
            IBaseService<FdRole> roleService,
            IBaseService<FdMenuButton> menuButtonService,
            IBaseService<FdRoleMenuButton> roleMenuButtonService,
            IPasswordService passwordService
            )
        {
            _appUserRepository = appUserRepository;
            _appUserRoleRepository = appUserRoleRepository;
            _roleService = roleService;
            _menuButtonService = menuButtonService;
            _roleMenuButtonService = roleMenuButtonService;
            _passwordService = passwordService;
        }
        public async Task<List<string>> GetUserButtonPermissionsAsync(string userId)
        {
            // 1. 获取用户的角色
            var userRoles = await GetUserRoleRelationsAsync(userId);
            var roleIds = userRoles.Select(ur => ur.RoleId).ToList();

            if (!roleIds.Any()) return new List<string>();

            // 2. 获取角色分配的菜单按钮权限
            var roleMenuButtons = await _roleMenuButtonService.GetListAsync(rmb => roleIds.Contains(rmb.RoleId));

            if (!roleMenuButtons.Any()) return new List<string>();

            // 3. 获取具体的菜单按钮信息
            var menuButtonIds = roleMenuButtons.Select(rmb => rmb.MenuButtonId).ToList();
            var menuButtons = await _menuButtonService.GetListAsync(mb => menuButtonIds.Contains(mb.Id));

            // 4. 返回按钮权限码列表
            return menuButtons.Select(mb => mb.Code).ToList();
        }

        public async Task<List<FdAppUserRole>> GetUserRoleRelationsAsync(string userId)
        {
            var userExistRole = await _appUserRoleRepository.GetListAsync(ur => ur.AppUserId == userId);
            var DefaultRole = await _roleService.GetListAsync(r => r.IsDefault && r.Belong== SystemCategory.App);
            return new List<FdAppUserRole> {
                new FdAppUserRole { AppUserId = userId, RoleId = DefaultRole.FirstOrDefault()?.Id }
            }.Union(userExistRole).ToList();
        }

        public async Task ResetPasswordAsync(string userId)
        {
            var user = await _appUserRepository.GetByIdAsync(userId);
            if (user == null)
            {
                throw new BusinessException("用户不存在");
            }

            // 使用系统配置的默认密码进行加密
            var encryptedPassword = await _passwordService.GetDefaultEncryptedPasswordAsync();
            user.Password = encryptedPassword;

            await _appUserRepository.UpdateAsync(user);
        }
    }
}
