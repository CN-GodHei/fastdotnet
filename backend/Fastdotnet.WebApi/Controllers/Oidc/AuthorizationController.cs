using System.Security.Claims;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OpenIddict.Abstractions;
using OpenIddict.Server.AspNetCore;
using Fastdotnet.Core.Entities.Admin;
using Fastdotnet.Core.Entities.App;
using Fastdotnet.Service.IService.Sys;
using Microsoft.Extensions.Options;
using Fastdotnet.Core.Settings;
using Fastdotnet.Core.Dtos.Auth;

namespace Fastdotnet.WebApi.Controllers.Oidc
{
    using static OpenIddict.Abstractions.OpenIddictConstants;
    
    /// <summary>
    /// OIDC 授权控制器
    /// 处理 OAuth2/OIDC 授权流程
    /// </summary>
    [ApiController]
    [Route("connect")]
    [SkipAntiReplayAttribute]
    public class AuthorizationController : ControllerBase
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly IAuthService _authService;
        private readonly OidcSettings _oidcSettings;

        public AuthorizationController(
            IServiceProvider serviceProvider,
            IAuthService authService,
            IOptions<OidcSettings> oidcSettings)
        {
            _serviceProvider = serviceProvider;
            _authService = authService;
            _oidcSettings = oidcSettings.Value;
        }

        /// <summary>
        /// 获取 OpenIddictApplicationManager（延迟获取）
        /// </summary>
        private IOpenIddictApplicationManager? GetApplicationManager()
        {
            return _serviceProvider.GetService<IOpenIddictApplicationManager>();
        }

        /// <summary>
        /// 获取 OpenIddictScopeManager（延迟获取）
        /// </summary>
        private IOpenIddictScopeManager? GetScopeManager()
        {
            return _serviceProvider.GetService<IOpenIddictScopeManager>();
        }

        /// <summary>
        /// OIDC 授权端点
        /// GET /connect/authorize
        /// </summary>
        [HttpGet("~/connect/authorize")]
        [HttpPost("~/connect/authorize")]
        [IgnoreAntiforgeryToken]
        public async Task<IActionResult> Authorize()
        {
            var request = HttpContext.GetOpenIddictServerRequest() ??
                throw new InvalidOperationException("The OpenID Connect request cannot be retrieved.");

            // 获取 ApplicationManager
            var applicationManager = GetApplicationManager() ??
                throw new InvalidOperationException("IOpenIddictApplicationManager is not available. Make sure OIDC is enabled.");

            // 检索应用程序以确保它已注册且有效
            var application = await applicationManager.FindByClientIdAsync(request.ClientId) ??
                throw new InvalidOperationException("Details concerning the calling client application cannot be found.");

            // 如果用户未认证，重定向到登录页面
            if (!User.Identity?.IsAuthenticated ?? true)
            {
                // 使用 Cookie 认证方案触发登录重定向
                return Challenge(
                    authenticationSchemes: "Identity.Application",
                    properties: new AuthenticationProperties
                    {
                        RedirectUri = HttpContext.Request.Path + HttpContext.Request.QueryString
                    });
            }

            // 创建认证票据
            var claims = new List<Claim>
            {
                // 添加主题标识符（sub claim）
                new Claim(Claims.Subject, User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? 
                                              User.FindFirst("nameid")?.Value ?? 
                                              throw new InvalidOperationException("Subject identifier not found.")),
                
                // 添加用户名
                new Claim(Claims.Name, User.Identity?.Name ?? string.Empty),
            };

            // 添加角色信息
            var roles = User.FindAll(ClaimTypes.Role).Select(r => r.Value);
            foreach (var role in roles)
            {
                claims.Add(new Claim(Claims.Role, role));
            }

            // 添加邮箱（如果有）
            var email = User.FindFirst(ClaimTypes.Email)?.Value;
            if (!string.IsNullOrEmpty(email))
            {
                claims.Add(new Claim(Claims.Email, email));
            }

            var claimsIdentity = new ClaimsIdentity(claims, OpenIddictServerAspNetCoreDefaults.AuthenticationScheme);
            var claimsPrincipal = new ClaimsPrincipal(claimsIdentity);

            // 设置作用域
            claimsPrincipal.SetScopes(request.GetScopes());

            // 签署认证票据
            return SignIn(claimsPrincipal, OpenIddictServerAspNetCoreDefaults.AuthenticationScheme);
        }

        /// <summary>
        /// OIDC Token 端点
        /// POST /connect/token
        /// </summary>
        [HttpPost("~/connect/token")]
        [Produces("application/json")]
        public async Task<IActionResult> Exchange()
        {
            var request = HttpContext.GetOpenIddictServerRequest() ??
                throw new InvalidOperationException("The OpenID Connect request cannot be retrieved.");

            if (request.IsAuthorizationCodeGrantType() || request.IsRefreshTokenGrantType())
            {
                // 检索身份验证结果以提取用户标识及其属性
                var result = await HttpContext.AuthenticateAsync(OpenIddictServerAspNetCoreDefaults.AuthenticationScheme);

                if (result?.Principal == null)
                {
                    return Forbid(
                        authenticationSchemes: OpenIddictServerAspNetCoreDefaults.AuthenticationScheme,
                        properties: new AuthenticationProperties(new Dictionary<string, string?>
                        {
                            [OpenIddictServerAspNetCoreConstants.Properties.Error] = Errors.InvalidGrant,
                            [OpenIddictServerAspNetCoreConstants.Properties.ErrorDescription] = "The token is no longer valid."
                        }));
                }

                // 确保用户仍然是有效的
                // TODO: 根据实际需求验证用户状态

                // 创建新的认证票据
                var claimsPrincipal = result.Principal;
                
                // 设置作用域（从 request 中获取）
                claimsPrincipal.SetScopes(request.GetScopes());

                return SignIn(claimsPrincipal, OpenIddictServerAspNetCoreDefaults.AuthenticationScheme);
            }

            throw new InvalidOperationException("The specified grant type is not supported.");
        }

        /// <summary>
        /// OIDC 登出端点
        /// GET /connect/logout
        /// </summary>
        [HttpGet("~/connect/logout")]
        [HttpPost("~/connect/logout")]
        [IgnoreAntiforgeryToken]
        public async Task<IActionResult> Logout()
        {
            // 清除认证 Cookie
            await HttpContext.SignOutAsync();

            // 获取登出请求
            var request = HttpContext.GetOpenIddictServerRequest();
            
            if (request != null && !string.IsNullOrEmpty(request.PostLogoutRedirectUri))
            {
                return Redirect(request.PostLogoutRedirectUri);
            }

            return Ok(new { message = "Logged out successfully" });
        }

        /// <summary>
        /// 用户登录端点（供前端调用）
        /// POST /connect/login
        /// </summary>
        [HttpPost("~/connect/login")]
        [AllowAnonymous]
        public async Task<IActionResult> Login([FromBody] LoginRequest model)
        {
            try
            {
                // 尝试验证用户（支持管理员和普通用户）
                var loginDto = new LoginDto
                {
                    Username = model.Username,
                    Password = model.Password
                };
                
                // 先尝试管理员登录，如果失败再尝试普通用户登录
                string? token = null;
                try
                {
                    token = await _authService.LoginAsync(loginDto, "Admin");
                }
                catch
                {
                    try
                    {
                        token = await _authService.LoginAsync(loginDto, "App");
                    }
                    catch
                    {
                        return Unauthorized(new { error = "invalid_grant", error_description = "用户名或密码错误" });
                    }
                }
                
                if (string.IsNullOrEmpty(token))
                {
                    return Unauthorized(new { error = "invalid_grant", error_description = "用户名或密码错误" });
                }

                // 创建 Claims
                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.NameIdentifier, model.Username),
                    new Claim(ClaimTypes.Name, model.Username),
                    new Claim("nameid", model.Username), // OpenIddict 需要的 subject claim
                };

                // 添加角色（这里简化处理，实际应该从数据库查询）
                claims.Add(new Claim(ClaimTypes.Role, "user"));

                // 注意：这个端点不签署 Cookie，只返回 JWT Token
                // 如果需要交互式登录，应该使用 /connect/authorize 端点
                
                return Ok(new { 
                    success = true, 
                    message = "登录成功",
                    token = token,
                    username = model.Username
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = "invalid_request", error_description = ex.Message });
            }
        }
    }

    /// <summary>
    /// 登录请求模型
    /// </summary>
    public class LoginRequest
    {
        public string Username { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
    }
}
