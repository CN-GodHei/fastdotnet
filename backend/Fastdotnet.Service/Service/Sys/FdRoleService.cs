using Fastdotnet.Core.Dtos.Sys;
using Fastdotnet.Core.Entities.Sys;
using Fastdotnet.Core.Enum;
using Fastdotnet.Core.IService;
using Fastdotnet.Service.IService.Sys;

namespace Fastdotnet.Service.Service.Sys
{
    /// <summary>
    /// 角色管理服务实现
    /// </summary>
    public class FdRoleService : IFdRoleService
    {
        private readonly IRepository<FdAdminUserRole> _adminUserRoleRepository;
        private readonly IRepository<FdAppUserRole> _appUserRoleRepository;
        private readonly IRepository<FdRolePermission> _rolePermissionRepository;
        private readonly IRepository<FdRoleMenu> _roleMenuRepository;
        private readonly IRepository<FdRoleMenuButton> _roleMenuButtonRepository;
        private readonly IUnitOfWork _unitOfWork;

        public FdRoleService(
            IRepository<FdAdminUserRole> adminUserRoleRepository,
            IRepository<FdAppUserRole> appUserRoleRepository,
            IRepository<FdRolePermission> rolePermissionRepository,
            IRepository<FdRoleMenu> roleMenuRepository,
            IRepository<FdRoleMenuButton> roleMenuButtonRepository,
            IUnitOfWork unitOfWork)
        {
            _adminUserRoleRepository = adminUserRoleRepository;
            _appUserRoleRepository = appUserRoleRepository;
            _rolePermissionRepository = rolePermissionRepository;
            _roleMenuRepository = roleMenuRepository;
            _roleMenuButtonRepository = roleMenuButtonRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task ValidateBeforeDeleteAsync(FdRole entity)
        {
            if (entity.IsSystem)
            {
                throw new BusinessException($"系统角色 '{entity.Name}' 不允许删除。");
            }

            bool inUse = entity.Belong == SystemCategory.Admin
                ? await _adminUserRoleRepository.ExistsAsync(ur => ur.RoleId == entity.Id)
                : await _appUserRoleRepository.ExistsAsync(ur => ur.RoleId == entity.Id);

            if (inUse)
            {
                throw new BusinessException($"角色 '{entity.Name}' 已分配给用户，无法删除。");
            }
        }

        public async Task<bool> IsRoleAssignedToUsersAsync(string roleId, SystemCategory belong)
        {
            return belong == SystemCategory.Admin
                ? await _adminUserRoleRepository.ExistsAsync(ur => ur.RoleId == roleId)
                : await _appUserRoleRepository.ExistsAsync(ur => ur.RoleId == roleId);
        }

        public async Task<List<string>> GetRolePermissionIdsAsync(string roleId)
        {
            var permissions = await _rolePermissionRepository.GetListAsync(rp => rp.RoleId == roleId);
            return permissions.Select(p => p.PermissionId).ToList();
        }

        public async Task AssignPermissionsAsync(string roleId, AssignPermissionsDto dto)
        {
            await _rolePermissionRepository.DeleteAsync(rp => rp.RoleId == roleId);

            if (dto.PermissionIds != null && dto.PermissionIds.Any())
            {
                var newPermissions = dto.PermissionIds.Select(pid => new FdRolePermission
                {
                    RoleId = roleId,
                    PermissionId = pid
                }).ToList();
                await _rolePermissionRepository.InsertRangeAsync(newPermissions);
            }
        }

        public async Task SaveMenuButtonPermissionsAsync(string roleId, List<MenuBtnRe> menuBtnList)
        {
            await _unitOfWork.BeginTransactionAsync();

            try
            {
                // 清除角色现有的菜单和按钮权限
                await _roleMenuRepository.DeleteAsync(rm => rm.RoleId == roleId);
                await _roleMenuButtonRepository.DeleteAsync(rmb => rmb.RoleId == roleId);

                // 处理菜单和按钮分配
                await ProcessMenuBtnRe(roleId, menuBtnList);

                await _unitOfWork.CommitAsync();
            }
            catch
            {
                await _unitOfWork.RollbackAsync();
                throw;
            }
        }

        private async Task ProcessMenuBtnRe(string roleId, List<MenuBtnRe> menuBtnList)
        {
            var roleMenus = new List<FdRoleMenu>();
            var roleMenuButtons = new List<FdRoleMenuButton>();

            foreach (var menuBtn in menuBtnList)
            {
                // 根据菜单的 DataStatus 决定如何处理菜单权限
                switch (menuBtn.DataStatus)
                {
                    case DataStatus.Added:
                        roleMenus.Add(new FdRoleMenu
                        {
                            RoleId = roleId,
                            MenuId = menuBtn.Id
                        });
                        break;
                    case DataStatus.Deleted:
                        break;
                    case DataStatus.Modified:
                        roleMenus.Add(new FdRoleMenu
                        {
                            RoleId = roleId,
                            MenuId = menuBtn.Id
                        });
                        break;
                    case DataStatus.NoChange:
                        break;
                }

                // 根据按钮的 DataStatus 处理按钮权限
                if (menuBtn.BtnList != null)
                {
                    foreach (var btn in menuBtn.BtnList)
                    {
                        switch (btn.DataStatus)
                        {
                            case DataStatus.Added:
                                roleMenuButtons.Add(new FdRoleMenuButton
                                {
                                    RoleId = roleId,
                                    MenuButtonId = btn.Id
                                });
                                break;
                            case DataStatus.Deleted:
                                break;
                            case DataStatus.Modified:
                                roleMenuButtons.Add(new FdRoleMenuButton
                                {
                                    RoleId = roleId,
                                    MenuButtonId = btn.Id
                                });
                                break;
                            case DataStatus.NoChange:
                                if (btn.Exist)
                                {
                                    roleMenuButtons.Add(new FdRoleMenuButton
                                    {
                                        RoleId = roleId,
                                        MenuButtonId = btn.Id
                                    });
                                }
                                break;
                        }
                    }
                }

                // 处理子菜单
                if (menuBtn.Children != null)
                {
                    await ProcessMenuBtnRe(roleId, menuBtn.Children);
                }
            }

            if (roleMenus.Count > 0)
            {
                await _roleMenuRepository.InsertRangeAsync(roleMenus);
            }

            if (roleMenuButtons.Count > 0)
            {
                await _roleMenuButtonRepository.InsertRangeAsync(roleMenuButtons);
            }
        }
    }
}

