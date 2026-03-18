using Fastdotnet.Core.Dtos;
using Fastdotnet.Core.Dtos.Sys;
using Fastdotnet.Core.Entities.Sys;

namespace Fastdotnet.WebApi.Controllers.Admin
{
    [ApiController]
    [Route("api/admin/[Controller]")]
    [Authorize]
    public class FdMenuButtonsController : GenericDtoControllerBase<FdMenuButton, string, CreateFdFdMenuButtonDto, UpdateFdFdMenuButtonDto, FdMenuButtonDto>
    {
        private readonly IBaseService<FdMenuButton, string> _service1;
        public FdMenuButtonsController(
            IBaseService<FdMenuButton, string> service,
            IMapper mapper) : base(service, mapper)
        {
            _service1 = service;
        }

        [Authorize(Policy = Permissions.Admin.MenuButtons.View)]
        public override Task<List<FdMenuButtonDto>> GetAll(CancellationToken cancellationToken = default) => base.GetAll();

        [Authorize(Policy = Permissions.Admin.MenuButtons.View)]
        public override Task<FdMenuButtonDto> GetById(string id, CancellationToken cancellationToken = default) => base.GetById(id);

        [Authorize(Policy = Permissions.Admin.MenuButtons.View)]
        public override Task<PageResult<FdMenuButtonDto>> GetPage([FromQuery] int pageIndex = 1, [FromQuery] int pageSize = 10, CancellationToken cancellationToken = default) => base.GetPage(pageIndex, pageSize);

        [Authorize(Policy = Permissions.Admin.MenuButtons.Create)]
        public override Task<FdMenuButtonDto> Create(CreateFdFdMenuButtonDto dto) => base.Create(dto);
        protected override async Task BeforeCreate(FdMenuButton entity, CreateFdFdMenuButtonDto dto)
        {
            if (string.IsNullOrEmpty(entity.Code))
            {
                entity.Code = $"{dto.MenuCode}_{dto.Name}";
            }
            else
            {
                entity.Code = $"{dto.MenuCode}_{dto.Code}";
            }
            var et = await _service1.GetFirstAsync(x => x.Code == entity.Code&&x.MenuCode==entity.MenuCode);
            if (et != null)
            {
                throw new BusinessException("�˵����Ѵ��ڸñ����İ�ť");
            }
            await base.BeforeCreate(entity, dto);
        }


        [Authorize(Policy = Permissions.Admin.MenuButtons.Edit)]
        public override Task<FdMenuButtonDto> Update(string id, UpdateFdFdMenuButtonDto dto) => base.Update(id, dto);

        [Authorize(Policy = Permissions.Admin.MenuButtons.Delete)]
        public override Task<bool> Delete(string id) => base.Delete(id);
    }
}