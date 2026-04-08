using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Fastdotnet.Service.IService;
using Fastdotnet.Service.IService.Sys;
using Fastdotnet.Core.Dtos.Auth;
using Fastdotnet.Core.Entities.Admin;
using SqlSugar;

namespace Fastdotnet.WebApi.Controllers.Oidc
{
    /// <summary>
    /// OIDC 登录控制器（提供简单的 HTML 登录表单）
    /// </summary>
    [ApiController]
    [Route("oidc")]
    [AllowAnonymous]
    [SkipAntiReplay]
    [SkipGlobalResult]  // 跳过全局结果过滤器，直接返回 HTML
    [ApiUsageScope(ApiUsageScopeEnum.Both)]
    public class OidcLoginController : Controller
    {
        private readonly ISqlSugarClient _db;
        private readonly IPasswordService _passwordService;

        public OidcLoginController(ISqlSugarClient db, IPasswordService passwordService)
        {
            _db = db;
            _passwordService = passwordService;
        }

        /// <summary>
        /// OIDC 登录页面（GET）
        /// </summary>
        [HttpGet("login")]
        [AllowAnonymous]
        [SkipAntiReplay]
        [ApiUsageScope(ApiUsageScopeEnum.Both)]
        public IActionResult Login(string? returnUrl = null, string? error = null)
        {
            // 如果已经登录，直接重定向
            if (User.Identity?.IsAuthenticated == true)
            {
                return Redirect(returnUrl ?? "/");
            }

            // 返回简单的 HTML 登录表单（测试用）
            var returnUrlValue = returnUrl ?? "none";
            var formAction = returnUrl != null ? $"/oidc/login?returnUrl={Uri.EscapeDataString(returnUrl)}" : "/oidc/login";
            var errorHtml = error != null ? $"<p style='color:red;'>Error: {error}</p>" : "";
            var html = $@"<!DOCTYPE html>
<html><head><title>OIDC Login</title></head>
<body>
<h1>OIDC Login Test</h1>
{errorHtml}
<p>This is a test login page.</p>
<p>Return URL: {returnUrlValue}</p>
<form method='post' action='{formAction}'>
    <input type='text' name='username' placeholder='Username' required><br>
    <input type='password' name='password' placeholder='Password' required><br>
    <button type='submit'>Login</button>
</form>
</body></html>";

            return Content(html, "text/html; charset=utf-8");
        }

        /// <summary>
        /// OIDC 登录处理（POST）
        /// </summary>
        [HttpPost("login")]
        [AllowAnonymous]
        [SkipAntiReplay]
        [SkipGlobalResult]
        [ApiUsageScope(ApiUsageScopeEnum.Both)]
        [Consumes("application/x-www-form-urlencoded")]
        public async Task<IActionResult> Login([FromForm] LoginDto model, string? returnUrl = null)
        {
            if (string.IsNullOrEmpty(model.Username) || string.IsNullOrEmpty(model.Password))
            {
                return RedirectToAction(nameof(Login), new { returnUrl, error = "用户名和密码不能为空" });
            }

            try
            {
                // 查询用户
                var user = await _db.Queryable<FdAdminUser>()
                    .Where(u => u.Username == model.Username)
                    .FirstAsync();

                if (user == null)
                {
                    return RedirectToAction(nameof(Login), new { returnUrl, error = "用户名或密码错误" });
                }

                // 验证密码
                var isValid = await _passwordService.VerifyPasswordAsync(model.Password, user.Password);
                if (!isValid)
                {
                    return RedirectToAction(nameof(Login), new { returnUrl, error = "用户名或密码错误" });
                }

                // 创建 Claims
                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.NameIdentifier, user.Id),
                    new Claim(ClaimTypes.Name, user.Username),
                    new Claim(ClaimTypes.Email, user.Email ?? string.Empty),
                };

                var claimsIdentity = new ClaimsIdentity(claims, "Identity.Application");
                var claimsPrincipal = new ClaimsPrincipal(claimsIdentity);

                // 签署 Cookie
                await HttpContext.SignInAsync(
                    "Identity.Application",
                    claimsPrincipal,
                    new AuthenticationProperties
                    {
                        IsPersistent = true,
                        ExpiresUtc = DateTimeOffset.UtcNow.AddHours(8)
                    });

                // 调试：输出 Cookie 信息
                Console.WriteLine($"[OIDC Login] User '{user.Username}' logged in successfully.");
                Console.WriteLine($"[OIDC Login] SignInAsync completed.");
                
                // 检查响应头中是否有 Set-Cookie
                var setCookieHeader = HttpContext.Response.Headers["Set-Cookie"];
                if (setCookieHeader.Count > 0)
                {
                    Console.WriteLine($"[OIDC Login] Set-Cookie header present: {setCookieHeader.Count} cookie(s)");
                    foreach (var cookie in setCookieHeader)
                    {
                        // 只显示 Cookie 名称，不显示完整值
                        var cookieName = cookie?.Split('=')[0] ?? "unknown";
                        Console.WriteLine($"[OIDC Login]   - Cookie: {cookieName}");
                    }
                }
                else
                {
                    Console.WriteLine($"[OIDC Login] WARNING: No Set-Cookie header in response!");
                }

                // 重定向回 OIDC 授权端点
                if (string.IsNullOrEmpty(returnUrl))
                {
                    Console.WriteLine("[OIDC Login] WARNING: returnUrl is empty!");
                    return Redirect("/");
                }
                
                Console.WriteLine($"[OIDC Login] Redirecting to: {returnUrl}");
                return Redirect(returnUrl);
            }
            catch (Exception ex)
            {
                return RedirectToAction(nameof(Login), new { returnUrl, error = $"登录失败：{ex.Message}" });
            }
        }

        /// <summary>
        /// OIDC 登出
        /// </summary>
        [HttpGet("logout")]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync("Identity.Application");
            return RedirectToAction("Index", "Home");
        }

        /// <summary>
        /// 访问被拒绝页面
        /// </summary>
        [HttpGet("access-denied")]
        [AllowAnonymous]
        [SkipAntiReplay]
        [ApiUsageScope(ApiUsageScopeEnum.Both)]
        public IActionResult AccessDenied()
        {
            return Content("<h1>访问被拒绝</h1><p>您没有权限访问此资源。</p>", "text/html; charset=utf-8");
        }
    }
}
