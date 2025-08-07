using Autofac;
using Autofac.Extensions.DependencyInjection;
using Fastdotnet.Core.Plugin;
using Fastdotnet.Plugin.Core.Infrastructure;
using Microsoft.AspNetCore.Mvc.ApplicationParts;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using System.Reflection;
using System.Text.Json;

var builder = WebApplication.CreateBuilder(args);

// 1. 配置 Autofac 作为服务提供程序工厂
builder.Host.UseServiceProviderFactory(new AutofacServiceProviderFactory());

// 2. 注册ASP.NET Core的核心服务，这是获取ApplicationPartManager的前提
builder.Services.AddControllers().AddControllersAsServices()
    .ConfigureApplicationPartManager(manager =>
    {
        manager.FeatureProviders.Add(new DynamicControllerFeatureProvider());
    });
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddSingleton<IActionDescriptorChangeProvider>(ActionDescriptorChangeProvider.Instance);

// 3. 预备核心服务和插件系统
var partManager = builder.Services.BuildServiceProvider().GetRequiredService<ApplicationPartManager>();
var pluginManager = new PluginManager(partManager);
var pluginSource = new PluginRegistrationSource(pluginManager);

// 4. 配置Autofac容器
builder.Host.ConfigureContainer<ContainerBuilder>(containerBuilder =>
{
    // 注册核心服务实例
    containerBuilder.RegisterInstance(pluginManager).AsSelf().SingleInstance();
    containerBuilder.RegisterType<PluginLoadService>().As<IPluginLoadService>().SingleInstance();
    containerBuilder.RegisterSource(pluginSource);

    // 发现并注册所有插件的服务
    ConfigurePlugins(containerBuilder, pluginManager);
});


// 5. 构建并运行应用
var app = builder.Build();

// 启动所有已加载的插件
await StartPlugins(app.Services);

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run("http://*:18889");


// --- 辅助方法 ---

static void ConfigurePlugins(ContainerBuilder containerBuilder, PluginManager pluginManager)
{
    var pluginPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "plugins");
    if (!Directory.Exists(pluginPath)) return;

    // 1. 读取所有插件配置
    var allPluginConfigs = new Dictionary<string, (PluginConfig Config, string DllPath)>();
    foreach (var pluginDir in Directory.GetDirectories(pluginPath))
    {
        var configPath = Path.Combine(pluginDir, "plugin.json");
        if (!File.Exists(configPath)) continue;

        var configJson = File.ReadAllText(configPath);
        var config = JsonSerializer.Deserialize<PluginConfig>(configJson, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
        
        if (config == null || !config.enabled)
        {
            continue;
        }

        var entryPoint = string.IsNullOrEmpty(config.entryPoint) ? config.id + ".dll" : config.entryPoint;
        var dllPath = Path.Combine(pluginDir, entryPoint);

        if (File.Exists(dllPath))
        {
            allPluginConfigs[config.id] = (config, dllPath);
        }
    }

    // 2. 拓扑排序
    var sortedPluginIds = TopologicalSort(allPluginConfigs.Values.Select(p => p.Config).ToList());

    // 3. 按顺序加载程序集并配置服务
    foreach (var pluginId in sortedPluginIds)
    {
        var (config, dllPath) = allPluginConfigs[pluginId];
        var assembly = pluginManager.LoadPlugin(config.id, dllPath);
        if (assembly == null) continue;

        var pluginType = assembly.GetTypes().FirstOrDefault(t => typeof(IPlugin).IsAssignableFrom(t) && !t.IsAbstract);
        if (pluginType != null)
        {
            var pluginInstance = (IPlugin)Activator.CreateInstance(pluginType);
            pluginInstance?.ConfigureServices(containerBuilder);
        }
    }
}

static async Task StartPlugins(IServiceProvider serviceProvider)
{
    using var scope = serviceProvider.CreateScope();
    var pluginManager = scope.ServiceProvider.GetRequiredService<PluginManager>();
    var loadedPlugins = pluginManager.GetLoadedPlugins(); // 需要在PluginManager中添加此方法

    foreach (var pluginId in loadedPlugins)
    {
        var assembly = pluginManager.GetPluginAssembly(pluginId); // 需要在PluginManager中添加此方法
        if (assembly == null) continue;

        var pluginType = assembly.GetTypes().FirstOrDefault(t => typeof(IPlugin).IsAssignableFrom(t) && !t.IsAbstract);
        if (pluginType != null)
        {
            var pluginInstance = (IPlugin)ActivatorUtilities.CreateInstance(scope.ServiceProvider, pluginType);
            await pluginInstance.InitializeAsync();
            await pluginInstance.StartAsync();
        }
    }
}

static List<string> TopologicalSort(List<PluginConfig> plugins)
{
    var sorted = new List<string>();
    var visited = new HashSet<string>();
    var graph = plugins.ToDictionary(p => p.id, p => p.dependencies ?? new List<string>());

    foreach (var plugin in plugins)
    {
        if (!visited.Contains(plugin.id))
        {
            Visit(plugin.id, graph, visited, sorted, new HashSet<string>());
        }
    }

    return sorted;
}

static void Visit(string nodeId, Dictionary<string, List<string>> graph, HashSet<string> visited, List<string> sorted, HashSet<string> recursionStack)
{
    visited.Add(nodeId);
    recursionStack.Add(nodeId);

    if (graph.TryGetValue(nodeId, out var dependencies))
    {
        foreach (var dependency in dependencies)
        {
            if (!graph.ContainsKey(dependency))
                throw new InvalidOperationException($"插件 '{nodeId}' 的依赖 '{dependency}' 未找到。");

            if (recursionStack.Contains(dependency))
                throw new InvalidOperationException($"检测到循环依赖: {nodeId} -> {dependency}");

            if (!visited.Contains(dependency))
            {
                Visit(dependency, graph, visited, sorted, recursionStack);
            }
        }
    }

    recursionStack.Remove(nodeId);
    sorted.Add(nodeId);
}