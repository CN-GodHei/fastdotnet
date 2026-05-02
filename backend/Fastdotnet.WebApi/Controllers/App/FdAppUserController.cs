using Fastdotnet.Core.Dtos.Admin.Users;
using Fastdotnet.Core.Dtos.App;
using Fastdotnet.Service.IService.App;
using Fastdotnet.Service.IService.Sys;
using Microsoft.AspNetCore.DataProtection.KeyManagement;
using Fastdotnet.Service.IService;

namespace Fastdotnet.WebApi.Controllers.App
{
    [ApiController]
    [Route("api/[controller]")]
    [ApiUsageScope(Core.Enum.ApiUsageScopeEnum.Both)]
    public class FdAppUserController 
        : AppGenericDtoControllerBase<FdAppUser, CreateFdAppUserDto, UpdateFdAppUserDto, FdAppUserDto>
    {
        public FdAppUserController(IBaseService<FdAppUser, string> service, ICurrentUser currentUser, IAppUserService appUserService, IPasswordService passwordService, IVerificationCodeManager verificationCodeManager) : base(service, currentUser)
        {
            _service = service;
            _currentUser = currentUser;
            _appUserService = appUserService;
            _passwordService = passwordService;
            _verificationCodeManager = verificationCodeManager;
        }
        private readonly IBaseService<FdAppUser, string> _service;
        private readonly ICurrentUser _currentUser;
        private readonly IAppUserService _appUserService;
        private readonly IPasswordService _passwordService;
        private readonly IVerificationCodeManager _verificationCodeManager;
        //public  FdAppUserController(IBaseService<FdAppUser, string> service, ICurrentUser currentUser) 
        //{
        //    _service = service;
        //    _mapper = mapper;
        //    _currentUser = currentUser;
        //}
        [HttpGet("getUserInfo")]
        public async Task<FdAppUserDto> getUserInfo()
        {
            // 获取当前用户信息
            var user = await _service.GetByIdAsync(_currentUser.Id);
            if (user == null)
            {
                throw new UnauthorizedAccessException("用户不存在");
            }

            // 获取用户角色
            var userRoleRelations = await _appUserService.GetUserRoleRelationsAsync(_currentUser.Id);
            var roleIds = userRoleRelations.Select(ur => ur.RoleId).ToList();

            // 获取用户按钮权限
            var buttons = await _appUserService.GetUserButtonPermissionsAsync(_currentUser.Id);

            // 构造返回对象
            var userDto = user.Adapt<FdAppUserDto>();
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
            var user = await _service.GetByIdAsync(userId);
            if (user == null)
            {
                return false;
            }

            // 使用密码服务验证密码
            bool isValid = await _passwordService.VerifyPasswordAsync(dto.Password, user.Password);

            return isValid;
        }

        /// <summary>
        /// 重置用户密码为系统默认密码
        /// </summary>
        /// <param name="id">用户ID</param>
        /// <returns>操作结果</returns>
        [HttpPost("{id}/reset-password")]
        [Authorize]
        public async Task<bool> ResetPassword(string id)
        {
            await _appUserService.ResetPasswordAsync(id);
            return true;
        }

        /// <summary>
        /// 修改用户邮箱
        /// </summary>
        /// <param name="dto">包含新邮箱和验证码的信息</param>
        /// <returns>操作结果</returns>
        [HttpPost("change-email")]
        [Authorize]
        public async Task<bool> ChangeEmail([FromBody] ChangeEmailDto dto)
        {
            dto.IsValid();
            
            // 获取当前用户ID
            var userId = _currentUser.Id;
            if (string.IsNullOrEmpty(userId))
            {
                throw new UnauthorizedAccessException("用户未登录");
            }

            // 验证验证码
            bool isCodeValid = await _verificationCodeManager.VerifyCodeAsync(dto.NewEmail, dto.VerificationCode, "ChangeEmail");
            if (!isCodeValid)
            {
                throw new BusinessException("验证码错误或已过期");
            }

            // 检查新邮箱是否已被其他用户使用
            var existingUser = await _service.GetFirstAsync(u => u.Email == dto.NewEmail && u.Id != userId);
            if (existingUser != null)
            {
                throw new BusinessException("该邮箱已被其他用户使用");
            }

            // 更新用户邮箱
            var user = await _service.GetByIdAsync(userId);
            if (user == null)
            {
                throw new BusinessException("用户不存在");
            }

            user.Email = dto.NewEmail;
            await _service.UpdateAsync(user);

            return true;
        }

        /// <summary>
        /// 修改用户密码
        /// </summary>
        /// <param name="dto">包含当前密码和新密码的信息</param>
        /// <returns>操作结果</returns>
        [HttpPost("change-password")]
        [Authorize]
        public async Task<bool> ChangePassword([FromBody] ChangePasswordDto dto)
        {
            dto.IsValid();
            
            // 获取当前用户ID
            var userId = _currentUser.Id;
            if (string.IsNullOrEmpty(userId))
            {
                throw new UnauthorizedAccessException("用户未登录");
            }

            // 获取当前用户信息
            var user = await _service.GetByIdAsync(userId);
            if (user == null)
            {
                throw new BusinessException("用户不存在");
            }

            // 验证当前密码
            bool isCurrentPasswordValid = await _passwordService.VerifyPasswordAsync(dto.CurrentPassword, user.Password);
            if (!isCurrentPasswordValid)
            {
                throw new BusinessException("当前密码错误");
            }

            // 加密新密码
            string encryptedNewPassword = await _passwordService.EncryptPasswordAsync(dto.NewPassword);

            // 更新用户密码
            user.Password = encryptedNewPassword;
            await _service.UpdateAsync(user);

            return true;
        }

        /// <summary>
        /// 发送修改邮箱验证码
        /// </summary>
        /// <param name="email">新邮箱地址</param>
        /// <returns>操作结果</returns>
        [HttpPost("send-change-email-code")]
        [Authorize]
        public async Task<bool> SendChangeEmailCode([FromBody] string email)
        {
            if (string.IsNullOrEmpty(email))
            {
                throw new BusinessException("邮箱地址不能为空");
            }

            // 检查新邮箱是否已被其他用户使用
            var userId = _currentUser.Id;
            var existingUser = await _service.GetFirstAsync(u => u.Email == email && u.Id != userId);
            if (existingUser != null)
            {
                throw new BusinessException("该邮箱已被其他用户使用");
            }

            // 发送验证码
            await _verificationCodeManager.SendCodeAsync(email, "ChangeEmail");
            return true;
        }
    }
}
