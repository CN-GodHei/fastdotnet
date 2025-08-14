using AutoMapper;
using Fastdotnet.Core.Controllers;
using Fastdotnet.Core.Entities.Admin;
using Fastdotnet.Core.Entities.App;
using Fastdotnet.Core.Entities.System;
using Fastdotnet.Core.Exceptions;
using Fastdotnet.Core.IService;
using Fastdotnet.Core.Models.System;
using Fastdotnet.Core.Utils; // 添加对雪花ID生成器的引用
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Fastdotnet.WebApi.Controllers.Admin
{
    [ApiController]
    [Route("api/admin/roles")]
    public class RolesController : GenericDtoControllerBase<FdRole, long, CreateRoleDto, UpdateRoleDto, RoleDto>
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

        protected override async Task BeforeCreate(FdRole entity, CreateRoleDto dto)
        {
            // 使用雪花ID生成唯一的角色编码
            var snowflakeId = SnowflakeIdGenerator.NextId();
            entity.Code = $"ROLE_CODE_{snowflakeId}";

            // 注意：因为雪花ID保证了唯一性，我们不再需要检查Code是否重复

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
        public async Task<List<long>> GetPermissionIds(long id)
        {
            var permissions = await _rolePermissionRepository.GetListAsync(rp => rp.RoleId == id);
            return permissions.Select(p => p.PermissionId).ToList();
        }

        [HttpPost("{id}/permissions")]
        public async Task<IActionResult> AssignPermissions(long id, [FromBody] AssignPermissionsDto dto)
        {
            // 建议：在真实项目中，以下两个操作应在一个数据库事务中执行
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
