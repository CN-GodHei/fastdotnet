using Fastdotnet.Core.Utils.Extensions;
using Fastdotnet.Plugin.Marketplace.Dto;
using Fastdotnet.Plugin.Marketplace.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Fastdotnet.Plugin.Marketplace.Controllers
{
    [ApiController]
    [Route("api/marketplace/license")]
    public class LicenseIssuanceController : ControllerBase
    {
        private readonly ILicenseService _licenseService;
        // 假设我们有一个服务来查询用户授权信息
        // private readonly IUserLicenseLookupService _userLicenseLookupService; 

        public LicenseIssuanceController(ILicenseService licenseService /*, IUserLicenseLookupService userLicenseLookupService*/)
        {
            _licenseService = licenseService;
            // _userLicenseLookupService = userLicenseLookupService;
        }

        /// <summary>
        /// 为指定插件和用户生成授权文件。
        /// </summary>
        /// <param name="request">包含生成授权所需信息的请求对象。</param>
        /// <returns>生成的授权文件内容。</returns>
        [HttpPost("generate")]
        [AllowAnonymous] // 根据需求决定是否允许匿名访问
        public IActionResult GenerateLicense([FromBody] GenerateLicenseRequestDto request)
        {
            if (request == null)
            {
                return BadRequest("请求体不能为空。");
            }

            // 使用自定义扩展方法进行模型验证 (包括 PluginId, UserId, MachineFingerprint)
            request.IsValid();
            request.MachineFingerprint = Utils.FingerprintUtil.GenerateMachineFingerprint();
            try
            {
                // 在真实场景中，这里需要查询数据库或调用其他服务来获取用户的授权类型
                // var userLicenseInfo = _userLicenseLookupService.GetUserLicenseInfo(request.UserId, request.PluginId);
                // if (userLicenseInfo == null)
                // {
                //     return NotFound("未找到该用户对此插件的授权记录。");
                // }
                // string licenseType = userLicenseInfo.LicenseType;
                
                // 临时示例：假设所有请求都是 SingleServer 类型
                // 实际应用中应替换为上面的查询逻辑
                string licenseType = "SingleServer"; 

                // 准备用于生成授权文件的指纹
                string fingerprintToUse = request.MachineFingerprint;
                
                // 根据授权类型决定如何使用指纹（或是否使用）
                // 例如，对于 MultiServer，可能使用一个特殊标识符而不是具体指纹
                // if (licenseType.Equals("MultiServer", StringComparison.OrdinalIgnoreCase))
                // {
                //     fingerprintToUse = "MULTI_SERVER_LICENSE"; // 或者其他逻辑
                // }
                // 对于 SingleServer, 直接使用 request.MachineFingerprint
                
                // 当前示例直接使用传入的指纹
                // （在实际应用中，可能还需要验证指纹的有效性或格式）

                var licenseDto = _licenseService.GenerateLicense(
                    request.PluginId,
                    request.UserId,
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
    }
}
