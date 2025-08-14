using AutoMapper;
using Fastdotnet.Core.Controllers;
using Fastdotnet.Core.Entities.Admin;
using Fastdotnet.Core.IService;
using Fastdotnet.Core.Models.Admin.Users;
using Fastdotnet.Service.IService.Admin;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace Fastdotnet.WebApi.Controllers.Admin
{
    [ApiController]
    [Route("api/admin/users")]
    public class AdminUsersController : GenericDtoControllerBase<FdAdminUser, long, CreateAdminUserDto, UpdateAdminUserDto, AdminUserDto>
    {
        private readonly IAdminUserService _adminUserService;

        public AdminUsersController(
            IAdminUserService adminUserService,
            IRepository<FdAdminUser> repository,
            IMapper mapper) : base(repository, mapper)
        {
            _adminUserService = adminUserService;
        }

        /// <summary>
        /// 创建新管理员（重写）
        /// </summary>
        /// <remarks>
        /// 重写基类方法以调用包含特殊业务逻辑（如密码加密、用户名检查）的服务。
        /// </remarks>
        /// <param name="dto">创建用户的DTO</param>
        /// <returns>创建成功后的用户信息</returns>
        public override async Task<AdminUserDto> Create(CreateAdminUserDto dto)
        {
            var userId = await _adminUserService.CreateAsync(dto);
            var user = await _adminUserService.GetAsync(userId);
            // 在实际项目中，如果GetAsync返回null，应进行处理
            return user!;
        }

        // Get, GetPage, Update, Delete 等所有其他标准CRUD方法均由 GenericDtoControllerBase 提供

        /// <summary>
        /// 重置管理员密码
        /// </summary>
        /// <param name="id">用户ID</param>
        /// <param name="dto">重置密码的DTO</param>
        [HttpPost("{id}/reset-password")]
        public async Task<IActionResult> ResetPassword(long id, [FromBody] ResetPasswordDto dto)
        { 
            await _adminUserService.ResetPasswordAsync(id, dto.NewPassword);
            return NoContent();
        }
    }
}