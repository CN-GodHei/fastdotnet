using Fastdotnet.Core.Models.App;

namespace Fastdotnet.WebApi.Controllers.App
{
    [ApiController]
    [Route("api/[controller]")]
    public class FdAppUserController : AppGenericDtoControllerBase<FdAppUser, CreateFdAppUserDto, UpdateFdAppUserDto, FdAppUserDto>
    {
        public FdAppUserController(IBaseService<FdAppUser, string> service, IMapper mapper, ICurrentUser currentUser) : base(service, mapper, currentUser)
        {
        }

        [HttpGet("getUserInfo")]
        [Authorize(Policy = Permissions.Admin.Users.View)]
        public async Task<FdAdminUserDto> getUserInfo()
        {
            // 获取当前用户信息
            var user = await _service.GetByIdAsync(_currentUser.Id);
            if (user == null)
            {
                throw new UnauthorizedAccessException("用户不存在");
            }

            // 获取用户角色
            //var userRoleRelations = await _adminUserService.GetUserRoleRelationsAsync(_currentUser.Id);
            //var roleIds = userRoleRelations.Select(ur => ur.RoleId).ToList();

            //// 获取用户按钮权限
            //var buttons = await _adminUserService.GetUserButtonPermissionsAsync(_currentUser.Id);

            // 构造返回对象
            var userDto = _mapper.Map<FdAdminUserDto>(user);
            //userDto.RoleIds = roleIds;
            //userDto.Buttons = buttons;

            return userDto;
        }

        /// <summary>
        /// 解锁屏幕
        /// </summary>
        /// <param name="dto">包含密码的解锁信息</param>
        /// <returns>解锁结果</returns>
        [HttpPost("unlock")]
        [Authorize]
        public async Task<bool> Unlock([FromBody] UnlockDto dto)
        {
            dto.IsValid();
            // 获取当前用户的ID
            var userId = _currentUser.Id;

            if (string.IsNullOrEmpty(userId))
            {
                return false;
            }

            // 获取当前用户信息
            var user = await _service.GetFirstAsync(u => u.Id == userId && u.Password == dto.Password);
            if (user == null)
            {
                return false;
            }

            // 验证密码
            bool isValid = user.Password == dto.Password;

            return isValid;
        }
    }
}
