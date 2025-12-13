namespace Fastdotnet.WebApi.Controllers.Admin
{
    [ApiController]
    [Route("api/admin/[Controller]")]
    [Authorize]
    public class FdRolesController : GenericDtoControllerBase<FdRole, string, CreateFdRoleDto, UpdateFdRoleDto, FdRoleDto>
    {
        private readonly IRepository<FdAdminUserRole> _adminUserRoleRepository;
        private readonly IRepository<FdAppUserRole> _appUserRoleRepository;
        private readonly IRepository<FdRolePermission> _rolePermissionRepository;

        public FdRolesController(
            IBaseService<FdRole, string> service,
            IMapper mapper,
            IRepository<FdAdminUserRole> adminUserRoleRepository,
            IRepository<FdAppUserRole> appUserRoleRepository,
            IRepository<FdRolePermission> rolePermissionRepository) : base(service, mapper)
        {
            _adminUserRoleRepository = adminUserRoleRepository;
            _appUserRoleRepository = appUserRoleRepository;
            _rolePermissionRepository = rolePermissionRepository;
        }

        [Authorize(Policy = Permissions.Admin.Roles.View)]
        public override Task<List<FdRoleDto>> GetAll( CancellationToken cancellationToken = default) => base.GetAll();

        [Authorize(Policy = Permissions.Admin.Roles.View)]
        public override Task<FdRoleDto> GetById(string id, CancellationToken cancellationToken = default) => base.GetById(id);

        [Authorize(Policy = Permissions.Admin.Roles.View)]
        public override Task<Fastdotnet.Core.Models.PageResult<FdRoleDto>> GetPage([FromQuery] int pageIndex = 1, [FromQuery] int pageSize = 10, CancellationToken cancellationToken = default) => base.GetPage(pageIndex, pageSize);

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

            bool inUse = entity.Category == "Admin"
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
    }
}