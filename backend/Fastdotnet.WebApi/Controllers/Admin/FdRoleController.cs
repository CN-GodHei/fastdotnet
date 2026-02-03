using Fastdotnet.Core.Dtos;
using Fastdotnet.Core.Enum;

namespace Fastdotnet.WebApi.Controllers.Admin
{
    [ApiController]
    [Route("api/admin/[Controller]")]
    [Authorize]
    public class FdRoleController : GenericDtoControllerBase<FdRole, string, CreateFdRoleDto, UpdateFdRoleDto, FdRoleDto>
    {
        private readonly IRepository<FdAdminUserRole> _adminUserRoleRepository;
        private readonly IRepository<FdAppUserRole> _appUserRoleRepository;
        private readonly IRepository<FdRolePermission> _rolePermissionRepository;
        private readonly IRepository<FdRoleMenu> _roleMenuRepository;
        private readonly IRepository<FdRoleMenuButton> _roleMenuButtonRepository;
        private readonly IUnitOfWork _unitOfWork;

        public FdRoleController(
            IBaseService<FdRole, string> service,
            IMapper mapper,
            IRepository<FdAdminUserRole> adminUserRoleRepository,
            IRepository<FdAppUserRole> appUserRoleRepository,
            IRepository<FdRolePermission> rolePermissionRepository,
            IRepository<FdRoleMenu> roleMenuRepository,
            IRepository<FdRoleMenuButton> roleMenuButtonRepository,
            IUnitOfWork unitOfWork) : base(service, mapper)
        {
            _adminUserRoleRepository = adminUserRoleRepository;
            _appUserRoleRepository = appUserRoleRepository;
            _rolePermissionRepository = rolePermissionRepository;
            _roleMenuRepository = roleMenuRepository;
            _roleMenuButtonRepository = roleMenuButtonRepository;
            _unitOfWork = unitOfWork;
        }

        [Authorize(Policy = Permissions.Admin.Roles.View)]
        public override Task<List<FdRoleDto>> GetAll( CancellationToken cancellationToken = default) => base.GetAll();

        [Authorize(Policy = Permissions.Admin.Roles.View)]
        public override Task<FdRoleDto> GetById(string id, CancellationToken cancellationToken = default) => base.GetById(id);

        [Authorize(Policy = Permissions.Admin.Roles.View)]
        public override Task<PageResult<FdRoleDto>> GetPage([FromQuery] int pageIndex = 1, [FromQuery] int pageSize = 10, CancellationToken cancellationToken = default) => base.GetPage(pageIndex, pageSize);

        [Authorize(Policy = Permissions.Admin.Roles.Create)]
        public override Task<FdRoleDto> Create(CreateFdRoleDto dto) => base.Create(dto);

        [Authorize(Policy = Permissions.Admin.Roles.Edit)]
        public override Task<FdRoleDto> Update(string id, UpdateFdRoleDto dto) => base.Update(id, dto);

        [Authorize(Policy = Permissions.Admin.Roles.Delete)]
        public override Task<bool> Delete(string id) => base.Delete(id);

        protected override async Task BeforeCreate(FdRole entity, CreateFdRoleDto dto)
        {
            var generatedCode = $"ROLE_CODE_{SnowflakeIdGenerator.NextStrId()}";
            entity.Code = generatedCode;
            await base.BeforeCreate(entity, dto);
        }

        protected override async Task BeforeDelete(FdRole entity)
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
            await base.BeforeDelete(entity);
        }

        [HttpGet("{id}/permissions")]
        [Authorize(Policy = Permissions.Admin.Roles.View)]
        public async Task<List<string>> GetPermissionIds(string id)
        {
            var permissions = await _rolePermissionRepository.GetListAsync(rp => rp.RoleId == id);
            return permissions.Select(p => p.PermissionId).ToList();
        }

        [HttpPost("{id}/permissions")]
        [Authorize(Policy = Permissions.Admin.Roles.AssignPermissions)]
        public async Task<bool> AssignPermissions(string id, [FromBody] AssignPermissionsDto dto)
        {
            await _rolePermissionRepository.DeleteAsync(rp => rp.RoleId == id);

            if (dto.PermissionIds != null && dto.PermissionIds.Any())
            {
                var newPermissions = dto.PermissionIds.Select(pid => new FdRolePermission
                {
                    RoleId = id,
                    PermissionId = pid
                }).ToList();
                await _rolePermissionRepository.InsertRangeAsync(newPermissions);
            }
            return true;
        }
        
        [HttpPost("{id}/menu-btns")]
        public async Task<bool> Save(string id, [FromBody] List<MenuBtnRe> menuBtnList)
        {
            // 开始事务 - 使用 SqlSugar 工作单元
            await _unitOfWork.BeginTransactionAsync();
            
            try
            {
                // 清除角色现有的菜单和按钮权限
                await _roleMenuRepository.DeleteAsync(rm => rm.RoleId == id);
                await _roleMenuButtonRepository.DeleteAsync(rmb => rmb.RoleId == id);
                
                // 处理菜单和按钮分配
                await ProcessMenuBtnRe(id, menuBtnList);
                
                await _unitOfWork.CommitAsync();
                return true;
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
                        // 添加新的菜单权限
                        roleMenus.Add(new FdRoleMenu
                        {
                            RoleId = roleId,
                            MenuId = menuBtn.Id
                        });
                        break;
                    case DataStatus.Deleted:
                        // 对于删除状态，暂时不处理（删除可通过其他方式实现）
                        break;
                    case DataStatus.Modified:
                        // 修改状态 - 添加菜单权限
                        roleMenus.Add(new FdRoleMenu
                        {
                            RoleId = roleId,
                            MenuId = menuBtn.Id
                        });
                        break;
                    case DataStatus.NoChange:
                        // 无变化 - 如果存在则添加
                        //if (menuBtn.Exist)
                        //{
                        //    roleMenus.Add(new FdRoleMenu
                        //    {
                        //        RoleId = roleId,
                        //        MenuId = menuBtn.Id
                        //    });
                        //}
                        break;
                }
                
                // 根据按钮的 DataStatus 处理按钮权限
                if (menuBtn.BtnList != null)
                {
                    foreach (var btn in menuBtn.BtnList)
                    {
                        // 根据按钮的 DataStatus 决定如何处理
                        switch(btn.DataStatus)
                        {
                            case DataStatus.Added:
                                // 添加新的按钮权限
                                roleMenuButtons.Add(new FdRoleMenuButton
                                {
                                    RoleId = roleId,
                                    MenuButtonId = btn.Id
                                });
                                break;
                            case DataStatus.Deleted:
                                // 对于删除状态，暂时不处理
                                break;
                            case DataStatus.Modified:
                                // 修改状态 - 添加按钮权限
                                roleMenuButtons.Add(new FdRoleMenuButton
                                {
                                    RoleId = roleId,
                                    MenuButtonId = btn.Id
                                });
                                break;
                            case DataStatus.NoChange:
                                // 无变化 - 如果存在则添加
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
            
            // 插入角色菜单和按钮
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



