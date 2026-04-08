using Autofac.Core;
using Fastdotnet.Core.Entities.Oidc;
using Fastdotnet.Core.Extensions;
using Fastdotnet.Core.Service.Oidc;
using Fastdotnet.Core.Service.Oidc.Stores;
using Fastdotnet.Core.Service.Sys;
using Fastdotnet.Core.Settings;
using Fastdotnet.Service.IService.App;
using Fastdotnet.Service.IService.Sys;
using Fastdotnet.Service.Service.Admin;
using Fastdotnet.Service.Service.App;
using Fastdotnet.Service.Service.Sys;
using System.IdentityModel.Tokens.Jwt;
using static System.Net.Mime.MediaTypeNames;

var builder = WebApplication.CreateBuilder(args);
// 可选：延长停机超时时间
builder.Host.ConfigureHostOptions(o => o.ShutdownTimeout = TimeSpan.FromMinutes(2));
var options = new IdGeneratorOptions(0001);
// options.WorkerIdBitLength = 10; // 默认值6，限定 WorkerId 最大值为2^6-1，即默认最多支持64个节点。
options.SeqBitLength = 10; // 默认值6，限制每毫秒生成的ID个数。若生成速度超过5万个/秒，建议加大 SeqBitLength 到 10。

YitIdHelper.SetIdGenerator(options);

// 添加CORS服务
builder.Services.AddCors(cors => cors.AddDefaultPolicy(policy =>
    policy.AllowAnyHeader()
         .AllowAnyMethod()
         .SetIsOriginAllowed(_ => true)  // 允许所有来源，但支持凭证
         .AllowCredentials()  // 允许携带凭证（cookies、authorization headers）
         .WithExposedHeaders("*")));

// 配置授权策略
builder.Services.AddAuthorization(options =>
{
    // 添加API作用域策略
    options.AddPolicy("ApiScope", policy =>
    {
        policy.Requirements.Add(new ApiScopeRequirement());
    });
});

// 1. 注册 ASP.NET Core 的核心服务
builder.Services.AddControllers(options =>
{
    var policy = new AuthorizationPolicyBuilder()
        .RequireAuthenticatedUser()
        .AddRequirements(new ApiScopeRequirement()) // 添加API作用域检查作为默认要求
        .Build();
    options.Filters.Add(new AuthorizeFilter(policy));

    //options.Filters.Add<GlobalExceptionFilter>();
    options.Filters.Add<GlobalResultFilter>(); // 添加全局结果过滤器
})
    .AddControllersAsServices()
.AddNewtonsoftJsonWithCustomSettings();
//模型验证失败，也会正常进入 Controller
builder.Services.Configure<ApiBehaviorOptions>(options =>
{
    options.SuppressModelStateInvalidFilter = true; // 关键：禁用自动 400 响应
});
builder.Services.AddEndpointsApiExplorer();
// 👇 添加自定义 Swagger
builder.Services.AddCustomSwagger();
builder.Services.AddSwaggerGenNewtonsoftSupport(); // ✅ Swagger启用 Newtonsoft 支持

builder.Services.AddSingleton<IActionDescriptorChangeProvider>(ActionDescriptorChangeProvider.Instance);
builder.Services.AddSingleton<DynamicMiddlewareRegistry>();
builder.Services.AddSingleton<Fastdotnet.Core.Plugin.PluginBranchRegistry>();

// 注册插件反代网关注册表（逃生舱专用）
builder.Services.AddSingleton<Fastdotnet.Core.Plugin.PluginReverseProxyRegistry>();
builder.Services.AddHttpForwarder();
builder.Services.AddSqlSugar(builder.Configuration);

// 注册 HttpContextAccessor 和 CurrentUser 服务
builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<ICurrentUser, CurrentUser>();
builder.Services.Configure<JwtSettings>(builder.Configuration.GetSection("JwtSettings"));

// 配置 OIDC/OAuth2 Identity Provider（OpenIddict）
var oidcSettings = builder.Configuration.GetSection("OidcSettings").Get<OidcSettings>() ?? new OidcSettings();
if (oidcSettings.Enabled)
{
    builder.Services.Configure<OidcSettings>(builder.Configuration.GetSection("OidcSettings"));

    // 注册 OpenIddict 服务（使用 SqlSugar Store）
    builder.Services.AddOpenIddict()
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
                   .EnableTokenEndpointPassthrough();

            // 禁用 HTTPS 要求（仅开发环境）
            if (!oidcSettings.RequireHttpsMetadata && builder.Environment.IsDevelopment())
            {
                // 注意：DisableTransportSecurityRequirement 在 UseAspNetCore() 之后调用
                // 这里通过配置实现
            }
        })
        .AddValidation(options =>
        {
            // 使用本地服务器验证令牌
            options.UseLocalServer();
            options.UseAspNetCore();
        });

    // 注册 OIDC 应用初始化器（仅在 OIDC 启用时注册）
    // builder.Services.AddScoped<IApplicationInitializer, Fastdotnet.Core.Service.Oidc.OidcApplicationInitializer>();

    Console.WriteLine("✅ OIDC/OAuth2 Identity Provider 已启用");
}
else
{
    Console.WriteLine("ℹ️ OIDC/OAuth2 Identity Provider 未启用（可在 appsettings.json 中配置 OidcSettings:Enabled=true 启用）");
    
    // 当 OIDC 未启用时，注册一个空的 AuthorizationController 以避免路由冲突
    // 或者我们可以简单地不注册任何 OIDC 相关的服务
}

// 添加认证和授权
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    // 此处需要手动读取配置，因为此时 DI 容器还未完全构建
    var jwtSettings = builder.Configuration.GetSection("JwtSettings").Get<JwtSettings>() ?? throw new InvalidOperationException("JwtSettings not configured.");
    
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = jwtSettings.Issuer,
        ValidAudience = jwtSettings.Audience,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings.SecretKey)),
        
        // 配置 Claim 类型映射，确保 SignalR 能正确识别用户 ID
        NameClaimType = JwtRegisteredClaimNames.NameId, // 使用 "nameid" 作为用户标识
        RoleClaimType = ClaimTypes.Role
    };
    
    // 处理 SignalR 的 Token 传递（从 Query String 读取）
    options.Events = new JwtBearerEvents
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
            
            return Task.CompletedTask;
        }
    };
});

// 注册日志服务
builder.Services.AddScoped<ILogService, LogService>();
//builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());
builder.Services.AddScoped<IUserRefFiller, UserRefFiller>();

// 注册字典数据初始化服务
builder.Services.AddScoped<IFdDictService, FdDictService>();

// 注册国家标准数据服务
builder.Services.AddScoped<IFdNationalStandardService, FdNationalStandardService>();
builder.Services.AddScoped<IFdNationalStandardItemService, FdNationalStandardItemService>();

// 注册应用服务和初始化器
// 注释：已在 Autofac 中注册
//builder.Services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
//builder.Services.AddScoped(typeof(IRepository<,>), typeof(Repository<,>));
//builder.Services.AddScoped(typeof(IBaseService<,>), typeof(BaseService<,>));
//builder.Services.AddScoped<IFdRoleInitializerService, FdRoleInitializerService>();
builder.Services.AddScoped<IAdminUserService, AdminUserService>();
builder.Services.AddScoped<IAppUserService, AppUserService>();
builder.Services.AddScoped<IAuthService, AuthService>();
// 扫描并注册所有 IApplicationInitializer 实现
builder.Services.Scan(scan => scan
    .FromAssemblies(AppDomain.CurrentDomain.GetAssemblies())
    .AddClasses(classes => classes.AssignableTo<IApplicationInitializer>())
    .AsImplementedInterfaces()
    .WithScopedLifetime());
builder.Services.AddScoped<IPermissionProvider, FrameworkPermissionProvider>();

// 注册全局异常过滤器
//builder.Services.AddScoped<GlobalExceptionFilter>();

// 添加内存缓存服务
//builder.Services.AddMemoryCache();

// 添加混合缓存服务
builder.Services.AddHybridCacheService(builder.Configuration);

// 注册邮件和验证码服务
builder.Services.AddScoped<IEmailService, EmailService>();
builder.Services.AddScoped<IVerificationCodeManager, VerificationCodeManager>();

// 扫描并注册所有验证码策略
builder.Services.Scan(scan => scan
    .FromAssemblies(AppDomain.CurrentDomain.GetAssemblies())
    .AddClasses(classes => classes.AssignableTo<IVerificationCodeStrategy>())
    .AsImplementedInterfaces()
    .WithScopedLifetime());

// 注册LAZY.CAPTCHA服务
builder.Services.AddCaptcha(builder.Configuration);

// 注册新权限同步服务
builder.Services.AddScoped<IPermissionSyncService, PermissionSyncService>();

// 注册授权处理器和策略提供者
builder.Services.AddSingleton<IAuthorizationHandler, PermissionHandler>();
builder.Services.AddSingleton<IAuthorizationHandler, ApiScopeHandler>();
builder.Services.AddSingleton<IAuthorizationPolicyProvider, PermissionPolicyProvider>();
builder.Services.AddScoped<IPermissionService, PermissionService>();
builder.Services.AddScoped<IMenuService, MenuService>();
builder.Services.AddScoped<ICodeGenConfigService, CodeGenConfigService>();

// 扫描并注册所有 IStartupTask 实现
builder.Services.Scan(scan => scan
    .FromAssemblies(AppDomain.CurrentDomain.GetAssemblies())
    .AddClasses(classes => classes.AssignableTo<IStartupTask>())
    .AsImplementedInterfaces()
    .WithScopedLifetime());

builder.Services.AddSingleton<IControllerActivator, PluginControllerActivator>();

// 注册限流和黑名单缓存服务
builder.Services.AddScoped<IRateLimitCacheService, RateLimitCacheService>();

// 添加 SignalR 服务
builder.Services.AddSignalR()
    .AddJsonProtocol(options =>
    {
        // 配置 JSON 序列化选项
        options.PayloadSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
    });

// 注册IStorageContext服务
builder.Services.AddScoped<IStorageContext, StorageContext>();

// 配置存储选项
builder.Services.Configure<StorageOptions>(options =>
{
    options.LocalStoragePath = builder.Configuration["Storage:LocalStoragePath"] ?? "wwwroot/uploads";
    options.BaseUrl = builder.Configuration["Storage:BaseUrl"] ?? "/uploads";
    options.DefaultBucket = builder.Configuration["Storage:DefaultBucket"] ?? "default";
});

// 注册存储服务代理作为 IStorageService 的实现
builder.Services.AddSingleton<StorageServiceProxy>();
builder.Services.AddScoped<IStorageService>(provider => 
    provider.GetRequiredService<StorageServiceProxy>());

// 2. 配置 Autofac 容器
// 👇 一行启用 Autofac + 自定义注册
builder.Host.UseAutofacWithCustomModules();

// 3. 构建并运行应用
var app = builder.Build();

// 执行应用初始化器
await app.UseApplicationInitializers();
// 👇 启用 Swagger（仅开发环境）
app.UseCustomSwagger();
// 4. 配置 HTTP 请求管道
if (app.Environment.IsDevelopment())
{
    app.UseCors();
}
// 只在生产环境中使用HTTPS重定向
//if (!app.Environment.IsDevelopment())
//{
//    app.UseHttpsRedirection();
//}
//app.UseHttpsRedirection();
app.UseRouting(); // 添加路由中间件
app.UseMiddleware<RequestIdMiddleware>();

// 🌟 启用插件逃生舱（微主机代理转发中间件）
// 必须放在防重放/加密中间件之前，以避免拦截第三方库（如 Elsa前端）的常规通讯
app.UseMiddleware<PluginReverseProxyMiddleware>();

// 注册防重放攻击中间件（在认证之前执行）
app.UseMiddleware<AntiReplayMiddleware>();
// 注册加密解密中间件
//if (!app.Environment.IsDevelopment())
//{
    app.UseMiddleware<EncryptionMiddleware>();
//}
app.UseMiddleware<GlobalExceptionMiddleware>();
app.UseMiddleware<OperationLogMiddleware>();
app.UseMiddleware<PluginStaticFileMiddleware>();

// 注册限流和黑名单中间件
app.UseMiddleware<BlacklistMiddleware>();
app.UseMiddleware<RateLimitMiddleware>();

app.UseMiddleware<DynamicMiddlewareDispatcher>();

// 启用插件分支网关 - 允许插件动态注册请求处理管道
app.UsePluginBranchGate();

app.UseAuthentication();
app.UseAuthorization();
// 👇 启用优雅停机
app.UseGracefulShutdown();
app.UseEndpoints(endpoints =>
{
    endpoints.MapControllers();
    // 注册主框架的SignalR端点
    endpoints.MapHub<UniversalHub>("/universalhub");
});

// 5. 应用程序启动后执行启动任务
app.RunStartupTasks();

// 6. 运行应用
app.Run("http://*:18889");