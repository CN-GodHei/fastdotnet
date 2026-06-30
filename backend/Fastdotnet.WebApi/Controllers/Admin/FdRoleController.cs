using Fastdotnet.Core.Dtos.Sys;
using Fastdotnet.Core.Entities.Sys;
using Fastdotnet.Service.IService.Sys;

namespace Fastdotnet.WebApi.Controllers.Admin
{
    [ApiController]
    [Route("api/admin/[controller]")]
    [Authorize]
    public class FdRoleController : GenericDtoControllerBase<FdRole, string, CreateFdRoleDto, UpdateFdRoleDto, FdRoleDto>
    {
        private readonly IFdRoleService _fdRoleService;

        public FdRoleController(
            IBaseService<FdRole, string> service,
            IFdRoleService fdRoleService) : base(service)
        {
            _fdRoleService = fdRoleService;
        }

        [Authorize(Policy = Permissions.Admin.Roles.View)]
        public override Task<List<FdRoleDto>> GetAll(CancellationToken cancellationToken = default) => base.GetAll(cancellationToken);

        [Authorize(Policy = Permissions.Admin.Roles.View)]
        public override Task<FdRoleDto> GetById(string id, CancellationToken cancellationToken = default) => base.GetById(id, cancellationToken);

        [Authorize(Policy = Permissions.Admin.Roles.View)]
        public override Task<PageResult<FdRoleDto>> GetPage([FromQuery] int pageIndex = 1, [FromQuery] int pageSize = 10, CancellationToken cancellationToken = default) => base.GetPage(pageIndex, pageSize, cancellationToken);

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
            await _fdRoleService.ValidateBeforeDeleteAsync(entity);
            await base.BeforeDelete(entity);
        }

        [HttpGet("{id}/permissions")]
        [Authorize(Policy = Permissions.Admin.Roles.View)]
        public async Task<List<string>> GetPermissionIds(string id)
        {
            return await _fdRoleService.GetRolePermissionIdsAsync(id);
        }

        [HttpPost("{id}/permissions")]
        [Authorize(Policy = Permissions.Admin.Roles.AssignPermissions)]
        public async Task<bool> AssignPermissions(string id, [FromBody] AssignPermissionsDto dto)
        {
            await _fdRoleService.AssignPermissionsAsync(id, dto);
            return true;
        }
        
        [HttpPost("{id}/menu-btns")]
        public async Task<bool> Save(string id, [FromBody] List<MenuBtnRe> menuBtnList)
        {
            await _fdRoleService.SaveMenuButtonPermissionsAsync(id, menuBtnList);
            return true;
        }
        
    }
}



