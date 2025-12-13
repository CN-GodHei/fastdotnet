using Autofac;
using Autofac.Core;
using Autofac.Extensions.DependencyInjection;
// 添加AutoMapper相关引用
using AutoMapper;
using Fastdotnet.Core.Hubs;
using Fastdotnet.Core.Initializers;
using Fastdotnet.Core.IService;
using Fastdotnet.Core.Middleware;
using Fastdotnet.Core.Models.LogModels;
using Fastdotnet.Core.Services.System;
using Fastdotnet.Core.Settings;
using Fastdotnet.Core.Utils;
using Fastdotnet.Core.Utils.Extensions;
using Fastdotnet.Orm;
using Fastdotnet.Plugin.Contracts;
using Fastdotnet.Plugin.Core.Infrastructure;
using Fastdotnet.Plugin.Shared.AdapterAOT;
using Fastdotnet.Service;
using Fastdotnet.Service.Initializers;
using Fastdotnet.Service.IService;
using Fastdotnet.Service.IService.Admin;
using Fastdotnet.Service.Service;
using Fastdotnet.Service.Service.Admin;
using Fastdotnet.Service.Service.System;
using Fastdotnet.WebApi.Controllers;
using Fastdotnet.WebApi.Extensions;
using Fastdotnet.WebApi.Filters;
using Fastdotnet.WebApi.Middleware;
using Fastdotnet.WebApi.Middleware.Authorization;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.ApplicationParts;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using Scrutor;
using Swashbuckle.AspNetCore.SwaggerGen;
using System.Reflection;
using System.Runtime.Loader;
using System.Text;
using System.Threading.Tasks;
using Yitter.IdGenerator;

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


// 1. 注册 ASP.NET Core 的核心服务
builder.Services.AddControllers(options =>
{
    var policy = new AuthorizationPolicyBuilder()
        .RequireAuthenticatedUser()
        .Build();
    options.Filters.Add(new AuthorizeFilter(policy));

    //options.Filters.Add<GlobalExceptionFilter>();
    options.Filters.Add<GlobalResultFilter>(); // 添加全局结果过滤器
})
    .AddControllersAsServices()
    .AddNewtonsoftJson(options =>
    {
        SetNewtonsoftJsonSetting(options.SerializerSettings);
    });
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

// 2. 配置 Autofac 容器
builder.Host.UseServiceProviderFactory(new AutofacServiceProviderFactory());
builder.Host.ConfigureContainer<ContainerBuilder>(containerBuilder =>
{
    containerBuilder.RegisterType<PluginManager>().AsSelf().SingleInstance();
    containerBuilder.RegisterType<PluginStaticFileProviderRegistry>().AsSelf().SingleInstance();

    containerBuilder.Register(c => new PluginLoadService(
        c.Resolve<PluginManager>(),
        c.Resolve<ILifetimeScope>(),
        c.Resolve<ILogger<PluginLoadService>>(),
        c.Resolve<ILoggerFactory>(),
        c.Resolve<PluginStaticFileProviderRegistry>(),
        c.Resolve<IConfiguration>()
    )).As<IPluginLoadService>().SingleInstance();

    containerBuilder.RegisterType<PluginActionDescriptorProvider>().As<IActionDescriptorProvider>().SingleInstance();

    // 在Autofac中注册AutoMapper
    containerBuilder.Register(c =>
    {
        var context = c.Resolve<IComponentContext>();
        var loggerFactory = context.Resolve<ILoggerFactory>(); // 1. 解析 ILoggerFactory

        // 2. 创建和配置 MapperConfigurationExpression
        var expression = new AutoMapper.MapperConfigurationExpression();
        expression.AddMaps(AppDomain.CurrentDomain.GetAssemblies());
        expression.ConstructServicesUsing(context.Resolve);

        // 3. 使用你提供的特定构造函数创建 MapperConfiguration
        var config = new MapperConfiguration(expression, loggerFactory);

        return config.CreateMapper();
    }).As<IMapper>().InstancePerLifetimeScope();

    // 如果需要在Autofac中进行更精细的缓存服务控制，可以在这里添加
    //containerBuilder.RegisterType<HybridCacheService>().As<IHybridCacheService>().InstancePerLifetimeScope();
    //containerBuilder.RegisterType<HybridCacheService>().As<IHybridCacheService>().OwnedByLifetimeScope();
});

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
var lifetime = app.Services.GetRequiredService<IHostApplicationLifetime>();
lifetime.ApplicationStarted.Register(() =>
{
    _ = Task.Run(async () =>
    {
        Console.WriteLine("Application has started. Executing startup tasks in background...");
        try
        {
            using (var scope = app.Services.CreateScope())
            {
                var startupTasks = scope.ServiceProvider.GetServices<IStartupTask>();
                var _logService = scope.ServiceProvider.GetRequiredService<ILogService>(); // ✅ 提前获取日志服务

                foreach (var task in startupTasks)
                {
                    var taskId = $"StartupTask-{Guid.NewGuid():N}";
                    var taskType = task.GetType().Name;
                    try
                    {
                        await task.ExecuteAsync();
                    }
                    catch (Exception ex)
                    {
                        // ❗ 统一记录异常到日志系统
                        var exceptionLog = new ExceptionLog
                        {
                            RequestId = taskId,
                            ExceptionType = ex.GetType().FullName,
                            Message = ex.Message,
                            StackTrace = ex.StackTrace ?? string.Empty,
                            Path = "/startup-task",
                            Method = taskType,
                            CreatedAt = DateTime.Now
                        };

                        try
                        {
                            await _logService.AddExceptionLogAsync(exceptionLog);
                        }
                        catch (Exception logEx)
                        {
                            // 如果写日志也失败，至少输出到控制台
                            Console.WriteLine($"Failed to log exception for {taskType}: {logEx.Message}");
                        }
                    }
                }
            }
            Console.WriteLine("All startup tasks executed.");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error during startup tasks execution: {ex.Message}");
        }

        // 初始化DebugLogger
        var logService = app.Services.GetRequiredService<ILogService>();
        DebugLogger.Initialize(logService);
    });
});

// 6. 运行应用
app.Run("http://*:18889");

static void SetNewtonsoftJsonSetting(JsonSerializerSettings setting)
{
    setting.DateFormatHandling = DateFormatHandling.IsoDateFormat;
    setting.DateTimeZoneHandling = DateTimeZoneHandling.Local;
    setting.DateFormatString = "yyyy-MM-dd HH:mm:ss";
    setting.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
    //不改变字段大小写：还是注释吧，总有人分不清大小写，但程序不会，通吃就行
    setting.ContractResolver = new DefaultContractResolver();

}