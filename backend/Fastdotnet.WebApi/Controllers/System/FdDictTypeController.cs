namespace Fastdotnet.WebApi.Controllers.System
{
    /// <summary>
    /// FdDictType 控制器
    /// </summary>
    [Route("api/[controller]")]
    public class FdDictTypeController : GenericDtoControllerBase<FdDictType, string, CreateFdDictTypeDto, UpdateFdDictTypeDto, FdDictTypeDto>
    {
        public FdDictTypeController(
            //IFdDictTypeService fddicttypeService,
            IBaseService<FdDictType, string> service,
            IMapper mapper) : base(service, mapper)
        {

        }
    }
}
