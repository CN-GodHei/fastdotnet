using Fastdotnet.Service.IService.Sys;

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
         .AllowAnyOrigin()
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
builder.Services.AddSqlSugar(builder.Configuration);

// 注册 HttpContextAccessor 和 CurrentUser 服务
builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<ICurrentUser, CurrentUser>();
builder.Services.Configure<JwtSettings>(builder.Configuration.GetSection("JwtSettings"));

// 添加认证和授权
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    // 此处需要手动读取配置，因为此时DI容器还未完全构建
    var jwtSettings = builder.Configuration.GetSection("JwtSettings").Get<JwtSettings>() ?? throw new InvalidOperationException("JwtSettings not configured.");
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = jwtSettings.Issuer,
        ValidAudience = jwtSettings.Audience,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings.SecretKey))
    };
});

// 注册日志服务
builder.Services.AddScoped<ILogService, LogService>();
//builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

// 注册应用服务和初始化器
builder.Services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
builder.Services.AddScoped(typeof(IRepository<,>), typeof(Repository<,>));
builder.Services.AddScoped(typeof(IBaseService<,>), typeof(BaseService<,>));
builder.Services.AddScoped<IAdminUserService, AdminUserService>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IApplicationInitializer, OrmCodeFirstInitializer>();
builder.Services.AddScoped<IApplicationInitializer, BlacklistInitializer>();
builder.Services.AddScoped<IApplicationInitializer, RateLimitRuleInitializer>();
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

// 添加SignalR服务
builder.Services.AddSignalR();

// 注册IStorageContext服务
builder.Services.AddScoped<IStorageContext, StorageContext>();

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
if (!app.Environment.IsDevelopment())
{
    app.UseHttpsRedirection();
}
app.UseHttpsRedirection();
app.UseMiddleware<RequestIdMiddleware>();
app.UseMiddleware<GlobalExceptionMiddleware>();
app.UseMiddleware<OperationLogMiddleware>();
app.UseMiddleware<PluginStaticFileMiddleware>();

// 注册限流和黑名单中间件
app.UseMiddleware<BlacklistMiddleware>();
app.UseMiddleware<RateLimitMiddleware>();

app.UseMiddleware<DynamicMiddlewareDispatcher>();

app.UseRouting(); // 添加路由中间件
app.UseAuthentication();
app.UseAuthorization();
// 👇 启用优雅停机
app.UseGracefulShutdown();
app.UseEndpoints(endpoints =>
{
    endpoints.MapControllers();
    // 注册主框架的SignalR端点
    endpoints.MapHub<UniversalHub>("/api/signalr");
});

// 5. 应用程序启动后执行启动任务
app.RunStartupTasks();

// 6. 运行应用
app.Run("http://*:18889");