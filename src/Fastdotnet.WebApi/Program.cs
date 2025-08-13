using Autofac;
using Autofac.Core;
using Autofac.Extensions.DependencyInjection;
// 添加AutoMapper相关引用
using AutoMapper;
using Fastdotnet.Core.Initializers;
using Fastdotnet.Core.IService;
using Fastdotnet.Core.Middleware;
using Fastdotnet.Core.Utils;
using Fastdotnet.Orm;
using Fastdotnet.Plugin.Core.Infrastructure;
using Fastdotnet.Service;
using Fastdotnet.Service.Initializers;
using Fastdotnet.Service.IService.Admin;
using Fastdotnet.Service.Service;
using Fastdotnet.WebApi.Controllers;
using Fastdotnet.WebApi.Extensions;
using Fastdotnet.WebApi.Filters;
using Fastdotnet.WebApi.Middleware;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.ApplicationParts;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using Scrutor;
using Swashbuckle.AspNetCore.SwaggerGen;

var builder = WebApplication.CreateBuilder(args);

// 1. 注册 ASP.NET Core 的核心服务
builder.Services.AddControllers(options =>
{
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
                return new[] { controllerName };
            }
        }
        return new[] { "Default" };
    });
});
builder.Services.AddSingleton<IActionDescriptorChangeProvider>(ActionDescriptorChangeProvider.Instance);
builder.Services.AddSingleton<DynamicMiddlewareRegistry>();
builder.Services.AddSqlSugar(builder.Configuration);


// 注册日志服务
builder.Services.AddScoped<ILogService, LogService>();
//builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

// 注册应用服务和初始化器
builder.Services.AddScoped<IAdminUserService, AdminUserService>();
builder.Services.AddScoped<IApplicationInitializer, AdminUserInitializer>();
builder.Services.AddScoped<IApplicationInitializer, OrmCodeFirstInitializer>();
builder.Services.AddScoped<GlobalExceptionFilter>();

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
    containerBuilder.Register(c => new PluginLoadService(
        c.Resolve<PluginManager>(),
        c.Resolve<ILifetimeScope>(),
        c.Resolve<ILogger<PluginLoadService>>(),
        c.Resolve<ILoggerFactory>()
    )).As<IPluginLoadService>().SingleInstance();
    containerBuilder.RegisterType<PluginActionDescriptorProvider>().As<IActionDescriptorProvider>().SingleInstance();
    //containerBuilder.RegisterType<DynamicControllerFeatureProvider>().As<IApplicationFeatureProvider<ControllerFeature>>().SingleInstance();
    
    // 在Autofac中注册AutoMapper，确保插件中的Profile也能被扫描到
    //containerBuilder.Register(context => {
    //    var config = new MapperConfiguration(cfg =>
    //    {
    //        // 扫描所有程序集中的Profile
    //        cfg.AddMaps(AppDomain.CurrentDomain.GetAssemblies());
    //    }, context.Resolve<ILoggerFactory>());
    //    return config;
    //}).AsSelf().SingleInstance();
    //containerBuilder.Register(c => c.Resolve<MapperConfiguration>().CreateMapper()).As<IMapper>().InstancePerLifetimeScope();
});

// 3. 构建并运行应用
var app = builder.Build();

// 执行应用初始化器
await app.UseApplicationInitializers();

// 4. 配置 HTTP 请求管道
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseMiddleware<RequestIdMiddleware>();
app.UseMiddleware<OperationLogMiddleware>();
app.UseMiddleware<DynamicMiddlewareDispatcher>();
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
    //setting.ContractResolver = new DefaultContractResolver();
}