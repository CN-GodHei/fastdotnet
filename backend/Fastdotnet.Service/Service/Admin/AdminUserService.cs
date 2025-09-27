using AutoMapper;
using Fastdotnet.Core.Entities.Admin;
using Fastdotnet.Core.Exceptions;
using Fastdotnet.Core.IService;
using Fastdotnet.Core.Models;
using Fastdotnet.Core.Models.Admin.Users;
using Fastdotnet.Service.IService.Admin;
using System.Threading.Tasks;
using Fastdotnet.Core.Entities.System;
using System.Linq;
using Fastdotnet.Core.Constants;

namespace Fastdotnet.Service.Service.Admin
{
    public class AdminUserService : IAdminUserService
    {
        private readonly IRepository<FdAdminUser> _repository;
        private readonly IMapper _mapper;
        private readonly IRepository<FdAdminUserRole> _adminUserRoleRepository;
        private readonly IRepository<FdRole> _roleRepository;
        private readonly IRepository<FdMenuButton> _menuButtonRepository;
        private readonly IRepository<FdRoleMenuButton> _roleMenuButtonRepository;

        public AdminUserService(
            IRepository<FdAdminUser> repository, 
            IMapper mapper,
            IRepository<FdAdminUserRole> adminUserRoleRepository,
            IRepository<FdRole> roleRepository,
            IRepository<FdMenuButton> menuButtonRepository,
            IRepository<FdRoleMenuButton> roleMenuButtonRepository)
        {
            _repository = repository;
            _mapper = mapper;
            _adminUserRoleRepository = adminUserRoleRepository;
            _roleRepository = roleRepository;
            _menuButtonRepository = menuButtonRepository;
            _roleMenuButtonRepository = roleMenuButtonRepository;
        }

        public async Task<string> CreateAsync(CreateAdminUserDto dto)
        {
            var existingUser = await _repository.GetFirstAsync(u => u.Username == dto.Username);
            if (existingUser != null)
            { 
                throw new BusinessException("用户名已存在");
            }

            var user = _mapper.Map<FdAdminUser>(dto);
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

        public async Task<AdminUserDto?> GetAsync(string id)
        {
            var user = await _repository.GetByIdAsync(id);
            return _mapper.Map<AdminUserDto?>(user);
        }

        public async Task<PageResult<AdminUserDto>> GetPageAsync(PageQueryDto query)
        {
            var pageResult = await _repository.GetPageAsync(
                u => string.IsNullOrEmpty(query.Keyword) || u.Username.Contains(query.Keyword) || u.Name.Contains(query.Keyword),
                query.PageIndex,
                query.PageSize
            );

            return new PageResult<AdminUserDto>
            {
                Items = _mapper.Map<IList<AdminUserDto>>(pageResult.Items),
                PageInfo = pageResult.PageInfo
            };
        }

        public async Task ResetPasswordAsync(string id, string newPassword)
        {
            var user = await _repository.GetByIdAsync(id);
            if (user == null)
            {
                throw new BusinessException("用户不存在");
            }

            // 在实际项目中，密码应该在这里进行加密处理
            // user.Password = PasswordHasher.Hash(newPassword);
            user.Password = newPassword; // 临时明文处理

            await _repository.UpdateAsync(user);
        }

        public async Task UpdateAsync(string id, UpdateAdminUserDto dto)
        {
            var user = await _repository.GetByIdAsync(id);
            if (user == null)
            { 
                throw new BusinessException("用户不存在");
            }

            _mapper.Map(dto, user);
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
            var roles = await _roleRepository.GetListAsync(r => roleIds.Contains(r.Id));

            // 检查是否包含超管角色
            return roles.Any(r => r.Code == SystemConstants.SuperAdminRoleCode);
        }
        
        public async Task<List<FdAdminUserRole>> GetUserRoleRelationsAsync(string userId)
        {
            return await _adminUserRoleRepository.GetListAsync(ur => ur.AdminUserId == userId);
        }
        
        public async Task<List<string>> GetUserButtonPermissionsAsync(string userId)
        {
            // 1. 获取用户的角色
            var userRoles = await _adminUserRoleRepository.GetListAsync(ur => ur.AdminUserId == userId);
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
    }
}