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
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

var builder = WebApplication.CreateBuilder(args);

// 1. 注册 ASP.NET Core 的核心服务
builder.Services.AddControllers().AddControllersAsServices()
    .ConfigureApplicationPartManager(manager =>
    {
        // This provider is essential for discovering controllers from dynamically loaded assemblies
        manager.FeatureProviders.Add(new DynamicControllerFeatureProvider());
    });
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.TagActionsBy(apiDesc =>
    {
        if (apiDesc.ActionDescriptor is ControllerActionDescriptor controllerActionDescriptor)
        {
            // 获取插件名称
            var pluginName = apiDesc.ActionDescriptor.Properties.ContainsKey("PluginName")
                ? apiDesc.ActionDescriptor.Properties["PluginName"] as string
                : null;

            var controllerName = controllerActionDescriptor.ControllerName;

            // 如果插件名称存在，则使用插件名称作为主要分组依据
            if (!string.IsNullOrEmpty(pluginName))
            {
                // 插件名作为主分组，控制器名作为子描述
                return new[] { pluginName };
            }
            else
            {
                // 对于非插件的API，按控制器名称分组
                return new[] { controllerName };
            }
        }

        return new[] { "Default" };
    });

});
builder.Services.AddSingleton<IActionDescriptorChangeProvider>(ActionDescriptorChangeProvider.Instance);
builder.Services.AddSingleton<DynamicMiddlewareRegistry>();
//builder.Services.AddSingleton<IActionDescriptorProvider, PluginActionDescriptorProvider>();
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
    containerBuilder.RegisterType<PluginActionDescriptorProvider>().As<IActionDescriptorProvider>().SingleInstance();
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
