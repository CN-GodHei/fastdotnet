using AutoMapper;
using Fastdotnet.Core.Constants;
using Fastdotnet.Core.Controllers;
using Fastdotnet.Core.Entities.Admin;
using Fastdotnet.Core.Exceptions;
using Fastdotnet.Core.IService;
using Fastdotnet.Core.Models.Admin.Users;
using Fastdotnet.Core.Utils.Extensions;
using Fastdotnet.Service.IService.Admin;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Fastdotnet.WebApi.Controllers.Admin
{
    [ApiController]
    [Route("api/admin/[Controller]")]
    public class FdAdminUserController : GenericDtoControllerBase<FdAdminUser, string, CreateFdAdminUserDto, UpdateFdAdminUserDto, FdAdminUserDto>
    {
        private readonly IAdminUserService _adminUserService;
        private readonly ICurrentUser _currentUser;
        private readonly IBaseService<FdAdminUser, string> _service;

        public FdAdminUserController(
            IAdminUserService adminUserService,
            IBaseService<FdAdminUser, string> service,
            IMapper mapper,
            ICurrentUser currentUser) : base(service, mapper)
        {
            _adminUserService = adminUserService;
            _service = service;
            _currentUser = currentUser;
        }

        /// <summary>
        /// 获取所有管理员用户 (自定义注释)
        /// </summary>
        /// <returns>包含所有管理员用户的列表</returns>
        [Authorize(Policy = Permissions.Admin.Users.View)]
        public override Task<List<FdAdminUserDto>> GetAll( CancellationToken cancellationToken = default) => base.GetAll();

        //[Authorize(Policy = Permissions.Admin.Users.View)]
        //public async Task<FdAdminUserDto> GetById(string id)
        //{
        //    // 超管可以查看所有用户，普通用户只能查看自己
        //    //bool isSuperAdmin = await _adminUserService.IsSuperAdminAsync(_currentUser.Id ?? 0);
        //    //if (!isSuperAdmin && _currentUser.Id != id)
        //    //{
        //    //    throw new UnauthorizedAccessException("您只能查看自己的信息");
        //    //}
            
        //    var user = await _adminUserService.GetAsync(id);
        //    return user!;
        //}

        [Authorize(Policy = Permissions.Admin.Users.View)]
        public override Task<Fastdotnet.Core.Models.PageResult<FdAdminUserDto>> GetPage([FromQuery] int pageIndex = 1, [FromQuery] int pageSize = 10, CancellationToken cancellationToken = default) => base.GetPage(pageIndex, pageSize);

        [Authorize(Policy = Permissions.Admin.Users.Create)]
        public override async Task<FdAdminUserDto> Create(CreateFdAdminUserDto dto)
        {
            var userId = await _adminUserService.CreateAsync(dto);
            var user = await _adminUserService.GetAsync(userId);
            return user!;
        }

        //[Authorize(Policy = Permissions.Admin.Users.Edit)]
        //public async Task<FdAdminUserDto> Update(string id, UpdateFdAdminUserDto dto)
        //{
        //    // 超管可以更新所有用户，普通用户只能更新自己
        //    //bool isSuperAdmin = await _adminUserService.IsSuperAdminAsync(_currentUser.Id ?? 0);
        //    //if (!isSuperAdmin && _currentUser.Id != id)
        //    //{
        //    //    throw new UnauthorizedAccessException("您只能更新自己的信息");
        //    //}
            
        //    await _adminUserService.UpdateAsync(id, dto);
        //    var user = await _adminUserService.GetAsync(id);
        //    return user!;
        //}

        [Authorize(Policy = Permissions.Admin.Users.Delete)]
        public override async Task<bool> Delete(string id)
        {
            // 检查是否尝试删除超管用户
            bool isTargetSuperAdmin = await _adminUserService.IsSuperAdminAsync(id);
            if (isTargetSuperAdmin)
            {
                throw new BusinessException("不能删除超级管理员用户");
            }
            
            // 只有超管可以删除用户
            //bool isCurrentUserSuperAdmin = await _adminUserService.IsSuperAdminAsync(_currentUser.Id ?? 0);
            //if (!isCurrentUserSuperAdmin)
            //{
            //    throw new UnauthorizedAccessException("只有超级管理员可以删除用户");
            //}
            
            await _adminUserService.DeleteAsync(id);
            return true;
        }

        [HttpPost("{id}/reset-password")]
        [Authorize(Policy = Permissions.Admin.Users.ResetPassword)]
        public async Task<bool> ResetPassword(string id, [FromBody] ResetPasswordDto dto)
        { 
            await _adminUserService.ResetPasswordAsync(id, dto.NewPassword);
            return true;
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
            var userRoleRelations = await _adminUserService.GetUserRoleRelationsAsync(_currentUser.Id);
            var roleIds = userRoleRelations.Select(ur => ur.RoleId).ToList();

            // 获取用户按钮权限
            var buttons = await _adminUserService.GetUserButtonPermissionsAsync(_currentUser.Id);

            // 构造返回对象
            var userDto = _mapper.Map<FdAdminUserDto>(user);
            userDto.RoleIds = roleIds;
            userDto.Buttons = buttons;

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
            var user = await _service.GetFirstAsync(u => u.Id == userId&&u.Password==dto.Password);
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