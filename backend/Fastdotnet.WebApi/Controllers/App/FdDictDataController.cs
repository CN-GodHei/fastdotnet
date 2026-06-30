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
        // 重写 _service 以匹配此类需要的类型
        new IBaseService<FdDictData, string> _service;
        IFdDictDataService _ddDictDataService;
        public FdDictDataAppController(
            IFdDictDataService fddictdataService,
            IBaseService<FdDictData, string> service) : base(service)
        {
            _service = service;
            _ddDictDataService = fddictdataService;
        }

        /// <summary>
        /// 获取用户相关配置
        /// </summary>
        /// <returns></returns>
        /// <exception cref="BusinessException"></exception>
        [HttpGet("GetUserConfig")]
        [AllowAnonymous]
        public async Task<List<FdDictDataDto>> UserConfig()
        {
            return (await _ddDictDataService.GetUserConfig()).Adapt<List<FdDictDataDto>>();
        }
    }
}
