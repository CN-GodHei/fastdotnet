using Autofac;
using Autofac.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Mvc.ApplicationParts;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using System.Reflection;
using Fastdotnet.Plugin.Core.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

// 设置应用程序URL
builder.WebHost.UseUrls("http://*:18889");

// Add ASP.NET Core services
builder.Services.AddControllers()
    .ConfigureApplicationPartManager(manager =>
    {
        // 确保ApplicationPartManager可以动态加载插件中的控制器
        manager.FeatureProviders.Add(new DynamicControllerFeatureProvider());
    });

// 注册ActionDescriptorChangeProvider服务
builder.Services.AddSingleton<IActionDescriptorChangeProvider, ActionDescriptorChangeProvider>();
builder.Services.AddSingleton<IActionDescriptorChangeProvider>(new ActionDescriptorChangeProvider());

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new() { Title = "Dynamic Plugin System API", Version = "v1" });
});

// 注册IServiceProviderIsService服务
builder.Services.AddSingleton<Microsoft.Extensions.DependencyInjection.IServiceProviderIsService>(new DefaultServiceProviderIsService());

// Use Autofac as the DI container
builder.Host.UseServiceProviderFactory(new AutofacServiceProviderFactory());
builder.Host.ConfigureContainer<ContainerBuilder>(containerBuilder =>
{
    // Register your custom services
    containerBuilder.RegisterType<PluginManager>().AsSelf().SingleInstance();

    // 注册PluginLoadService，并将IServiceProvider作为参数传入
    containerBuilder.RegisterType<PluginLoadService>().As<IPluginLoadService>().SingleInstance()
        .PropertiesAutowired()
        .WithParameter(
            (pi, ctx) => pi.ParameterType == typeof(IServiceProvider),
            (pi, ctx) => ctx.Resolve<IServiceProvider>());
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

//app.UseHttpsRedirection();
app.UseRouting();
app.UseAuthorization();
app.MapControllers();

app.Run();

