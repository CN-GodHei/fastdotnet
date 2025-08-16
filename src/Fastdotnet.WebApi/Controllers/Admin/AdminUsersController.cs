using AutoMapper;
using Fastdotnet.Core.Constants;
using Fastdotnet.Core.Controllers;
using Fastdotnet.Core.Entities.Admin;
using Fastdotnet.Core.Exceptions;
using Fastdotnet.Core.IService;
using Fastdotnet.Core.Models.Admin.Users;
using Fastdotnet.Service.IService.Admin;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace Fastdotnet.WebApi.Controllers.Admin
{
    [ApiController]
    [Route("api/admin/users")]
    public class AdminUsersController : GenericDtoControllerBase<FdAdminUser, string, CreateAdminUserDto, UpdateAdminUserDto, AdminUserDto>
    {
        private readonly IAdminUserService _adminUserService;
        private readonly ICurrentUser _currentUser;

        public AdminUsersController(
            IAdminUserService adminUserService,
            IRepository<FdAdminUser> repository,
            IMapper mapper,
            ICurrentUser currentUser) : base(repository, mapper)
        {
            _adminUserService = adminUserService;
            _currentUser = currentUser;
        }

        [Authorize(Policy = Permissions.Admin.Users.View)]
        public override Task<System.Collections.Generic.List<AdminUserDto>> GetAll() => base.GetAll();

        //[Authorize(Policy = Permissions.Admin.Users.View)]
        //public async Task<AdminUserDto> GetById(string id)
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
        public override Task<Fastdotnet.Core.Models.PageResult<AdminUserDto>> GetPage([FromQuery] int pageIndex = 1, [FromQuery] int pageSize = 10) => base.GetPage(pageIndex, pageSize);

        [Authorize(Policy = Permissions.Admin.Users.Create)]
        public override async Task<AdminUserDto> Create(CreateAdminUserDto dto)
        {
            var userId = await _adminUserService.CreateAsync(dto);
            var user = await _adminUserService.GetAsync(userId);
            return user!;
        }

        //[Authorize(Policy = Permissions.Admin.Users.Edit)]
        //public async Task<AdminUserDto> Update(string id, UpdateAdminUserDto dto)
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
        public async Task<IActionResult> ResetPassword(string id, [FromBody] ResetPasswordDto dto)
        { 
            await _adminUserService.ResetPasswordAsync(id, dto.NewPassword);
            return NoContent();
        }
    }
}