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
    [SkipGlobalResult]
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
                // 检查是否是 prompt=none 请求（静默认证）
                if (request.HasPromptValue(PromptValues.None))
                {
                    // 对于 prompt=none，如果用户未登录，必须返回错误而不是重定向到登录页
                    Console.WriteLine($"[OIDC Authorize] prompt=none but user not authenticated, returning error");
                    return Forbid(
                        authenticationSchemes: OpenIddictServerAspNetCoreDefaults.AuthenticationScheme,
                        properties: new AuthenticationProperties(new Dictionary<string, string?>
                        {
                            [OpenIddictServerAspNetCoreConstants.Properties.Error] = Errors.LoginRequired,
                            [OpenIddictServerAspNetCoreConstants.Properties.ErrorDescription] = "The user is not logged in."
                        }));
                }
                
                // 直接重定向到登录页面，并携带 returnUrl
                var returnUrl = $"/connect/authorize{HttpContext.Request.QueryString}";
                Console.WriteLine($"[OIDC Authorize] User not authenticated, redirecting to login with returnUrl: {returnUrl}");
                return Redirect($"/oidc/login?returnUrl={Uri.EscapeDataString(returnUrl)}");
            }
            
            Console.WriteLine($"[OIDC Authorize] User authenticated, proceeding with authorization");

            // 检查是否需要用户同意 (Consent)
            // 这里简化处理：如果应用要求 Explicit 同意且未携带同意标识，则显示同意页
            var consentType = await applicationManager.GetConsentTypeAsync(application);
            
            // 检查是否已经用户同意过（通过 URL 参数标记）
            var consentGiven = HttpContext.Request.Query["consent_given"].ToString() == "true";
            
            if (consentType == OpenIddict.Abstractions.OpenIddictConstants.ConsentTypes.Explicit && 
                !request.HasPromptValue(OpenIddict.Abstractions.OpenIddictConstants.PromptValues.None) &&
                !consentGiven)
            {
                // 构造同意页 HTML，使用 JavaScript 处理
                var appName = await applicationManager.GetDisplayNameAsync(application) ?? request.ClientId;
                var scopes = string.Join(", ", request.GetScopes());
                var consentHtml = $@"<!DOCTYPE html>
<html lang=""zh-CN"">
<head>
    <meta charset=""UTF-8"">
    <meta name=""viewport"" content=""width=device-width, initial-scale=1.0"">
    <title>授权确认 - Fastdotnet OIDC</title>
    <style>
        :root {{ --primary: #4f46e5; --bg: #f3f4f6; }}
        body {{ display: flex; justify-content: center; align-items: center; min-height: 100vh; background: var(--bg); font-family: sans-serif; }}
        .card {{ background: white; padding: 2rem; border-radius: 12px; box-shadow: 0 4px 6px rgba(0,0,0,0.1); max-width: 450px; width: 100%; text-align: center; }}
        h2 {{ color: #111827; margin-bottom: 1rem; }}
        p {{ color: #4b5563; margin-bottom: 1.5rem; line-height: 1.6; }}
        .scopes {{ background: #f9fafb; padding: 1rem; border-radius: 8px; margin-bottom: 2rem; text-align: left; font-size: 0.9rem; color: #374151; }}
        .actions {{ display: flex; gap: 1rem; }}
        button {{ flex: 1; padding: 0.75rem; border-radius: 8px; font-weight: 600; cursor: pointer; border: none; transition: 0.2s; }}
        .btn-allow {{ background: var(--primary); color: white; }}
        .btn-allow:hover {{ background: #4338ca; }}
        .btn-deny {{ background: #e5e7eb; color: #374151; }}
        .btn-deny:hover {{ background: #d1d5db; }}
    </style>
</head>
<body>
    <div class=""card"">
        <h2>授权请求</h2>
        <p><strong>{System.Net.WebUtility.HtmlEncode(appName)}</strong> 申请访问您的以下信息：</p>
        <div class=""scopes"">{System.Net.WebUtility.HtmlEncode(scopes)}</div>
        <div class=""actions"">
            <button onclick=""handleDeny()"" class=""btn-deny"">拒绝</button>
            <button onclick=""handleAllow()"" class=""btn-allow"">允许</button>
        </div>
    </div>
    <script>
        function handleAllow() {{
            // 用户允许，添加 consent_given 参数并重新加载
            const url = new URL(window.location.href);
            url.searchParams.set('consent_given', 'true');
            window.location.href = url.toString();
        }}
        
        function handleDeny() {{
            // 用户拒绝，重定向到回调地址
            const urlParams = new URLSearchParams(window.location.search);
            const redirectUri = urlParams.get('redirect_uri');
            const state = urlParams.get('state');
            
            if (redirectUri) {{
                let errorUrl = redirectUri + '?error=access_denied&error_description=User+denied+the+consent';
                if (state) {{
                    errorUrl += '&state=' + encodeURIComponent(state);
                }}
                window.location.href = errorUrl;
            }} else {{
                alert('授权请求无效');
            }}
        }}
    </script>
</body>
</html>";
                return Content(consentHtml, "text/html; charset=utf-8");
            }

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

                // 显式设置 Audience 为客户端 ID
                // 1. 设置 Access Token 的受众为你的 API 标识
                // 这里的字符串应该对应你在 Elsa 配置中预期的 API 受众
                //claimsPrincipal.SetAudiences("fastdotnet-api");
                claimsPrincipal.SetAudiences(new[] { "elsa-workflows", "fastdotnet-api" });
                // 2. 设置 ID Token 的展示者（可选，增加兼容性）
                claimsPrincipal.SetPresenters("elsa-workflows");
                claimsPrincipal.AddClaim(OpenIddictConstants.Claims.Role, "admin");
                claimsPrincipal.SetCreationDate(DateTimeOffset.UtcNow.AddMinutes(-5));
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
                    //claim.SetDestinations(destinations);
                    claim.SetDestinations(Destinations.AccessToken, Destinations.IdentityToken);
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
