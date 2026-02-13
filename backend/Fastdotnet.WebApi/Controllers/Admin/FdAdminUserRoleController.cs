namespace Fastdotnet.WebApi.Controllers.Admin
{
    /// <summary>
    /// FdAdminUserRole 控制器
    /// </summary>
    [Route("api/[controller]")]
    public class FdAdminUserRoleController : GenericDtoControllerBase<FdAdminUserRole, string, CreateFdAdminUserRoleDto, UpdateFdAdminUserRoleDto, FdAdminUserRoleDto>
    {
        private readonly IBaseService<FdAdminUserRole, string> _baseService;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IAdminUserService _adminUserService;

        public FdAdminUserRoleController(
            //IFdAdminUserRoleService fdadminuserroleService,
            IBaseService<FdAdminUserRole, string> service,
            IMapper mapper,
            IUnitOfWork unitOfWork,
            IAdminUserService adminUserService) : base(service, mapper)
        {
            _baseService = service;
            _unitOfWork = unitOfWork;
            _adminUserService = adminUserService;
        }

        /// <summary>
        /// 为用户分配角色（事务安全的一次性操作）
        /// </summary>
        [HttpPost("assign-roles")]
        public async Task<bool> AssignUserRoles([FromBody] AssignUserRolesDto dto)
        {
            dto.IsValid();
            await _unitOfWork.BeginTransactionAsync();
            try
            {
                // 1. 删除用户现有角色关联
                var Delresult = await _baseService.DeleteAsync(x => x.AdminUserId == dto.UserId);
                // 2. 创建新的角色关联
                var newUserRoles = dto.RoleIds.Select(roleId => new FdAdminUserRole
                {
                    AdminUserId = dto.UserId,
                    RoleId = roleId
                }).ToList();

                if (newUserRoles.Any())
                {
                    await _baseService.InsertRangeAsync(newUserRoles);
                }
                await _unitOfWork.CommitAsync();

            }
            catch (Exception ex)
            {
                await _unitOfWork.RollbackAsync();
                return false;
            }
            return true;
        }

        /// <summary>
        /// 获取指定用户当前角色
        /// </summary>
        [HttpGet("user/{userId}/roles")]
        public async Task<ActionResult<List<string>>> GetUserRoles(string userId)
        {
            userId.IsValid();
            var roles = await _adminUserService.GetUserRoleRelationsAsync(userId);
            return roles.Select(ur => ur.RoleId).ToList();
        }
    }
}
