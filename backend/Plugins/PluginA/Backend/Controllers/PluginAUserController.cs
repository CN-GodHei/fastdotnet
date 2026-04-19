using AutoMapper;
using Fastdotnet.Core.Attributes;
using Fastdotnet.Core.Controllers;
using Fastdotnet.Core.Dtos.App;
using Fastdotnet.Core.Entities.App;
using Fastdotnet.Core.Enum;
using Fastdotnet.Core.Extensibility.Users;
using Fastdotnet.Core.IService.App;
using Fastdotnet.Core.IService.Sys;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PluginA.Dto;
using PluginA.Entities;
using PluginA.IService;
using System;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace PluginA.Controllers
{
    [Route("api/[controller]")]
    [ApiUsageScope(ApiUsageScopeEnum.Both)]
    [AllowAnonymous]
    public class PluginAUserController : ControllerBase
    {
        private readonly IFdAppUserService _appUserService;
        private readonly IMapper _mpper;
        private readonly IStorageContext _storageContext;
        private readonly IFdAppUserExtensionHandler<PluginAUserExtension> _userExtensionHandler;

        public PluginAUserController(
            IFdAppUserService appUserService, 
            IMapper mpper,
            IStorageContext storageContext,
            IFdAppUserExtensionHandler<PluginAUserExtension> userExtensionHandler)
        {
            _appUserService = appUserService;
            _mpper = mpper;
            _storageContext = storageContext;
            _userExtensionHandler = userExtensionHandler;
        }

        /// <summary>
        /// 获取用户的PluginA扩展数据
        /// </summary>
        [HttpGet("extension/{userId}")]
        public async Task<IActionResult> GetUserExtension(string userId)
        {
            try
            {
                var result = await _appUserService.GetFdAppUserWithExtensionAsync<PluginAUserExtension>(userId);

                if (result.Base == null)
                {
                    return NotFound($"User with ID {userId} not found.");
                }

                return Ok(new
                {
                    User = result.Base,
                    Extension = result.Extension ?? new PluginAUserExtension
                    {
                        FdAppUserId = userId,
                        Preferences = "",
                        Points = 0
                    }
                });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        /// <summary>
        /// 更新用户的PluginA扩展数据
        /// </summary>
        [HttpPut("extension/{userId}")]
        public async Task<IActionResult> UpdateUserExtension(string userId, [FromBody] PluginAUserExtension extensionData)
        {
            try
            {
                // 验证用户是否存在
                var user = await _appUserService.GetFdAppUserAsync(userId);
                if (user == null)
                {
                    return NotFound($"User with ID {userId} not found.");
                }

                // 【优化】仅更新插件扩展表，不触碰主框架用户表
                await _userExtensionHandler.SaveAsync(userId, extensionData, _storageContext);

                return Ok(new { Message = "User extension updated successfully." });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        /// <summary>
        /// 创建带有PluginA扩展数据的新用户
        /// </summary>
        [HttpPost("extension")]
        public async Task<IActionResult> CreateUserWithExtension([FromBody] CreateUserWithExtensionRequest request)
        {
            try
            {
                var userId = await _appUserService.CreateFdAppUserWithExtensionAsync(
                    u =>
                    {
                        u.Username = request.Username;
                        u.Password = request.Password;
                        u.Email = request.Email;
                        u.PhoneNumber = request.PhoneNumber;
                        u.Nickname = request.Nickname;
                        u.AvatarUrl = request.AvatarUrl;
                        u.Status = request.Status;
                        u.RegistrationDate = System.DateTime.Now;
                    },
                    _mpper.Map<PluginAUserExtension>(request.ExtensionData)
                );
                //var userId = await _appUserService.CreateFdAppUserWithExtensionAsync(
                //    u =>
                //    {
                //        u.Username = request.Username;
                //        u.Password = request.Password;
                //        u.Email = request.Email;
                //        u.PhoneNumber = request.PhoneNumber;
                //        u.Nickname = request.Nickname;
                //        u.AvatarUrl = request.AvatarUrl;
                //        u.Status = request.Status;
                //        u.RegistrationDate = System.DateTime.Now;
                //    },
                //  new PluginAUserExtension() { Points = request.ExtensionData.Points, Preferences = request.ExtensionData.Preferences }
                //);
                return Ok(new { UserId = userId, Message = "User created with extension successfully." });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }

    public class CreateUserWithExtensionRequest : CreateFdAppUserDto
    {
        public CreatePluginAUserExtensionDto ExtensionData { get; set; }
    }
}