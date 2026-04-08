using Fastdotnet.Core.Entities.Oidc;
using Fastdotnet.Core.Service.Oidc;
using Fastdotnet.Core.Service.Oidc.Stores;
using Fastdotnet.Core.Settings;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using OpenIddict.Abstractions;

namespace Fastdotnet.WebApi.Extensions;

/// <summary>
/// OpenIddict 配置扩展方法
/// </summary>
public static class OpenIddictConfigurationExtensions
{
    /// <summary>
    /// 添加 OpenIddict OIDC/OAuth2 服务
    /// </summary>
    public static void AddOpenIddictServices(this IServiceCollection services, IConfiguration configuration)
    {
        var oidcSettings = configuration.GetSection("OidcSettings").Get<OidcSettings>() ?? new OidcSettings();
        
        if (!oidcSettings.Enabled)
        {
            return;
        }

        services.Configure<OidcSettings>(configuration.GetSection("OidcSettings"));

        // 注册 OpenIddict 服务（使用 SqlSugar Store）
        services.AddOpenIddict()
            .AddCore(options =>
            {
                // 使用 SqlSugar 存储 OpenIddict 数据
                options.UseSqlSugar();
            })
            .AddServer(options =>
            {
                // 启用授权端点和令牌端点
                options.SetAuthorizationEndpointUris("connect/authorize")
                       .SetTokenEndpointUris("connect/token")
                       .SetIntrospectionEndpointUris("connect/introspect")
                       .SetRevocationEndpointUris("connect/revoke");

                // 允许授权码流和刷新令牌流
                options.AllowAuthorizationCodeFlow()
                       .AllowRefreshTokenFlow();

                // 注册作用域
                options.RegisterScopes(
                    OpenIddictConstants.Scopes.OpenId,
                    OpenIddictConstants.Scopes.Profile,
                    OpenIddictConstants.Scopes.Email,
                    OpenIddictConstants.Scopes.Roles);

                // 注册签名和加密证书（开发环境使用临时证书）
                options.AddDevelopmentEncryptionCertificate()
                       .AddDevelopmentSigningCertificate();

                // 注册 ASP.NET Core 宿主并配置选项
                options.UseAspNetCore()
                       .EnableStatusCodePagesIntegration()
                       .EnableAuthorizationEndpointPassthrough()
                       .EnableTokenEndpointPassthrough()
                       .DisableTransportSecurityRequirement(); // 开发环境禁用 HTTPS 要求
            })
            .AddValidation(options =>
            {
                // 使用本地服务器验证令牌
                options.UseLocalServer();
                options.UseAspNetCore();
            });

        // 注册 OIDC 应用初始化器
        // services.AddScoped<Fastdotnet.Core.Initializers.IApplicationInitializer, 
        //     Fastdotnet.Core.Service.Oidc.OidcApplicationInitializer>();

        // 配置 OIDC Cookie 认证（用于授权端点的用户会话）
        services.AddAuthentication(options =>
        {
            options.DefaultScheme = "Identity.Application";
        })
        .AddCookie("Identity.Application", options =>
        {
            options.Cookie.Name = ".Fastdotnet.OIDC";
            options.ExpireTimeSpan = TimeSpan.FromHours(8);
            options.SlidingExpiration = true;
            
            // 开发环境：使用简单的登录页面
            // 生产环境：可以重定向到 Vue 前端登录页
            options.LoginPath = "/oidc/login";
            options.LogoutPath = "/oidc/logout";
            options.AccessDeniedPath = "/oidc/access-denied";
        });
    }

    /// <summary>
    /// 配置 JWT Bearer 认证（用于 API 保护）
    /// </summary>
    public static AuthenticationBuilder AddJwtBearerAuthentication(this IServiceCollection services, IConfiguration configuration)
    {
        return services.AddAuthentication(options =>
        {
            options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        })
        .AddJwtBearer(options =>
        {
            var jwtSettings = configuration.GetSection("JwtSettings").Get<Fastdotnet.Core.Settings.JwtSettings>() 
                ?? throw new InvalidOperationException("JwtSettings not configured.");
            
            options.TokenValidationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,
                ValidIssuer = jwtSettings.Issuer,
                ValidAudience = jwtSettings.Audience,
                IssuerSigningKey = new Microsoft.IdentityModel.Tokens.SymmetricSecurityKey(
                    System.Text.Encoding.UTF8.GetBytes(jwtSettings.SecretKey)),
                
                // 配置 Claim 类型映射，确保 SignalR 能正确识别用户 ID
                NameClaimType = Microsoft.IdentityModel.JsonWebTokens.JwtRegisteredClaimNames.NameId,
                RoleClaimType = System.Security.Claims.ClaimTypes.Role
            };
            
            // 处理 SignalR 的 Token 传递（从 Query String 读取）
            options.Events = new Microsoft.AspNetCore.Authentication.JwtBearer.JwtBearerEvents
            {
                OnMessageReceived = context =>
                {
                    var accessToken = context.Request.Query["access_token"];
                    
                    // 如果请求路径是 SignalR Hub，则从 Query String 读取 Token
                    var path = context.HttpContext.Request.Path;
                    if (!string.IsNullOrEmpty(accessToken) && path.StartsWithSegments("/universalhub"))
                    {
                        context.Token = accessToken;
                    }
                    
                    return System.Threading.Tasks.Task.CompletedTask;
                }
            };
        });
    }
}
