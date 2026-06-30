using Fastdotnet.Core.IService;
using Fastdotnet.Core.Entities.Sys;
using Fastdotnet.Service.IService.Sys;

namespace Fastdotnet.Service.Service.Admin
{
    public class AdminUserService : IAdminUserService
    {
        private readonly IRepository<FdAdminUser> _repository;
        private readonly IRepository<FdAdminUserRole> _adminUserRoleRepository;
        private readonly IBaseService<FdRole> _roleService;
        private readonly IBaseService<FdMenuButton> _menuButtonService;
        private readonly IBaseService<FdRoleMenuButton> _roleMenuButtonService;
        private readonly IPasswordService _passwordService;

        public AdminUserService(
            IRepository<FdAdminUser> repository, 
            IRepository<FdAdminUserRole> adminUserRoleRepository,
            IBaseService<FdRole> roleService,
            IBaseService<FdMenuButton> menuButtonService,
            IBaseService<FdRoleMenuButton> roleMenuButtonService,
            IPasswordService passwordService)
        {
            _repository = repository;
            _adminUserRoleRepository = adminUserRoleRepository;
            _roleService = roleService;
            _menuButtonService = menuButtonService;
            _roleMenuButtonService = roleMenuButtonService;
            _passwordService = passwordService;
        }

        public async Task<string> CreateAsync(CreateFdAdminUserDto dto)
        {
            var existingUser = await _repository.GetFirstAsync(u => u.Username == dto.Username);
            if (existingUser != null)
            { 
                throw new BusinessException("用户名已存在");
            }

            var user = dto.Adapt<FdAdminUser>();
            // 在实际项目中，密码应该在这里进行加密处理
            // user.Password = PasswordHasher.Hash(dto.Password);

            await _repository.InsertAsync(user);
            return user.Id;
        }

        public async Task DeleteAsync(string id)
        {
            var user = await _repository.GetByIdAsync(id);
            if (user == null)
            {
                throw new BusinessException("用户不存在");
            }
            await _repository.DeleteAsync(id);
        }

        public async Task<FdAdminUserDto?> GetAsync(string id)
        {
            var user = await _repository.GetByIdAsync(id);
            return user.Adapt<FdAdminUserDto?>();
        }

        public async Task<PageResult<FdAdminUserDto>> GetPageAsync(PageQueryDto query)
        {
            var pageResult = await _repository.GetPageAsync(
                u => string.IsNullOrEmpty(query.Keyword) || u.Username.Contains(query.Keyword) || u.Name.Contains(query.Keyword),
                query.PageIndex,
                query.PageSize
            );

            return new PageResult<FdAdminUserDto>
            {
                Items = pageResult.Items.Adapt<IList<FdAdminUserDto>>(),
                PageInfo = pageResult.PageInfo
            };
        }

        public async Task ResetPasswordAsync(string id)
        {
            var user = await _repository.GetByIdAsync(id);
            if (user == null)
            {
                throw new BusinessException("用户不存在");
            }

            // 使用系统配置的默认密码进行加密
            var encryptedPassword = await _passwordService.GetDefaultEncryptedPasswordAsync();
            user.Password = encryptedPassword;

            await _repository.UpdateAsync(user);

        }

        public async Task UpdateAsync(string id, UpdateFdAdminUserDto dto)
        {
            var user = await _repository.GetByIdAsync(id);
            if (user == null)
            { 
                throw new BusinessException("用户不存在");
            }

            dto.Adapt(user);
            await _repository.UpdateAsync(user);
        }
        
        public async Task<bool> IsSuperAdminAsync(string userId)
        {
            // 获取用户的角色
            var userRoles = await _adminUserRoleRepository.GetListAsync(ur => ur.AdminUserId == userId);
            var roleIds = userRoles.Select(ur => ur.RoleId).ToList();

            if (!roleIds.Any())
            {
                return false;
            }

            // 获取角色信息
            var roles = await _roleService.GetListAsync(r => roleIds.Contains(r.Id));

            // 检查是否包含超管角色
            return roles.Any(r => r.Code == SystemConstants.SuperAdminRoleCode);
        }
        
        public async Task<List<FdAdminUserRole>> GetUserRoleRelationsAsync(string userId)
        {
            //return await _adminUserRoleRepository.GetListAsync(ur => ur.AdminUserId == userId);
            var userExistRole = await _adminUserRoleRepository.GetListAsync(ur => ur.AdminUserId == userId);
            var DefaultRole = await _roleService.GetListAsync(r => r.IsDefault && r.Belong == SystemCategory.Admin);
            return new List<FdAdminUserRole> {
                new FdAdminUserRole { AdminUserId = userId, RoleId = DefaultRole.FirstOrDefault()?.Id ?? string.Empty }
            }.Union(userExistRole).ToList();
        }
        
        public async Task<List<string>> GetUserButtonPermissionsAsync(string userId)
        {
            if (await IsSuperAdminAsync(userId))
            {
                var menuButtons = await _menuButtonService.GetListAsync(x=>true);

                // 4. 返回按钮权限码列表
                return menuButtons.Select(mb => mb.Code).ToList();
            }
            else
            {

            // 1. 获取用户的角色
            var userRoles = await _adminUserRoleRepository.GetListAsync(ur => ur.AdminUserId == userId);
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

        }
    }
}