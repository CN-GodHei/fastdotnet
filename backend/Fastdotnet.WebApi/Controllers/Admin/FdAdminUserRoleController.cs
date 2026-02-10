namespace Fastdotnet.WebApi.Controllers.Admin
{
    /// <summary>
    /// FdAdminUserRole 控制器
    /// </summary>
    [Route("api/[controller]")]
    public class FdAdminUserRoleController : GenericDtoControllerBase<FdAdminUserRole, string, CreateFdAdminUserRoleDto, UpdateFdAdminUserRoleDto, FdAdminUserRoleDto>
    {
        public FdAdminUserRoleController(
            //IFdAdminUserRoleService fdadminuserroleService,
            IBaseService<FdAdminUserRole, string> service,
            IMapper mapper) : base(service, mapper)
        {

        }
    }
}
