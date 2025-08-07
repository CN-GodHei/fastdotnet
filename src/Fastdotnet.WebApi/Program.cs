using Autofac;
using Autofac.Extensions.DependencyInjection;
using Fastdotnet.Plugin.Core.Infrastructure;
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

// 5. 配置 HTTP 请求管道
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
//app.UseMiddleware<PluginRoutingMiddleware>();
app.UseAuthorization();
app.MapControllers();

// 6. 运行应用
app.Run("http://*:18889");