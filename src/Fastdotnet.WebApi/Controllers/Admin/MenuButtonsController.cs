using AutoMapper;
using Fastdotnet.Core.Constants;
using Fastdotnet.Core.Controllers;
using Fastdotnet.Core.Entities.System;
using Fastdotnet.Core.IService;
using Fastdotnet.Core.Models.System;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Fastdotnet.WebApi.Controllers.Admin
{
    [ApiController]
    [Route("api/admin/menu-buttons")]
    [Authorize]
    public class MenuButtonsController : GenericDtoControllerBase<FdMenuButton, string, CreateMenuButtonDto, UpdateMenuButtonDto, MenuButtonDto>
    {
        public MenuButtonsController(
            IRepository<FdMenuButton> repository,
            IMapper mapper) : base(repository, mapper)
        {
        }

        [Authorize(Policy = Permissions.Admin.MenuButtons.View)]
        public override Task<List<MenuButtonDto>> GetAll() => base.GetAll();

        [Authorize(Policy = Permissions.Admin.MenuButtons.View)]
        public override Task<MenuButtonDto> GetById(string id) => base.GetById(id);

        [Authorize(Policy = Permissions.Admin.MenuButtons.View)]
        public override Task<Fastdotnet.Core.Models.PageResult<MenuButtonDto>> GetPage([FromQuery] int pageIndex = 1, [FromQuery] int pageSize = 10) => base.GetPage(pageIndex, pageSize);

        [Authorize(Policy = Permissions.Admin.MenuButtons.Create)]
        public override Task<MenuButtonDto> Create(CreateMenuButtonDto dto) => base.Create(dto);

        [Authorize(Policy = Permissions.Admin.MenuButtons.Edit)]
        public override Task<MenuButtonDto> Update(string id, UpdateMenuButtonDto dto) => base.Update(id, dto);

        [Authorize(Policy = Permissions.Admin.MenuButtons.Delete)]
        public override Task<bool> Delete(string id) => base.Delete(id);
    }
}