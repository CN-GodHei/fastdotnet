using Fastdotnet.Core.Dtos.Oidc;
using Fastdotnet.Service.IService;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Fastdotnet.WebApi.Controllers.Oidc;

/// <summary>
/// OIDC 应用管理控制器
/// </summary>
[ApiController]
[Route("api/[controller]")]
//[Authorize(Roles = "Admin")] // 仅管理员可访问
[ApiUsageScope(ApiUsageScopeEnum.AdminOnly)]
public class OidcAppController : ControllerBase
{
    private readonly IOidcAppService _oidcAppService;

    public OidcAppController(IOidcAppService oidcAppService)
    {
        _oidcAppService = oidcAppService;
    }

    /// <summary>
    /// 创建 OIDC 应用
    /// </summary>
    [HttpPost]
    public async Task<IActionResult> CreateApplication([FromBody] CreateOidcApplicationRequest request)
    {
        var result = await _oidcAppService.CreateApplicationAsync(request);
        return Ok(result);
    }

    /// <summary>
    /// 获取所有 OIDC 应用列表
    /// </summary>
    [HttpGet]
    public async Task<IActionResult> GetApplications()
    {
        var apps = await _oidcAppService.GetApplicationsAsync();
        return Ok(apps);
    }

    /// <summary>
    /// 根据 ClientId 获取应用详情
    /// </summary>
    [HttpGet("{clientId}")]
    public async Task<IActionResult> GetApplication(string clientId)
    {
        var app = await _oidcAppService.GetApplicationByClientIdAsync(clientId);
        if (app == null) return NotFound();
        return Ok(app);
    }

    /// <summary>
    /// 重置应用密钥
    /// </summary>
    [HttpPost("{clientId}/reset-secret")]
    public async Task<IActionResult> ResetSecret(string clientId)
    {
        try
        {
            var newSecret = await _oidcAppService.ResetClientSecretAsync(clientId);
            return Ok(new { clientSecret = newSecret });
        }
        catch (InvalidOperationException ex)
        {
            return NotFound(ex.Message);
        }
    }

    /// <summary>
    /// 删除应用
    /// </summary>
    [HttpDelete("{clientId}")]
    public async Task<bool> DeleteApplication(string clientId)
    {
        await _oidcAppService.DeleteApplicationAsync(clientId);
        return true;
    }
}
