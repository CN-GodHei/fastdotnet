using Autofac;
using Autofac.Extensions.DependencyInjection;
using Fastdotnet.Core.Initializers;
using Fastdotnet.Core.Middleware;
using Fastdotnet.Plugin.Core.Infrastructure;
using Fastdotnet.Service;
using Fastdotnet.Service.Initializers;
using Fastdotnet.Service.IService.Admin;
using Fastdotnet.WebApi.Extensions;
using Fastdotnet.WebApi.Middleware;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.ApplicationParts;
using Microsoft.AspNetCore.Mvc.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

// 1. 注册 ASP.NET Core 的核心服务
builder.Services.AddControllers().AddControllersAsServices()
    .ConfigureApplicationPartManager(manager =>
    {
        // This provider is essential for discovering controllers from dynamically loaded assemblies
        manager.FeatureProviders.Add(new DynamicControllerFeatureProvider());
    });
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddSingleton<IActionDescriptorChangeProvider>(ActionDescriptorChangeProvider.Instance);
builder.Services.AddSingleton<DynamicMiddlewareRegistry>();
builder.Services.AddSingleton<IActionDescriptorProvider, PluginActionDescriptorProvider>();
// 注册应用服务和初始化器
builder.Services.AddScoped<IAdminUserService, AdminUserService>();
builder.Services.AddScoped<IApplicationInitializer, AdminUserInitializer>();


// 2. 准备插件系统的核心实例
var partManager = builder.Services.BuildServiceProvider().GetRequiredService<ApplicationPartManager>();
var pluginManager = new PluginManager(partManager);
var pluginSource = new PluginRegistrationSource(pluginManager);

// 3. 配置 Autofac 容器
builder.Host.UseServiceProviderFactory(new AutofacServiceProviderFactory());
builder.Host.ConfigureContainer<ContainerBuilder>(containerBuilder =>
{
    // 注册宿主的核心服务
    containerBuilder.RegisterInstance(pluginManager).AsSelf().SingleInstance();
    containerBuilder.RegisterType<PluginLoadService>().As<IPluginLoadService>().SingleInstance();
    containerBuilder.RegisterSource(pluginSource);
});

// 4. 构建并运行应用
var app = builder.Build();

// 执行应用初始化器
await app.UseApplicationInitializers();

// 5. 配置 HTTP 请求管道
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

// 注册动态中间件调度器，它将执行来自所有插件的中间件
app.UseMiddleware<DynamicMiddlewareDispatcher>();

//app.UseMiddleware<PluginRoutingMiddleware>();
app.UseAuthorization();
app.MapControllers();

// 6. 运行应用
app.Run("http://*:18889");
