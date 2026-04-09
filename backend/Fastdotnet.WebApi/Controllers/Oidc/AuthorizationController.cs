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
        [AllowAnonymous]
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

            // 调试：输出当前请求的认证信息
            Console.WriteLine($"[OIDC Authorize] Request URL: {HttpContext.Request.Path}{HttpContext.Request.QueryString}");
            Console.WriteLine($"[OIDC Authorize] User.Identity?.IsAuthenticated (default): {User.Identity?.IsAuthenticated}");
            
            // 明确检查 Cookie 认证状态（使用 "Identity.Application" 方案）
            var cookieAuthResult = await HttpContext.AuthenticateAsync("Identity.Application");
            var isAuthenticated = cookieAuthResult.Succeeded && cookieAuthResult.Principal != null;
            
            Console.WriteLine($"[OIDC Authorize] Cookie Auth Result - Succeeded: {cookieAuthResult.Succeeded}, IsAuthenticated: {isAuthenticated}");
            
            if (!isAuthenticated)
            {
                // 直接重定向到登录页面，并携带 returnUrl
                var returnUrl = $"/connect/authorize{HttpContext.Request.QueryString}";
                Console.WriteLine($"[OIDC Authorize] User not authenticated, redirecting to login with returnUrl: {returnUrl}");
                return Redirect($"/oidc/login?returnUrl={Uri.EscapeDataString(returnUrl)}");
            }
            
            Console.WriteLine($"[OIDC Authorize] User authenticated, proceeding with authorization");

            // 创建认证票据
            var claims = new List<Claim>
            {
                // 添加主题标识符（sub claim）
                new Claim(Claims.Subject, cookieAuthResult.Principal!.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? 
                                              cookieAuthResult.Principal.FindFirst("nameid")?.Value ?? 
                                              throw new InvalidOperationException("Subject identifier not found.")),
                
                // 添加用户名
                new Claim(Claims.Name, cookieAuthResult.Principal.Identity?.Name ?? string.Empty),
            };

            // 添加角色信息
            var roles = cookieAuthResult.Principal.FindAll(ClaimTypes.Role).Select(r => r.Value);
            foreach (var role in roles)
            {
                claims.Add(new Claim(Claims.Role, role));
            }

            // 添加邮箱（如果有）
            var email = cookieAuthResult.Principal.FindFirst(ClaimTypes.Email)?.Value;
            if (!string.IsNullOrEmpty(email))
            {
                claims.Add(new Claim(Claims.Email, email));
            }

            Console.WriteLine($"[OIDC Authorize] Creating authentication ticket for user: {claims.First(c => c.Type == Claims.Name).Value}");

            var claimsIdentity = new ClaimsIdentity(claims, OpenIddictServerAspNetCoreDefaults.AuthenticationScheme);
            var claimsPrincipal = new ClaimsPrincipal(claimsIdentity);

            // 设置作用域
            claimsPrincipal.SetScopes(request.GetScopes());

            // 设置 Claims Destinations
            foreach (var claim in claimsPrincipal.Claims)
            {
                var destinations = new List<string> { Destinations.AccessToken };
                if (claim.Type == Claims.Subject || 
                    (claim.Type == Claims.Name && claimsPrincipal.HasScope(Scopes.Profile)) ||
                    (claim.Type == Claims.Email && claimsPrincipal.HasScope(Scopes.Email)) ||
                    (claim.Type == Claims.Role && claimsPrincipal.HasScope(Scopes.Roles)))
                {
                    destinations.Add(Destinations.IdentityToken);
                }
                claim.SetDestinations(destinations);
            }

            // 签署认证票据
            return SignIn(claimsPrincipal, OpenIddictServerAspNetCoreDefaults.AuthenticationScheme);
        }

        /// <summary>
        /// OIDC Token 端点
        /// POST /connect/token
        /// </summary>
        [HttpPost("~/connect/token")]
        [Produces("application/json")]
        [AllowAnonymous]
        [IgnoreAntiforgeryToken]
        public async Task<IActionResult> Exchange()
        {
            Console.WriteLine($"[OIDC Token] ===== REQUEST RECEIVED =====");
            Console.WriteLine($"[OIDC Token] Method: {HttpContext.Request.Method}");
            Console.WriteLine($"[OIDC Token] Path: {HttpContext.Request.Path}");
            
            var request = HttpContext.GetOpenIddictServerRequest() ??
                throw new InvalidOperationException("The OpenID Connect request cannot be retrieved.");

            Console.WriteLine($"[OIDC Token] Grant Type: {request.GrantType}");
            Console.WriteLine($"[OIDC Token] Client ID: {request.ClientId}");
            Console.WriteLine($"[OIDC Token] Redirect URI: {request.RedirectUri}");
            if (request.IsAuthorizationCodeGrantType())
            {
                Console.WriteLine($"[OIDC Token] Code: [REDACTED]");
                Console.WriteLine($"[OIDC Token] Code Verifier: [REDACTED]");
            }

            if (request.IsAuthorizationCodeGrantType())
            {
                // 授权码流程：OpenIddict 已经验证了授权码，我们只需要检索结果
                var result = await HttpContext.AuthenticateAsync(OpenIddictServerAspNetCoreDefaults.AuthenticationScheme);

                if (result?.Principal == null)
                {
                    Console.WriteLine("[OIDC Token] ERROR: Authentication result is null or principal is missing");
                    return Forbid(
                        authenticationSchemes: OpenIddictServerAspNetCoreDefaults.AuthenticationScheme,
                        properties: new AuthenticationProperties(new Dictionary<string, string?>
                        {
                            [OpenIddictServerAspNetCoreConstants.Properties.Error] = Errors.InvalidGrant,
                            [OpenIddictServerAspNetCoreConstants.Properties.ErrorDescription] = "The authorization code is invalid or has expired."
                        }));
                }

                Console.WriteLine($"[OIDC Token] User authenticated: {result.Principal.Identity?.Name}");

                // 创建新的认证票据
                var claimsPrincipal = result.Principal;

                // 设置 Claims Destinations
                foreach (var claim in claimsPrincipal.Claims)
                {
                    var destinations = new List<string> { Destinations.AccessToken };
                    if (claim.Type == Claims.Subject || 
                        (claim.Type == Claims.Name && claimsPrincipal.HasScope(Scopes.Profile)) ||
                        (claim.Type == Claims.Email && claimsPrincipal.HasScope(Scopes.Email)) ||
                        (claim.Type == Claims.Role && claimsPrincipal.HasScope(Scopes.Roles)))
                    {
                        destinations.Add(Destinations.IdentityToken);
                    }
                    claim.SetDestinations(destinations);
                }

                Console.WriteLine($"[OIDC Token] Creating tokens for user: {claimsPrincipal.Identity?.Name}");
                return SignIn(claimsPrincipal, OpenIddictServerAspNetCoreDefaults.AuthenticationScheme);
            }
            else if (request.IsRefreshTokenGrantType())
            {
                // 刷新令牌流程
                var result = await HttpContext.AuthenticateAsync(OpenIddictServerAspNetCoreDefaults.AuthenticationScheme);

                if (result?.Principal == null)
                {
                    return Forbid(
                        authenticationSchemes: OpenIddictServerAspNetCoreDefaults.AuthenticationScheme,
                        properties: new AuthenticationProperties(new Dictionary<string, string?>
                        {
                            [OpenIddictServerAspNetCoreConstants.Properties.Error] = Errors.InvalidGrant,
                            [OpenIddictServerAspNetCoreConstants.Properties.ErrorDescription] = "The refresh token is invalid or has expired."
                        }));
                }

                var claimsPrincipal = result.Principal;

                foreach (var claim in claimsPrincipal.Claims)
                {
                    var destinations = new List<string> { Destinations.AccessToken };
                    if (claim.Type == Claims.Subject || 
                        (claim.Type == Claims.Name && claimsPrincipal.HasScope(Scopes.Profile)) ||
                        (claim.Type == Claims.Email && claimsPrincipal.HasScope(Scopes.Email)) ||
                        (claim.Type == Claims.Role && claimsPrincipal.HasScope(Scopes.Roles)))
                    {
                        destinations.Add(Destinations.IdentityToken);
                    }
                    claim.SetDestinations(destinations);
                }

                return SignIn(claimsPrincipal, OpenIddictServerAspNetCoreDefaults.AuthenticationScheme);
            }

            throw new InvalidOperationException($"The specified grant type '{request.GrantType}' is not supported.");
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
