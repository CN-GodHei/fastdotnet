
using Autofac;
using Autofac.Core;
using Autofac.Extensions.DependencyInjection;
using Fastdotnet.Plugin.Core.Infrastructure;
using Microsoft.AspNetCore.Mvc.ApplicationParts;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using System.Reflection;

var builder = WebApplication.CreateBuilder(args);

// 1. 配置 Autofac 作为服务提供程序工厂
builder.Host.UseServiceProviderFactory(new AutofacServiceProviderFactory());

// 2. 向容器中添加服务
builder.Services.AddControllers().AddControllersAsServices()
    .ConfigureApplicationPartManager(manager =>
    {
        // 这使得 MVC 能够从动态加载的插件程序集中发现控制器
        manager.FeatureProviders.Add(new DynamicControllerFeatureProvider());
    });

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new() { Title = "Dynamic Plugin System API", Version = "v1" });
    var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    if (File.Exists(xmlPath))
    {
        c.IncludeXmlComments(xmlPath);
    }
});

// 【优化】移除重复注册，只保留一个必要的单例注册。
builder.Services.AddSingleton<IActionDescriptorChangeProvider>(ActionDescriptorChangeProvider.Instance);


// 【最终修复】手动创建核心服务实例，以确保实例统一和注册方式正确
var partManager = builder.Services.BuildServiceProvider().GetRequiredService<ApplicationPartManager>();
var pluginManager = new PluginManager(partManager);
var pluginSource = new PluginRegistrationSource(pluginManager);

// 3. 配置 Autofac 容器
builder.Host.ConfigureContainer<ContainerBuilder>(containerBuilder =>
{
    // 注册我们手动创建的 PluginManager 实例为单例
    containerBuilder.RegisterInstance(pluginManager).AsSelf().SingleInstance();

    // 注册其他服务
    containerBuilder.RegisterType<PluginLoadService>().As<IPluginLoadService>().SingleInstance();

    // 使用正确的方法将 pluginSource 注册为动态注册源
    containerBuilder.RegisterSource(pluginSource);
});


var app = builder.Build();

// 【修复】在应用程序启动时加载插件
using (var scope = app.Services.CreateScope())
{
    var pluginLoadService = scope.ServiceProvider.GetRequiredService<IPluginLoadService>();
    var pluginDir = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "plugins");
    if (Directory.Exists(pluginDir))
    {
        var pluginAssemblies = Directory.GetFiles(pluginDir, "*.dll");
        foreach (var pluginAssembly in pluginAssemblies)
        {
            pluginLoadService.LoadPluginAsync(pluginAssembly).GetAwaiter().GetResult();
        }
    }
}


// 配置 HTTP 请求管道
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run("http://*:18889");
