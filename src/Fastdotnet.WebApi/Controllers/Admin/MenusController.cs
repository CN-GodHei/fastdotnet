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

namespace Fastdotnet.WebApi.Controllers.Admin
{
    [ApiController]
    [Route("api/admin/menus")]
    [Authorize]
    public class MenusController : GenericDtoControllerBase<FdMenu, long, CreateMenuDto, UpdateMenuDto, MenuDto>
    {
        private readonly IMenuService _menuService;
        private readonly ICurrentUser _currentUser;

        public MenusController(
            IRepository<FdMenu> repository,
            IMapper mapper,
            IMenuService menuService,
            ICurrentUser currentUser) : base(repository, mapper)
        {
            _menuService = menuService;
            _currentUser = currentUser;
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

            var menus = await _menuService.GetUserMenusAsync(userId.Value, "Admin");
            return Ok(menus);
        }

        [Authorize(Policy = Permissions.Admin.Menus.View)]
        public override Task<List<MenuDto>> GetAll() => base.GetAll();

        [Authorize(Policy = Permissions.Admin.Menus.View)]
        public override Task<MenuDto> GetById(long id) => base.GetById(id);

        [Authorize(Policy = Permissions.Admin.Menus.View)]
        public override Task<Fastdotnet.Core.Models.PageResult<MenuDto>> GetPage([FromQuery] int pageIndex = 1, [FromQuery] int pageSize = 10) => base.GetPage(pageIndex, pageSize);

        [Authorize(Policy = Permissions.Admin.Menus.Create)]
        public override Task<MenuDto> Create(CreateMenuDto dto) => base.Create(dto);

        [Authorize(Policy = Permissions.Admin.Menus.Edit)]
        public override Task<MenuDto> Update(long id, UpdateMenuDto dto) => base.Update(id, dto);

        [Authorize(Policy = Permissions.Admin.Menus.Delete)]
        public override Task<bool> Delete(long id) => base.Delete(id);
    }
}