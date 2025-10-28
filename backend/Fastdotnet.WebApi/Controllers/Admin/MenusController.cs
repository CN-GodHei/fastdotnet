using AutoMapper;
using Fastdotnet.Core.Constants;
using Fastdotnet.Core.Controllers;
using Fastdotnet.Core.Entities.Admin;
using Fastdotnet.Core.Entities.System;
using Fastdotnet.Core.Exceptions;
using Fastdotnet.Core.IService;
using Fastdotnet.Core.Models.System;
using Fastdotnet.Core.Utils;
using Fastdotnet.Service.IService;
using Fastdotnet.Service.IService.Admin;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

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
        public async Task<List<MenuDto>> GetUserMenuTree()
        {
            var userId = _currentUser.Id;
            if (userId == null)
            {
                //return Unauthorized();
                throw new BusinessException("用户未登录");
            }

            // 判断是否为超管角色
            bool isSuperAdmin = await _adminUserService.IsSuperAdminAsync(userId);
            List<MenuDto> menus;

            if (isSuperAdmin)
            {
                // 超管获取所有菜单
                var menutemp = await _repository.GetListAsync(m => m.Category == "Admin" && m.IsEnabled);
                menus = await _menuService.BuildMenuTree(menutemp, null);
            }
            else
            {
                // 普通用户根据权限获取菜单
                menus = await _menuService.GetUserMenusAsync(userId, "Admin");
            }

            return menus;
        }


        [HttpGet]
        [Authorize(Policy = Permissions.Admin.Menus.View)]
        public override async Task<List<MenuDto>> GetAll()
        {
            // 获取所有菜单
            var menus = await _repository.GetListAsync(m => m.Category == "Admin");
            
            // 构建树形结构
            var menuTree = await _menuService.BuildMenuTree(menus, null);
            
            // 转换为 DTO
            var menuDtos = _mapper.Map<List<MenuDto>>(menuTree);
            
            return menuDtos;
        }

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
            if (string.IsNullOrEmpty(entity.ParentCode))
            {
                entity.ParentCode = null;
            }
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