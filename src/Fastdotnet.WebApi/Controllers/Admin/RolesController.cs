using AutoMapper;
using Fastdotnet.Core.Constants;
using Fastdotnet.Core.Controllers;
using Fastdotnet.Core.Entities.Admin;
using Fastdotnet.Core.Entities.App;
using Fastdotnet.Core.Entities.System;
using Fastdotnet.Core.Exceptions;
using Fastdotnet.Core.IService;
using Fastdotnet.Core.Models.System;
using Fastdotnet.Core.Utils;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Fastdotnet.WebApi.Controllers.Admin
{
    [ApiController]
    [Route("api/admin/roles")]
    [Authorize]
    public class RolesController : GenericDtoControllerBase<FdRole, string, CreateRoleDto, UpdateRoleDto, RoleDto>
    {
        private readonly IRepository<FdAdminUserRole> _adminUserRoleRepository;
        private readonly IRepository<FdAppUserRole> _appUserRoleRepository;
        private readonly IRepository<FdRolePermission> _rolePermissionRepository;

        public RolesController(
            IRepository<FdRole> repository,
            IMapper mapper,
            IRepository<FdAdminUserRole> adminUserRoleRepository,
            IRepository<FdAppUserRole> appUserRoleRepository,
            IRepository<FdRolePermission> rolePermissionRepository) : base(repository, mapper)
        {
            _adminUserRoleRepository = adminUserRoleRepository;
            _appUserRoleRepository = appUserRoleRepository;
            _rolePermissionRepository = rolePermissionRepository;
        }

        [Authorize(Policy = Permissions.Admin.Roles.View)]
        public override Task<System.Collections.Generic.List<RoleDto>> GetAll() => base.GetAll();

        [Authorize(Policy = Permissions.Admin.Roles.View)]
        public override Task<RoleDto> GetById(string id) => base.GetById(id);

        [Authorize(Policy = Permissions.Admin.Roles.View)]
        public override Task<Fastdotnet.Core.Models.PageResult<RoleDto>> GetPage([FromQuery] int pageIndex = 1, [FromQuery] int pageSize = 10) => base.GetPage(pageIndex, pageSize);

        [Authorize(Policy = Permissions.Admin.Roles.Create)]
        public override Task<RoleDto> Create(CreateRoleDto dto) => base.Create(dto);

        [Authorize(Policy = Permissions.Admin.Roles.Edit)]
        public override Task<RoleDto> Update(string id, UpdateRoleDto dto) => base.Update(id, dto);

        [Authorize(Policy = Permissions.Admin.Roles.Delete)]
        public override Task<bool> Delete(string id) => base.Delete(id);

        protected override async Task BeforeCreate(FdRole entity, CreateRoleDto dto)
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
        public async Task<IActionResult> AssignPermissions(string id, [FromBody] AssignPermissionsDto dto)
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
            return NoContent();
        }
    }
}