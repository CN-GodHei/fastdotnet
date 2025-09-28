using AutoMapper;
using Fastdotnet.Core.Constants;
using Fastdotnet.Core.Controllers;
using Fastdotnet.Core.Entities.System;
using Fastdotnet.Core.Exceptions;
using Fastdotnet.Core.IService;
using Fastdotnet.Core.Models.System;
using Fastdotnet.Core.Service;
using Fastdotnet.Core.Utils;
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
        private readonly IRepository<FdMenuButton> _repository1;
        public MenuButtonsController(
            IRepository<FdMenuButton> repository,
            IMapper mapper) : base(repository, mapper)
        {
            _repository1 = repository;
        }

        [Authorize(Policy = Permissions.Admin.MenuButtons.View)]
        public override Task<List<MenuButtonDto>> GetAll() => base.GetAll();

        [Authorize(Policy = Permissions.Admin.MenuButtons.View)]
        public override Task<MenuButtonDto> GetById(string id) => base.GetById(id);

        [Authorize(Policy = Permissions.Admin.MenuButtons.View)]
        public override Task<Fastdotnet.Core.Models.PageResult<MenuButtonDto>> GetPage([FromQuery] int pageIndex = 1, [FromQuery] int pageSize = 10) => base.GetPage(pageIndex, pageSize);

        [Authorize(Policy = Permissions.Admin.MenuButtons.Create)]
        public override Task<MenuButtonDto> Create(CreateMenuButtonDto dto) => base.Create(dto);
        protected override async Task BeforeCreate(FdMenuButton entity, CreateMenuButtonDto dto)
        {
            if (string.IsNullOrEmpty(entity.Code))
            {
                entity.Code = $"{dto.MenuCode}_{dto.Name}";
            }
            else
            {
                entity.Code = $"{dto.MenuCode}_{dto.Code}";
            }
            var et = await _repository1.GetFirstAsync(x => x.Code == entity.Code&&x.MenuCode==entity.MenuCode);
            if (et != null)
            {
                throw new BusinessException("菜单下已存在该编码的按钮");
            }
            await base.BeforeCreate(entity, dto);
        }


        [Authorize(Policy = Permissions.Admin.MenuButtons.Edit)]
        public override Task<MenuButtonDto> Update(string id, UpdateMenuButtonDto dto) => base.Update(id, dto);

        [Authorize(Policy = Permissions.Admin.MenuButtons.Delete)]
        public override Task<bool> Delete(string id) => base.Delete(id);
    }
}