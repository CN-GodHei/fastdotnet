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
        public IActionResult Login(string? returnUrl = null)
        {
            // 如果已经登录，直接重定向
            if (User.Identity?.IsAuthenticated == true)
            {
                return Redirect(returnUrl ?? "/");
            }

            // 返回简单的 HTML 登录表单
            var html = $@"<!DOCTYPE html>
<html lang=""zh-CN"">
<head>
    <meta charset=""UTF-8"">
    <meta name=""viewport"" content=""width=device-width, initial-scale=1.0"">
    <title>Fastdotnet OIDC 登录</title>
    <style>
        * {{ margin: 0; padding: 0; box-sizing: border-box; }}
        body {{ 
            font-family: -apple-system, BlinkMacSystemFont, 'Segoe UI', Roboto, 'Helvetica Neue', Arial, sans-serif;
            background: linear-gradient(135deg, #667eea 0%, #764ba2 100%);
            min-height: 100vh;
            display: flex;
            align-items: center;
            justify-content: center;
        }}
        .login-container {{
            background: white;
            padding: 40px;
            border-radius: 10px;
            box-shadow: 0 10px 40px rgba(0,0,0,0.2);
            width: 100%;
            max-width: 400px;
        }}
        h1 {{
            text-align: center;
            color: #333;
            margin-bottom: 30px;
            font-size: 24px;
        }}
        .form-group {{
            margin-bottom: 20px;
        }}
        label {{
            display: block;
            margin-bottom: 8px;
            color: #555;
            font-weight: 500;
        }}
        input[type=""text""],
        input[type=""password""] {{
            width: 100%;
            padding: 12px;
            border: 2px solid #e1e1e1;
            border-radius: 6px;
            font-size: 14px;
            transition: border-color 0.3s;
        }}
        input[type=""text""]:focus,
        input[type=""password""]:focus {{
            outline: none;
            border-color: #667eea;
        }}
        button {{
            width: 100%;
            padding: 12px;
            background: linear-gradient(135deg, #667eea 0%, #764ba2 100%);
            color: white;
            border: none;
            border-radius: 6px;
            font-size: 16px;
            font-weight: 600;
            cursor: pointer;
            transition: transform 0.2s, box-shadow 0.2s;
        }}
        button:hover {{
            transform: translateY(-2px);
            box-shadow: 0 5px 20px rgba(102, 126, 234, 0.4);
        }}
        button:active {{
            transform: translateY(0);
        }}
        .error {{
            background: #fee;
            color: #c33;
            padding: 12px;
            border-radius: 6px;
            margin-bottom: 20px;
            text-align: center;
        }}
        .info {{
            text-align: center;
            margin-top: 20px;
            color: #999;
            font-size: 12px;
        }}
    </style>
</head>
<body>
    <div class=""login-container"">
        <h1>🔐 Fastdotnet OIDC 登录</h1>
        {(TempData["Error"] != null ? $@"<div class=""error"">{TempData["Error"]}</div>" : "")}
        <form method=""post"" action=""/oidc/login{(returnUrl != null ? $"?returnUrl={returnUrl}" : "")}"">
            <div class=""form-group"">
                <label for=""username"">用户名</label>
                <input type=""text"" id=""username"" name=""username"" required autofocus placeholder=""请输入用户名"">
            </div>
            <div class=""form-group"">
                <label for=""password"">密码</label>
                <input type=""password"" id=""password"" name=""password"" required placeholder=""请输入密码"">
            </div>
            <button type=""submit"">登 录</button>
        </form>
        <div class=""info"">
            <p>这是 OIDC 授权登录页面</p>
            <p>登录后将自动完成授权</p>
        </div>
    </div>
</body>
</html>";

            return Content(html, "text/html; charset=utf-8");
        }

        /// <summary>
        /// OIDC 登录处理（POST）
        /// </summary>
        [HttpPost("login")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginDto model, string? returnUrl = null)
        {
            if (string.IsNullOrEmpty(model.Username) || string.IsNullOrEmpty(model.Password))
            {
                TempData["Error"] = "用户名和密码不能为空";
                return RedirectToAction(nameof(Login), new { returnUrl });
            }

            try
            {
                // 查询用户
                var user = await _db.Queryable<FdAdminUser>()
                    .Where(u => u.Username == model.Username)
                    .FirstAsync();

                if (user == null)
                {
                    TempData["Error"] = "用户名或密码错误";
                    return RedirectToAction(nameof(Login), new { returnUrl });
                }

                // 验证密码
                var isValid = await _passwordService.VerifyPasswordAsync(model.Password, user.Password);
                if (!isValid)
                {
                    TempData["Error"] = "用户名或密码错误";
                    return RedirectToAction(nameof(Login), new { returnUrl });
                }

                // 创建 Claims
                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.NameIdentifier, user.Id),
                    new Claim(ClaimTypes.Name, user.Username),
                    new Claim(ClaimTypes.Email, user.Email ?? string.Empty),
                };

                // TODO: 添加角色（需要查询用户角色）
                // var roles = await GetUserRolesAsync(user.Id);
                // foreach (var role in roles)
                // {
                //     claims.Add(new Claim(ClaimTypes.Role, role));
                // }

                var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
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

                // 重定向回 OIDC 授权端点
                return Redirect(returnUrl ?? "/");
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"登录失败：{ex.Message}";
                return RedirectToAction(nameof(Login), new { returnUrl });
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
        public IActionResult AccessDenied()
        {
            return Content("<h1>访问被拒绝</h1><p>您没有权限访问此资源。</p>", "text/html; charset=utf-8");
        }
    }
}
