using Fastdotnet.Core.Dtos.Sys;
using Fastdotnet.Core.Entities.Sys;
using Fastdotnet.Service.IService.Sys;

namespace Fastdotnet.WebApi.Controllers.Admin
{
    /// <summary>
    /// FdDictData 控制器
    /// </summary>
    [Route("api/[controller]")]
    public class FdDictDataAppController : AppGenericDtoControllerBase<FdDictData, string, CreateFdDictDataDto, UpdateFdDictDataDto, FdDictDataDto>
    {
        IBaseService<FdDictData, string> _service;
        IFdDictDataService _ddDictDataService;
        public FdDictDataAppController(
            IFdDictDataService fddictdataService,
            IBaseService<FdDictData, string> service,
            IMapper mapper) : base(service, mapper)
        {
            _service = service;
            _ddDictDataService = fddictdataService;
        }

        /// <summary>
        /// 获取用户相关配置
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        /// <exception cref="BusinessException"></exception>
        [HttpGet("GetUserConfig")]
        [AllowAnonymous]
        public async Task<List<FdDictDataDto>> UserConfig()
        {
            return _mapper.Map<List<FdDictDataDto>>(await _ddDictDataService.GetUserConfig());
        }
    }
}
