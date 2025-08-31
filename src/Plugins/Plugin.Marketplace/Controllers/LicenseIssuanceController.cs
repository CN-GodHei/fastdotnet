using Fastdotnet.Plugin.Marketplace.Dto;
using Fastdotnet.Plugin.Marketplace.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Fastdotnet.Plugin.Marketplace.Controllers
{
    [ApiController]
    [Route("api/marketplace/issue")]
    public class LicenseIssuanceController : ControllerBase
    {
        private readonly ILicenseService _licenseService;

        public LicenseIssuanceController(ILicenseService licenseService)
        {
            _licenseService = licenseService;
        }

        /// <summary>
        /// [仅限测试] 为 PluginA 生成一个单服务器授权文件。
        /// </summary>
        /// <returns>一个 JSON 格式的授权文件内容。</returns>
        [HttpGet("generate-test-license")]
        [AllowAnonymous]
        public LicenseFileDto GenerateTestLicenseForPluginA()
        {
            // 在真实场景中，这些值将来自数据库和前端请求
            var pluginId = "11375910391972869";
            var userId = "test-user-01";
            var licenseType = "SingleServer";
            // 生成当前机器的指纹来创建一个有效的授权
            var machineFingerprint = Utils.FingerprintUtil.GenerateMachineFingerprint();
            //dotnet user-secrets init --project src/Fastdotnet.WebApi
            //dotnet user-secrets set "Marketplace:PrivateKey" "..."
            var licenseJson = _licenseService.GenerateLicense(pluginId, userId, licenseType, machineFingerprint);
            return licenseJson;
        }
    }
}
