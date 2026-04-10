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

            var formAction = returnUrl != null ? $"/oidc/login?returnUrl={Uri.EscapeDataString(returnUrl)}" : "/oidc/login";
            var errorHtml = error != null ? $@"<div class=""error-box"">
                <svg width=""20"" height=""20"" viewBox=""0 0 24 24"" fill=""none"" stroke=""currentColor"" stroke-width=""2""><circle cx=""12"" cy=""12"" r=""10""/><line x1=""12"" y1=""8"" x2=""12"" y2=""12""/><line x1=""12"" y1=""16"" x2=""12.01"" y2=""16""/></svg>
                <span>{System.Net.WebUtility.HtmlEncode(error)}</span>
            </div>" : "";

            var html = $@"<!DOCTYPE html>
<html lang=""zh-CN"">
<head>
    <meta charset=""UTF-8"">
    <meta name=""viewport"" content=""width=device-width, initial-scale=1.0"">
    <title>Fastdotnet OIDC 登录</title>
    <style>
        :root {{ --primary: #4f46e5; --primary-hover: #4338ca; --bg: #f3f4f6; }}
        * {{ box-sizing: border-box; margin: 0; padding: 0; font-family: -apple-system, BlinkMacSystemFont, 'Segoe UI', Roboto, sans-serif; }}
        body {{ display: flex; justify-content: center; align-items: center; min-height: 100vh; background: var(--bg); background-image: radial-gradient(#e5e7eb 1px, transparent 1px); background-size: 20px 20px; }}
        .login-card {{ background: white; padding: 2.5rem; border-radius: 16px; box-shadow: 0 10px 25px -5px rgba(0, 0, 0, 0.1), 0 8px 10px -6px rgba(0, 0, 0, 0.1); width: 100%; max-width: 400px; }}
        .header {{ text-align: center; margin-bottom: 2rem; }}
        .header h1 {{ color: #111827; font-size: 1.5rem; font-weight: 700; margin-bottom: 0.5rem; }}
        .header p {{ color: #6b7280; font-size: 0.875rem; }}
        .form-group {{ margin-bottom: 1.25rem; }}
        label {{ display: block; font-size: 0.875rem; font-weight: 500; color: #374151; margin-bottom: 0.5rem; }}
        input {{ width: 100%; padding: 0.75rem 1rem; border: 1px solid #d1d5db; border-radius: 8px; font-size: 0.95rem; transition: all 0.2s; outline: none; }}
        input:focus {{ border-color: var(--primary); box-shadow: 0 0 0 3px rgba(79, 70, 229, 0.1); }}
        button {{ width: 100%; padding: 0.75rem; background: var(--primary); color: white; border: none; border-radius: 8px; font-weight: 600; cursor: pointer; transition: background 0.2s; font-size: 1rem; margin-top: 1rem; }}
        button:hover {{ background: var(--primary-hover); }}
        .error-box {{ display: flex; align-items: center; gap: 0.75rem; padding: 0.75rem; background: #fef2f2; border: 1px solid #fecaca; color: #b91c1c; border-radius: 8px; margin-bottom: 1.5rem; font-size: 0.875rem; }}
        .footer {{ text-align: center; margin-top: 1.5rem; font-size: 0.75rem; color: #9ca3af; }}
    </style>
</head>
<body>
    <div class=""login-card"">
        <div class=""header"">
            <h1>欢迎回来</h1>
            <p>请使用您的账号进行授权登录</p>
        </div>
        {errorHtml}
        <form method='post' action='{formAction}'>
            <div class=""form-group"">
                <label for=""username"">用户名</label>
                <input type=""text"" id=""username"" name=""username"" placeholder=""请输入用户名"" required autofocus>
            </div>
            <div class=""form-group"">
                <label for=""password"">密码</label>
                <input type=""password"" id=""password"" name=""password"" placeholder=""请输入密码"" required>
            </div>
            <button type=""submit"">立即登录</button>
        </form>
        <div class=""footer"">Protected by Fastdotnet OIDC</div>
    </div>
</body>
</html>";

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
