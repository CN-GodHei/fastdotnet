using Autofac;
using Autofac.Core;
using Autofac.Extensions.DependencyInjection;
// 添加AutoMapper相关引用
using AutoMapper;
using Fastdotnet.Core.Initializers;
using Fastdotnet.Core.IService;
using Fastdotnet.Core.Middleware;
using Fastdotnet.Core.Service;
using Fastdotnet.Core.Settings;
using Fastdotnet.Core.Utils;
using Fastdotnet.Orm;
using Fastdotnet.Plugin.Contracts;
using Fastdotnet.Plugin.Core.Infrastructure;
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
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using Scrutor;
using Swashbuckle.AspNetCore.SwaggerGen;
using System.Reflection;
using System.Text;
using Yitter.IdGenerator;

var builder = WebApplication.CreateBuilder(args);
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

    options.Filters.Add<GlobalExceptionFilter>();
    options.Filters.Add<GlobalResultFilter>(); // 添加全局结果过滤器
})
    .AddControllersAsServices()
    .AddNewtonsoftJson(options =>
    {
        SetNewtonsoftJsonSetting(options.SerializerSettings);
    });

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    // 添加主API文档
    c.SwaggerDoc("main", new OpenApiInfo
    {
        Title = "主系统 API",
        Version = "v1",
        Description = "Fastdotnet 主系统 API 文档"
    });

    // 为插件动态添加API文档定义
    var pluginDirs = Directory.GetDirectories(Path.Combine(AppContext.BaseDirectory, "Plugins"));
    foreach (var pluginDir in pluginDirs)
    {
        try
        {
            var pluginJsonPath = Path.Combine(pluginDir, "plugin.json");
            if (File.Exists(pluginJsonPath))
            {
                var pluginJson = File.ReadAllText(pluginJsonPath);
                var pluginConfig = System.Text.Json.JsonDocument.Parse(pluginJson);
                var pluginId = pluginConfig.RootElement.GetProperty("id").GetString();
                var pluginName = pluginConfig.RootElement.GetProperty("name").GetString();
                var pluginDescription = pluginConfig.RootElement.GetProperty("description").GetString();

                if (!string.IsNullOrEmpty(pluginId))
                {
                    c.SwaggerDoc($"plugin-{pluginId.ToLower()}", new OpenApiInfo
                    {
                        Title = $"{pluginName} 插件 API",
                        Version = "v1",
                        Description = $"Fastdotnet {pluginDescription}"
                    });
                }
            }
        }
        catch (Exception ex)
        {
            // 如果读取plugin.json失败，跳过该插件
            Console.WriteLine($"读取插件配置失败 {pluginDir}: {ex.Message}");
        }
    }

    c.TagActionsBy(apiDesc =>
    {
        if (apiDesc.ActionDescriptor is ControllerActionDescriptor controllerActionDescriptor)
        {
            var pluginName = apiDesc.ActionDescriptor.Properties.ContainsKey("PluginName")
                ? apiDesc.ActionDescriptor.Properties["PluginName"] as string
                : null;

            var controllerName = controllerActionDescriptor.ControllerName;

            if (!string.IsNullOrEmpty(pluginName))
            {
                return new[] { pluginName };
            }
            else
            {
                // 为AuthController设置特殊标签名，使其排在最前面
                if (controllerName == "Auth")
                {
                    return new[] { "00-认证接口" };
                }
                return new[] { controllerName };
            }
        }
        return new[] { "Default" };
    });

    // 添加文档过滤器
    c.DocumentFilter<PluginDocumentFilter>();
    c.DocumentFilter<TagOrderDocumentFilter>();

    // 启用 XML 文档注释
    var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    if (File.Exists(xmlPath))
    {
        c.IncludeXmlComments(xmlPath);
    }

    // 为插件中的控制器添加XML注释支持
    foreach (var pluginDir in pluginDirs)
    {
        try
        {
            var pluginJsonPath = Path.Combine(pluginDir, "plugin.json");
            if (File.Exists(pluginJsonPath))
            {
                var pluginJson = File.ReadAllText(pluginJsonPath);
                var pluginConfig = System.Text.Json.JsonDocument.Parse(pluginJson);
                var pluginId = pluginConfig.RootElement.GetProperty("id").GetString();

                if (!string.IsNullOrEmpty(pluginId))
                {
                    var pluginXmlPath = Path.Combine(pluginDir, $"{pluginId}.xml");
                    if (File.Exists(pluginXmlPath))
                    {
                        c.IncludeXmlComments(pluginXmlPath);
                    }
                }
            }
        }
        catch (Exception ex)
        {
            // 如果读取plugin.json失败，跳过该插件
            Console.WriteLine($"读取插件配置失败 {pluginDir}: {ex.Message}");
        }
    }

    // Add JWT Bearer security definition
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "JWT Authorization header using the Bearer scheme. Example: \"Authorization: Bearer {token}\"",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.Http, // Change type to Http
        Scheme = "bearer", // Must be lowercase
        BearerFormat = "JWT"
    });

    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            new string[] {}
        }
    });
});
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
builder.Services.AddScoped<IAdminUserService, AdminUserService>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IApplicationInitializer, OrmCodeFirstInitializer>();
// 扫描并注册所有 IApplicationInitializer 实现
builder.Services.Scan(scan => scan
    .FromAssemblies(AppDomain.CurrentDomain.GetAssemblies())
    .AddClasses(classes => classes.AssignableTo<IApplicationInitializer>())
    .AsImplementedInterfaces()
    .WithScopedLifetime());
builder.Services.AddScoped<IPermissionProvider, FrameworkPermissionProvider>();
builder.Services.AddScoped<GlobalExceptionFilter>();

// 添加内存缓存服务
builder.Services.AddMemoryCache();

// 注册邮件和验证码服务
builder.Services.AddScoped<IEmailService, EmailService>();
builder.Services.AddScoped<IVerificationCodeManager, VerificationCodeManager>();

// 扫描并注册所有验证码策略
builder.Services.Scan(scan => scan
    .FromAssemblies(AppDomain.CurrentDomain.GetAssemblies())
    .AddClasses(classes => classes.AssignableTo<IVerificationCodeStrategy>())
    .AsImplementedInterfaces()
    .WithScopedLifetime());

// 注册新权限同步服务
builder.Services.AddScoped<IPermissionSyncService, PermissionSyncService>();

// 注册授权处理器和策略提供者
builder.Services.AddSingleton<IAuthorizationHandler, PermissionHandler>();
builder.Services.AddSingleton<IAuthorizationPolicyProvider, PermissionPolicyProvider>();
builder.Services.AddScoped<IPermissionService, PermissionService>();
builder.Services.AddScoped<IMenuService, MenuService>();

// 扫描并注册所有 IStartupTask 实现
builder.Services.Scan(scan => scan
    .FromAssemblies(AppDomain.CurrentDomain.GetAssemblies())
    .AddClasses(classes => classes.AssignableTo<IStartupTask>())
    .AsImplementedInterfaces()
    .WithScopedLifetime());

builder.Services.AddSingleton<IControllerActivator, PluginControllerActivator>();

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
        c.Resolve<PluginStaticFileProviderRegistry>()
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
});

// 3. 构建并运行应用
var app = builder.Build();

// 执行应用初始化器
await app.UseApplicationInitializers();

// 4. 配置 HTTP 请求管道
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        // 添加主API文档
        c.SwaggerEndpoint("/swagger/main/swagger.json", "主系统 API v1");

        // 为插件动态添加API文档
        var pluginDirs = Directory.GetDirectories(Path.Combine(AppContext.BaseDirectory, "Plugins"));
        foreach (var pluginDir in pluginDirs)
        {
            try
            {
                var pluginJsonPath = Path.Combine(pluginDir, "plugin.json");
                if (File.Exists(pluginJsonPath))
                {
                    var pluginJson = File.ReadAllText(pluginJsonPath);
                    var pluginConfig = System.Text.Json.JsonDocument.Parse(pluginJson);
                    var pluginId = pluginConfig.RootElement.GetProperty("id").GetString();
                    var pluginName = pluginConfig.RootElement.GetProperty("name").GetString();

                    if (!string.IsNullOrEmpty(pluginId))
                    {
                        c.SwaggerEndpoint($"/swagger/plugin-{pluginId.ToLower()}/swagger.json", $"{pluginName} 插件 API v1");
                    }
                }
            }
            catch (Exception ex)
            {
                // 如果读取plugin.json失败，跳过该插件
                Console.WriteLine($"读取插件配置失败 {pluginDir}: {ex.Message}");
            }
        }

        c.RoutePrefix = "swagger";
        c.DefaultModelsExpandDepth(-1); // 隐藏底部的Models部分
    });
    app.UseCors();
}
// 只在生产环境中使用HTTPS重定向
if (!app.Environment.IsDevelopment())
{
    app.UseHttpsRedirection();
}
app.UseHttpsRedirection();
app.UseMiddleware<RequestIdMiddleware>();
app.UseMiddleware<OperationLogMiddleware>();
app.UseMiddleware<PluginStaticFileMiddleware>();
app.UseMiddleware<DynamicMiddlewareDispatcher>();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

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
                foreach (var task in startupTasks)
                {
                    try
                    {
                        await task.ExecuteAsync();
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Error executing startup task {task.GetType().Name}: {ex.Message}");
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