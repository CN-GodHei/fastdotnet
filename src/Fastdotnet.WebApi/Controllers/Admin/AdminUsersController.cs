using AutoMapper;
using Fastdotnet.Core.Constants;
using Fastdotnet.Core.Controllers;
using Fastdotnet.Core.Entities.Admin;
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

        [Authorize(Policy = Permissions.Admin.Users.View)]
        public override Task<System.Collections.Generic.List<AdminUserDto>> GetAll() => base.GetAll();

        [Authorize(Policy = Permissions.Admin.Users.View)]
        public override Task<AdminUserDto> GetById(long id) => base.GetById(id);

        [Authorize(Policy = Permissions.Admin.Users.View)]
        public override Task<Fastdotnet.Core.Models.PageResult<AdminUserDto>> GetPage([FromQuery] int pageIndex = 1, [FromQuery] int pageSize = 10) => base.GetPage(pageIndex, pageSize);

        [Authorize(Policy = Permissions.Admin.Users.Create)]
        public override async Task<AdminUserDto> Create(CreateAdminUserDto dto)
        {
            var userId = await _adminUserService.CreateAsync(dto);
            var user = await _adminUserService.GetAsync(userId);
            return user!;
        }

        [Authorize(Policy = Permissions.Admin.Users.Edit)]
        public override Task<AdminUserDto> Update(long id, UpdateAdminUserDto dto) => base.Update(id, dto);

        [Authorize(Policy = Permissions.Admin.Users.Delete)]
        public override Task<bool> Delete(long id) => base.Delete(id);

        [HttpPost("{id}/reset-password")]
        [Authorize(Policy = Permissions.Admin.Users.ResetPassword)]
        public async Task<IActionResult> ResetPassword(long id, [FromBody] ResetPasswordDto dto)
        { 
            await _adminUserService.ResetPasswordAsync(id, dto.NewPassword);
            return NoContent();
        }
    }
}
