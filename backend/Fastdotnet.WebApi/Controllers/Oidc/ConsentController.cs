using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OpenIddict.Abstractions;
using Fastdotnet.Core.Entities.Oidc;
using SqlSugar;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore;

namespace Fastdotnet.WebApi.Controllers.Oidc;

/// <summary>
/// OIDC 授权同意控制器
/// </summary>
[ApiController]
[Route("oidc")]
[Authorize] // 必须先登录
[SkipAntiReplayAttribute]
public class ConsentController : Controller
{
    private readonly IOpenIddictApplicationManager _applicationManager;
    private readonly ISqlSugarClient _db;

    public ConsentController(IOpenIddictApplicationManager applicationManager, ISqlSugarClient db)
    {
        _applicationManager = applicationManager;
        _db = db;
    }

    /// <summary>
    /// 显示授权同意页面
    /// </summary>
    [HttpGet("consent")]
    public async Task<IActionResult> Index(string? error = null)
    {
        var request = HttpContext.GetOpenIddictServerRequest();
        if (request == null) return BadRequest("Invalid OIDC request.");

        var app = await _applicationManager.FindByClientIdAsync(request.ClientId);
        if (app == null) return NotFound("Application not found.");

        var model = new ConsentViewModel
        {
            ApplicationName = await _applicationManager.GetDisplayNameAsync(app) ?? request.ClientId,
            Scopes = request.GetScopes(),
            ReturnUrl = HttpContext.Request.Path + HttpContext.Request.QueryString
        };

        return View(model);
    }

    /// <summary>
    /// 处理用户同意或拒绝
    /// </summary>
    [HttpPost("consent")]
    public async Task<IActionResult> Index([FromForm] ConsentInputModel model)
    {
        if (model.Button != "yes")
        {
            // 用户拒绝授权
            return Forbid(
                authenticationSchemes: OpenIddict.Server.AspNetCore.OpenIddictServerAspNetCoreDefaults.AuthenticationScheme,
                properties: new AuthenticationProperties(new Dictionary<string, string?>
                {
                    [OpenIddict.Server.AspNetCore.OpenIddictServerAspNetCoreConstants.Properties.Error] = OpenIddict.Abstractions.OpenIddictConstants.Errors.AccessDenied,
                    [OpenIddict.Server.AspNetCore.OpenIddictServerAspNetCoreConstants.Properties.ErrorDescription] = "The user denied the consent."
                }));
        }

        // 用户同意，重定向回授权端点继续流程
        return Redirect(model.ReturnUrl ?? "/");
    }
}

public class ConsentViewModel
{
    public string ApplicationName { get; set; } = string.Empty;
    public IEnumerable<string> Scopes { get; set; } = Enumerable.Empty<string>();
    public string ReturnUrl { get; set; } = string.Empty;
}

public class ConsentInputModel
{
    public string Button { get; set; } = string.Empty; // "yes" or "no"
    public string ReturnUrl { get; set; } = string.Empty;
}
