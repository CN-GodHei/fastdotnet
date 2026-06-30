using Fastdotnet.Core.Dtos.Sys;
using Fastdotnet.Core.Entities.Sys;

namespace Fastdotnet.WebApi.Controllers.Admin
{
    [ApiController]
    [Route("api/admin/[controller]")]
    public class FdPermissionsController : GenericDtoControllerBase<FdPermission, string, CreateFdPermissionDto, UpdateFdPermissionDto, FdPermissionDto>
    {
        public FdPermissionsController(
            IBaseService<FdPermission, string> service) : base(service)
        {

        }


    }
}
