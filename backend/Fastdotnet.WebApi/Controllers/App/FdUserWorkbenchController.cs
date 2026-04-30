using Fastdotnet.Core.Controllers;
using Fastdotnet.Core.Dtos.App;
using Fastdotnet.Core.Dtos.Sys;
using Fastdotnet.Core.Entities.App;
using Fastdotnet.Core.Attributes;
using Fastdotnet.Core.Enum;
using Fastdotnet.Service.IService;
using Fastdotnet.Service.IService.App;
using Microsoft.AspNetCore.Mvc;
using AutoMapper;

namespace Fastdotnet.WebApi.Controllers.App
{
    /// <summary>
    /// 我的工作台 (应用端)
    /// </summary>
    [Route("api/app/[controller]")]
    [ApiUsageScope(ApiUsageScopeEnum.Both)]
    public class FdAppUserWorkbenchController : AppGenericDtoControllerBase<FdUserLayout, SaveFdUserLayoutDto, UpdateFdUserLayoutDto, FdUserLayoutDto>
    {
        private readonly IFdUserWorkbenchService _userWorkbenchService;

        public FdAppUserWorkbenchController(
            IFdUserWorkbenchService userWorkbenchService,
            IBaseService<FdUserLayout, string> service,
            IMapper mapper,
            Fastdotnet.Core.IService.ICurrentUser currentUser) : base(service, mapper, currentUser)
        {
            _userWorkbenchService = userWorkbenchService;
        }

        /// <summary>
        /// 获取我可用的卡片列表 (已授权)
        /// </summary>
        [HttpGet("available-cards")]
        public async Task<List<FdWorkbenchCardDto>> GetAvailableCards()
        {
            return await _userWorkbenchService.GetAvailableCardsAsync();
        }

        /// <summary>
        /// 获取我的当前工作台布局
        /// </summary>
        [HttpGet("my-layout")]
        public async Task<FdUserLayoutDto?> GetMyLayout(string name = "Default")
        {
            return await _userWorkbenchService.GetMyLayoutAsync(name);
        }

        /// <summary>
        /// 保存我的工作台布局 (自定义业务逻辑)
        /// </summary>
        [HttpPost("save-layout")]
        public async Task<bool> SaveLayout([FromBody] SaveFdUserLayoutDto dto)
        {
            return await _userWorkbenchService.SaveMyLayoutAsync(dto);
        }
    }
}
