using AutoMapper;
using Fastdotnet.Core.Constants;
using Fastdotnet.Core.Controllers;
using Fastdotnet.Core.Entities.System;
using Fastdotnet.Core.IService;
using Fastdotnet.Core.Models.System;
using Fastdotnet.Service.IService;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;
using Fastdotnet.Core.Utils;
using Fastdotnet.Core.Entities.Admin;
using System.Linq;
using System;
using Fastdotnet.Service.IService.Admin;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;

namespace Fastdotnet.WebApi.Controllers.Admin
{
    [ApiController]
    [Route("api/admin/menus")]
    [Authorize]
    public class MenusController : GenericDtoControllerBase<FdMenu, string, CreateMenuDto, UpdateMenuDto, MenuDto>
    {
        private readonly IMenuService _menuService;
        private readonly ICurrentUser _currentUser;
        private readonly IRepository<FdRole> _roleRepository;
        private readonly IAdminUserService _adminUserService;

        public MenusController(
            IRepository<FdMenu> repository,
            IMapper mapper,
            IMenuService menuService,
            ICurrentUser currentUser,
            IRepository<FdAdminUserRole> adminUserRoleRepository,
            IAdminUserService adminUserService,
            IRepository<FdRole> roleRepository) : base(repository, mapper)
        {
            _menuService = menuService;
            _currentUser = currentUser;
            _adminUserService = adminUserService;
            _roleRepository = roleRepository;
        }

        [HttpGet("tree")]
        [Authorize(Policy = Permissions.Admin.Menus.View)]
        public async Task<IActionResult> GetUserMenuTree()
        {
            var userId = _currentUser.Id;
            if (userId == null)
            {
                return Unauthorized();
            }

            // 判断是否为超管角色
            bool isSuperAdmin = await _adminUserService.IsSuperAdminAsync(userId);
            List<FdMenu> menus;
            
            if (isSuperAdmin)
            {
                // 超管获取所有菜单
                menus = await _repository.GetListAsync(m => m.Category == "Admin" && m.IsEnabled);
                JsonConvert.SerializeObject(menus, Formatting.Indented);
                //将menus 转为 JSON 字符串输出
                var menusJson = JsonConvert.SerializeObject(menus, Formatting.Indented);


                var ss = menus.ToString();
                menus = BuildMenuTree(menus, null);
                var menusJson1 = JsonConvert.SerializeObject(menus, Formatting.Indented);

            }
            else
            {
                // 普通用户根据权限获取菜单
                menus = await _menuService.GetUserMenusAsync(userId, "Admin");
            }
            
            return Ok(menus);
        }

        private List<FdMenu> BuildMenuTree(List<FdMenu> allMenus, string? parentCode)
        {
            return allMenus
                .Where(m => m.ParentCode == parentCode)
                .OrderBy(m => m.Sort)
                .Select(m => new FdMenu
                {
                    Id = m.Id,
                    Name = m.Name,
                    Code = m.Code,
                    Path = m.Path,
                    Icon = m.Icon,
                    ParentCode = m.ParentCode,
                    Sort = m.Sort,
                    Type = m.Type,
                    Module = m.Module,
                    Category = m.Category,
                    IsExternal = m.IsExternal,
                    ExternalUrl = m.ExternalUrl,
                    IsEnabled = m.IsEnabled,
                    PermissionCode = m.PermissionCode,
                    Children = BuildMenuTree(allMenus, m.Code)
                })
                .ToList();
        }

        [Authorize(Policy = Permissions.Admin.Menus.View)]
        public override Task<List<MenuDto>> GetAll() => base.GetAll();

        [Authorize(Policy = Permissions.Admin.Menus.View)]
        public override Task<MenuDto> GetById(string id) => base.GetById(id);

        [Authorize(Policy = Permissions.Admin.Menus.View)]
        public override Task<Fastdotnet.Core.Models.PageResult<MenuDto>> GetPage([FromQuery] int pageIndex = 1, [FromQuery] int pageSize = 10) => base.GetPage(pageIndex, pageSize);

        [Authorize(Policy = Permissions.Admin.Menus.Create)]
        public override Task<MenuDto> Create(CreateMenuDto dto) => base.Create(dto);

        [Authorize(Policy = Permissions.Admin.Menus.Edit)]
        public override Task<MenuDto> Update(string id, UpdateMenuDto dto) => base.Update(id, dto);

        [Authorize(Policy = Permissions.Admin.Menus.Delete)]
        public override Task<bool> Delete(string id) => base.Delete(id);
        
        protected override async Task BeforeCreate(FdMenu entity, CreateMenuDto dto)
        {
            var generatedCode = $"MENU_CODE_{SnowflakeIdGenerator.NextStrId()}";
            entity.Code = generatedCode;
            //entity.ParentCode = entity.ParentCode == 0 ? null : entity.ParentCode;
            await base.BeforeCreate(entity, dto);
        }
        
        protected override async Task BeforeUpdate(FdMenu existingEntity, FdMenu updatedEntity, UpdateMenuDto dto)
        {
            // Code字段由系统自动生成，不允许修改
            updatedEntity.Code = existingEntity.Code;
            await base.BeforeUpdate(existingEntity, updatedEntity, dto);
        }
    }
}