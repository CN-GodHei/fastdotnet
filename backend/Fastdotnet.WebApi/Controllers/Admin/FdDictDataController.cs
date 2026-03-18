using Fastdotnet.Core.Dtos.Sys;
using Fastdotnet.Core.Entities.Sys;

namespace Fastdotnet.WebApi.Controllers.Admin
{
    /// <summary>
    /// FdDictData 控制器
    /// </summary>
    [Route("api/admin/[controller]")]
    public class FdDictDataController : GenericDtoControllerBase<FdDictData, string, CreateFdDictDataDto, UpdateFdDictDataDto, FdDictDataDto>
    {
        public FdDictDataController(
            //IFdDictDataService fddictdataService,
            IBaseService<FdDictData, string> service,
            IMapper mapper) : base(service, mapper)
        {

        }
    }
}
