namespace Fastdotnet.WebApi.Controllers.Admin
{
    [ApiController]
    [Route("api/admin/[Controller]")]
    public class FdPermissionsController : GenericDtoControllerBase<FdPermission, string, CreateFdPermissionDto, UpdateFdPermissionDto, FdPermissionDto>
    {
        public FdPermissionsController(
            IBaseService<FdPermission, string> service,
            IMapper mapper) : base(service, mapper)
        {

        }

        /// <summary>
        /// 获取所有权限，按模块分组，方便前端展示
        /// </summary>
        //[HttpGet]
        //public async Task<Dictionary<string,List<FdPermissionDto>>> GetAll()
        //{
        //    var permissions = await _service.GetAllAsync();
        //    var permissionDtos = _mapper.Map<List<FdPermissionDto>>(permissions);

        //    var groupedPermissions = permissionDtos
        //        .GroupBy(p => p.Module ?? "System") // 将没有模块的权限归类到"System"
        //        .ToDictionary(g => g.Key, g => g.ToList());

        //    return groupedPermissions;
        //}
    }
}
