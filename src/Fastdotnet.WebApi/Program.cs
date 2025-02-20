using Fastdotnet.WebApi.Middleware.Authentication;
using Fastdotnet.Service.IService.Admin;
using Fastdotnet.Service;
using Fastdotnet.Infrastructure;
using Fastdotnet.Core;
using Fastdotnet.Core.Plugin;
using Microsoft.AspNetCore.Routing;

var builder = WebApplication.CreateBuilder(args);

// 设置应用程序URL
builder.WebHost.UseUrls("http://*:18848");

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// 注册服务
builder.Services.AddScoped<IAdminUserService, AdminUserService>();

// 注册基础设施服务
builder.Services.AddInfrastructureServices(builder.Configuration);

// 注册业务服务
builder.Services.AddApplicationServices();

// 配置插件管理器
var pluginPath = Path.Combine(AppContext.BaseDirectory, "plugins");
if (!Directory.Exists(pluginPath))
{
    Directory.CreateDirectory(pluginPath);
}
var pluginManager = new PluginManager(builder.Services, pluginPath);
builder.Services.AddSingleton(pluginManager);

// 注册IEndpointRouteBuilder服务
builder.Services.AddSingleton<IEndpointRouteBuilder>(provider => provider.GetRequiredService<WebApplication>());

// 加载所有现有插件
pluginManager.LoadAllPlugins();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// 使用JWT中间件
app.UseMiddleware<JwtMiddleware>();

// 启用路由和控制器
app.Logger.LogInformation("开始注册路由...");
app.MapControllers();

// 设置插件管理器的EndpointRouteBuilder
pluginManager.SetEndpointRouteBuilder(app);

app.Run();
