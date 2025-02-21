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
var pluginManager = new MefPluginManager(builder.Services, pluginPath);
builder.Services.AddSingleton(pluginManager);

// 创建应用程序实例
var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// 使用JWT中间件
app.UseMiddleware<JwtMiddleware>();

// 启用路由
app.UseRouting();

// 启用授权（确保在UseRouting和UseEndpoints之间）
app.UseAuthorization();

// 配置端点和加载插件
app.MapControllers();

// 加载插件并注册插件路由
app.Logger.LogInformation("开始注册插件路由...");
var endpointRouteBuilder = app;
pluginManager.SetEndpointRouteBuilder(endpointRouteBuilder);
pluginManager.LoadAllPlugins();

app.Run();
app.UseRouting();

// 配置路由端点
app.UseEndpoints(endpoints =>
{
    // 先注册插件路由
    app.Logger.LogInformation("开始注册插件路由...");
    pluginManager.SetEndpointRouteBuilder(endpoints);
    pluginManager.LoadAllPlugins();

    // 再注册主应用路由
    app.Logger.LogInformation("开始注册主应用路由...");
    endpoints.MapControllers();
});

app.Run();
