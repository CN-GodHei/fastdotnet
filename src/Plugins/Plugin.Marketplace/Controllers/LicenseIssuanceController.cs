using Fastdotnet.Core.IService;
using Fastdotnet.Core.Utils.Extensions;
using Fastdotnet.Plugin.Marketplace.Dto;
using Fastdotnet.Plugin.Marketplace.IService;
using Fastdotnet.Plugin.Marketplace.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;

namespace Fastdotnet.Plugin.Marketplace.Controllers
{
    [ApiController]
    [Route("api/marketplace/license")]
    public class LicenseIssuanceController : ControllerBase
    {
        private readonly ILicenseService _licenseService;
        private readonly IUserLicenseLookupService _userLicenseLookupService;
        private readonly ICurrentUser _currentUser;

        public LicenseIssuanceController(ILicenseService licenseService, IUserLicenseLookupService userLicenseLookupService,
            ICurrentUser currentUser)
        {
            _licenseService = licenseService;
            _userLicenseLookupService = userLicenseLookupService;
            _currentUser = currentUser;
        }

        /// <summary>
        /// 为指定插件和用户生成授权文件。
        /// </summary>
        /// <param name="request">包含生成授权所需信息的请求对象。</param>
        /// <returns>生成的授权文件内容。</returns>
        [HttpPost("generate")]
        //[Authorize(Policy = "AdminOnly")] // 假设需要管理员权限才能生成授权
        //[AllowAnonymous]
        public IActionResult GenerateLicense([FromBody] GenerateLicenseRequestDto request)
        {
            if (request == null)
            {
                return BadRequest("请求体不能为空。");
            }

            // 使用自定义扩展方法进行模型验证 (包括 PluginId, UserId, MachineFingerprint)
            var validationResult = request.IsValid(internalReturn: false);
            if (!validationResult.IsValid)
            {
                ModelState.AddModelError(string.Empty, validationResult.Message);
            }

            if (!ModelState.IsValid)
            {
                return ValidationProblem(ModelState);
            }
            //后续可以改为前端传递
            request.MachineFingerprint = Utils.FingerprintUtil.GenerateMachineFingerprint();
            try
            {
                // ---------------------------------------------------
                // 调用 IUserLicenseLookupService 查询用户的授权信息
                // ---------------------------------------------------
                var userLicenseInfo = _userLicenseLookupService.GetUserLicenseInfo(_currentUser.Id, request.PluginId);
                
                if (userLicenseInfo == null)
                {
                    ModelState.AddModelError(string.Empty, "未找到该用户对此插件的有效授权记录。");
                    return ValidationProblem(ModelState);
                }

                string licenseType = userLicenseInfo.LicenseType;

                // 根据查询到的 LicenseType 验证 MachineFingerprint
                if (licenseType.Equals("SingleServer", StringComparison.OrdinalIgnoreCase) &&
                    string.IsNullOrWhiteSpace(request.MachineFingerprint))
                {
                    ModelState.AddModelError("MachineFingerprint", "对于 SingleServer 类型的授权，必须提供 MachineFingerprint。");
                    return ValidationProblem(ModelState);
                }

                // 准备用于生成授权文件的指纹
                string fingerprintToUse = request.MachineFingerprint;
                
                // 根据授权类型决定如何使用指纹（或是否使用）
                // 例如，对于 MultiServer，可能使用一个特殊标识符而不是具体指纹
                // if (licenseType.Equals("MultiServer", StringComparison.OrdinalIgnoreCase))
                // {
                //     fingerprintToUse = "MULTI_SERVER_LICENSE"; // 或者其他逻辑
                // }
                // 对于 SingleServer, 直接使用 request.MachineFingerprint

                var licenseDto = _licenseService.GenerateLicense(
                    request.PluginId,
                    _currentUser.Id,
                    licenseType, // 使用从数据库查询到的类型
                    fingerprintToUse // 使用处理后的指纹
                );

                return Ok(licenseDto); // 返回 JSON 格式的 LicenseFileDto
            }
            catch (Exception ex)
            {
                // 记录日志
                // _logger.LogError(ex, "生成授权文件时发生错误");
                return StatusCode(500, "生成授权文件时发生内部服务器错误。");
            }
        }

        /// <summary>
        /// [离线激活] 为指定插件和用户生成授权文件。
        /// </summary>
        /// <param name="request">包含离线激活所需信息的请求对象。</param>
        /// <returns>生成的授权文件内容 (LicenseFileDto)。</returns>
        [HttpPost("generate-offline")]
        [Authorize(Policy = "AdminOnly")] // 通常也需要管理员权限
        public IActionResult GenerateOfflineLicense([FromBody] OfflineActivationRequestDto request)
        {
            if (request == null)
            {
                return BadRequest("请求体不能为空。");
            }

            // 使用自定义扩展方法进行模型验证
            var validationResult = request.IsValid(internalReturn: false);
            if (!validationResult.IsValid)
            {
                ModelState.AddModelError(string.Empty, validationResult.Message);
            }

            // 额外的离线激活逻辑校验（例如，检查时间戳是否过旧以防止重放攻击）
            // 这里可以添加一个合理的窗口期，例如 1 小时
            if (request.RequestTimestamp != default && DateTime.UtcNow.Subtract(request.RequestTimestamp).TotalHours > 1)
            {
                ModelState.AddModelError("RequestTimestamp", "请求已过期。");
            }

            if (!ModelState.IsValid)
            {
                return ValidationProblem(ModelState);
            }

            try
            {
                // 调用 IUserLicenseLookupService 查询用户的授权信息
                // 注意：离线激活请求中包含 UserId，我们使用请求中的 UserId 进行查询
                var userLicenseInfo = _userLicenseLookupService.GetUserLicenseInfo(_currentUser.Id, request.PluginId);

                if (userLicenseInfo == null)
                {
                    ModelState.AddModelError(string.Empty, "未找到该用户对此插件的有效授权记录。");
                    return ValidationProblem(ModelState);
                }

                string licenseType = userLicenseInfo.LicenseType;

                // 离线激活时，通常也需要根据授权类型校验指纹
                if (licenseType.Equals("SingleServer", StringComparison.OrdinalIgnoreCase) &&
                    string.IsNullOrWhiteSpace(request.MachineFingerprint))
                {
                    ModelState.AddModelError("MachineFingerprint", "对于 SingleServer 类型的授权，必须提供 MachineFingerprint。");
                    return ValidationProblem(ModelState);
                }

                // 准备用于生成授权文件的指纹
                string fingerprintToUse = request.MachineFingerprint;

                // 生成许可证
                var licenseDto = _licenseService.GenerateLicense(
                    request.PluginId,
                    _currentUser.Id,
                    licenseType,
                    fingerprintToUse
                );

                // *********************************************************
                // 关键修改：直接返回 LicenseFileDto，与在线激活保持一致
                // 这确保了生成的 .lic 文件内容格式统一，插件加载器可以无差别验证
                // *********************************************************
                return Ok(licenseDto);
            }
            catch (Exception ex)
            {
                // 记录日志
                // _logger.LogError(ex, "生成离线授权文件时发生错误");
                return StatusCode(500, "生成离线授权文件时发生内部服务器错误。");
            }
        }
    }
}
