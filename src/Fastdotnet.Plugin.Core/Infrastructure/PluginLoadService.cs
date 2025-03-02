using System.Reflection;
using Autofac;
using Autofac.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Mvc.ApplicationParts;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.Extensions.Primitives;
using Microsoft.Extensions.Logging;
using Fastdotnet.Core.Models;
using System.Threading.Tasks;
using Fastdotnet.Core.Plugin;

namespace Fastdotnet.Plugin.Core.Infrastructure
{
    public interface IPluginLoadService
    {
        Task<CommonResult<bool>> LoadPluginAsync(string pluginPath);
        Task<CommonResult<bool>> UnloadPlugin(string pluginName);
        bool IsPluginLoaded(string pluginName);
        IEnumerable<string> GetLoadedPlugins();
        bool pluginStatus(string configPath);
        string GetPluginPath(string pluginId);
    }

    public class PluginLoadService : IPluginLoadService, IDisposable
    {
        private readonly PluginManager _pluginManager;
        private readonly ILifetimeScope _lifetimeScope;
        private readonly Dictionary<string, IPlugin> _activePlugins;
        private readonly Dictionary<string, ILifetimeScope> _pluginScopes;
        private readonly ApplicationPartManager _partManager;
        private readonly IActionDescriptorChangeProvider _actionDescriptorChangeProvider;
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<PluginLoadService> _logger;
        private bool _disposed = false;
        private readonly string _pluginPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "plugins");

        public PluginLoadService(
            PluginManager pluginManager,
            ILifetimeScope lifetimeScope,
            ApplicationPartManager partManager,
            IActionDescriptorChangeProvider actionDescriptorChangeProvider,
            IServiceProvider serviceProvider,
            ILogger<PluginLoadService> logger = null)
        {
            _pluginManager = pluginManager ?? throw new ArgumentNullException(nameof(pluginManager));
            _lifetimeScope = lifetimeScope ?? throw new ArgumentNullException(nameof(lifetimeScope));
            _partManager = partManager ?? throw new ArgumentNullException(nameof(partManager));
            _actionDescriptorChangeProvider = actionDescriptorChangeProvider ?? throw new ArgumentNullException(nameof(actionDescriptorChangeProvider));
            _serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
            _logger = logger;
            _activePlugins = new Dictionary<string, IPlugin>();
            _pluginScopes = new Dictionary<string, ILifetimeScope>();
        }

        public async Task<CommonResult<bool>> LoadPluginAsync(string pluginPath)
        {
            if (string.IsNullOrEmpty(pluginPath))
            {
                return CommonResult<bool>.Error($"插件加载失败: 插件目录不存在");
            }

            var pluginName = Path.GetFileNameWithoutExtension(pluginPath);
            try
            {
                if (_pluginManager.IsPluginLoaded(pluginName))
                {
                    _logger?.LogWarning($"Plugin {pluginName} is already loaded");
                    return CommonResult<bool>.Error($"插件加载失败: {pluginName} 已在运行中");
                }

                await _pluginManager.LoadPluginAsync(pluginPath);

                // 获取插件程序集
                var assembly = _pluginManager.LoadedPlugins[pluginName];

                // 使用Autofac动态注册插件服务
                var serviceTypes = assembly.GetTypes()
                    .Where(t => t.IsClass && !t.IsAbstract && t.IsPublic && t.GetInterfaces().Any())
                    .ToList();

                // 创建一个新的子容器来注册插件服务
                var pluginScope = RegisterPluginServices(pluginName, serviceTypes);

                // 保存插件的生命周期范围引用，用于后续卸载
                _pluginScopes[pluginName] = pluginScope;

                // 将插件服务注册到ASP.NET Core的服务容器中
                RegisterServicesToRootContainer(pluginName, serviceTypes);

                // 添加插件程序集到ApplicationPartManager并刷新路由系统
                UpdateApplicationParts(assembly);

                // 查找并初始化插件实例
                var pluginTypes = assembly.GetTypes()
                    .Where(t => typeof(IPlugin).IsAssignableFrom(t) && !t.IsAbstract);

                foreach (var pluginType in pluginTypes)
                {
                    try
                    {
                        var plugin = (IPlugin)ActivatorUtilities.CreateInstance(_serviceProvider, pluginType);
                        await plugin.InitializeAsync();
                        await plugin.StartAsync();
                        _activePlugins[pluginName] = plugin;
                        _logger?.LogInformation($"Plugin {pluginName} loaded and started successfully");
                    }
                    catch (Exception ex)
                    {
                        _logger?.LogError(ex, $"Failed to initialize or start plugin {pluginName}");
                        // 如果初始化失败，尝试卸载插件
                        await CleanupPluginAsync(pluginName);
                    }
                }
            }
            catch (Exception ex)
            {
                _logger?.LogError(ex, $"Failed to load plugin {pluginName}");
                return CommonResult<bool>.Error($"插件加载失败: {ex.Message}");
            }
            return CommonResult<bool>.Success(true, "插件加载成功");

        }

        private ILifetimeScope RegisterPluginServices(string pluginName, IEnumerable<Type> serviceTypes)
        {
            var pluginScope = _lifetimeScope.BeginLifetimeScope(builder =>
            {
                // 注册所有服务类型
                foreach (var serviceType in serviceTypes)
                {
                    // 直接注册具体类型，这样可以通过类型直接解析
                    builder.RegisterType(serviceType).AsSelf().InstancePerLifetimeScope();

                    // 注册接口映射
                    var interfaceTypes = serviceType.GetInterfaces()
                        .Where(i => i != typeof(IPlugin) && !i.IsGenericType); // 排除IPlugin接口和泛型接口

                    foreach (var interfaceType in interfaceTypes)
                    {
                        // 注册服务到Autofac容器
                        builder.RegisterType(serviceType).As(interfaceType).InstancePerLifetimeScope();
                        _logger?.LogDebug($"Registering {interfaceType.FullName} -> {serviceType.FullName}");
                    }
                }
            });

            return pluginScope;
        }

        public async Task<CommonResult<bool>> UnloadPlugin(string pluginName)
        {
            try
            {
                // 停止并卸载插件实例
                if (_activePlugins.TryGetValue(pluginName, out var plugin))
                {
                    plugin.StopAsync().Wait();
                    plugin.UnloadAsync().Wait();
                    _activePlugins.Remove(pluginName);
                }

                // 从ApplicationPartManager中移除插件的ApplicationPart
                if (_pluginManager.LoadedPlugins.TryGetValue(pluginName, out var assembly))
                {
                    // 查找并移除所有与此程序集相关的ApplicationPart
                    var assemblyParts = _partManager.ApplicationParts
                        .Where(part => part is AssemblyPart ap && ap.Assembly == assembly)
                        .ToList();

                    foreach (var assemblyPart in assemblyParts)
                    {
                        _partManager.ApplicationParts.Remove(assemblyPart);
                    }

                    // 重新构建控制器特性
                    var controllerFeature = new ControllerFeature();
                    _partManager.PopulateFeature(controllerFeature);

                    // 通知ASP.NET Core路由系统已更新
                    if (_actionDescriptorChangeProvider is ActionDescriptorChangeProvider provider)
                    {
                        provider.TokenCanceled();
                    }

                    // 输出已移除的控制器
                    var controllerTypes = assembly.GetExportedTypes()
                        .Where(t => !t.IsAbstract && !t.IsInterface && t.Name.EndsWith("Controller", StringComparison.OrdinalIgnoreCase))
                        .ToList();

                    if (controllerTypes.Any())
                    {
                        Console.WriteLine($"[{pluginName}] Removed controllers:");
                        foreach (var controllerType in controllerTypes)
                        {
                            Console.WriteLine($"  - {controllerType.FullName}");
                        }
                    }
                }

                // 从Autofac容器中移除服务注册
                if (_serviceProvider is AutofacServiceProvider autofacServiceProvider)
                {
                    // 获取根容器
                    var rootLifetimeScope = autofacServiceProvider.LifetimeScope;

                    // 获取插件程序集中的所有服务类型
                    if (_pluginManager.LoadedPlugins.TryGetValue(pluginName, out var pluginAssembly))
                    {
                        var serviceTypes = pluginAssembly.GetTypes()
                            .Where(t => t.IsClass && !t.IsAbstract && t.IsPublic && t.GetInterfaces().Any())
                            .ToList();

                        // 使用Autofac 4.9.4版本的方式移除服务注册
                        // 创建一个新的ContainerBuilder
                        var builder = new ContainerBuilder();

                        // 对于每个服务类型，注册一个空的实现来覆盖原有注册
                        foreach (var serviceType in serviceTypes)
                        {
                            var interfaces = serviceType.GetInterfaces()
                                .Where(i => i != typeof(IPlugin) && !i.IsGenericType)
                                .ToList();

                            // 对每个接口，注册一个空实现或null来覆盖原有注册
                            foreach (var iface in interfaces)
                            {
                                // 注册一个空的委托工厂，返回null，这样可以覆盖原有注册
                                builder.Register<object>(c => null).As(iface).InstancePerLifetimeScope();
                                Console.WriteLine($"  - Unregistering {iface.FullName} -> {serviceType.FullName}");
                            }

                            // 同样覆盖具体类型的注册
                            builder.Register<object>(c => null).As(serviceType).InstancePerLifetimeScope();
                        }

                        // 使用Update方法将新的注册应用到现有容器
                        builder.Update(rootLifetimeScope.ComponentRegistry);

                        // 记录已卸载的服务
                        Console.WriteLine($"[{pluginName}] Unregistered services:");
                        foreach (var serviceType in serviceTypes)
                        {
                            var interfaces = serviceType.GetInterfaces()
                                .Where(i => i != typeof(IPlugin) && !i.IsGenericType)
                                .ToList();

                            foreach (var iface in interfaces)
                            {
                                Console.WriteLine($"  - {iface.FullName} -> {serviceType.FullName}");
                            }
                        }
                    }
                }

                // 释放插件的生命周期范围
                if (_pluginScopes.TryGetValue(pluginName, out var scope))
                {
                    scope.Dispose();
                    _pluginScopes.Remove(pluginName);
                    Console.WriteLine($"[{pluginName}] Plugin scope disposed");
                }

                // 卸载插件
                _pluginManager.UnloadPlugin(pluginName);
                Console.WriteLine($"[{pluginName}] Plugin unloaded from PluginManager");

                // 强制GC回收
                GC.Collect();
                GC.WaitForPendingFinalizers();
                Console.WriteLine($"[{pluginName}] Garbage collection completed");
            }
            catch (Exception ex)
            {
                return CommonResult<bool>.Error($"插件加载失败: {ex.Message}");

                throw;
            }
            return CommonResult<bool>.Success(true, "插件卸载成功");
        }

        public bool IsPluginLoaded(string pluginName)
        {
            return _pluginManager.IsPluginLoaded(pluginName);
        }

        public IEnumerable<string> GetLoadedPlugins()
        {
            return _pluginManager.GetLoadedPlugins();
        }

        private void RegisterServicesToRootContainer(string pluginName, IEnumerable<Type> serviceTypes)
        {
            // 将插件服务注册到ASP.NET Core的服务容器中
            if (_serviceProvider is AutofacServiceProvider autofacServiceProvider)
            {
                // 获取根容器
                var rootLifetimeScope = autofacServiceProvider.LifetimeScope;

                // 创建一个新的ContainerBuilder来注册插件服务
                var builder = new ContainerBuilder();

                // 注册所有服务类型
                foreach (var serviceType in serviceTypes)
                {
                    // 直接注册具体类型
                    builder.RegisterType(serviceType).AsSelf().InstancePerLifetimeScope();

                    // 注册接口映射
                    var interfaceTypes = serviceType.GetInterfaces()
                        .Where(i => i != typeof(IPlugin) && !i.IsGenericType);

                    foreach (var interfaceType in interfaceTypes)
                    {
                        builder.RegisterType(serviceType).As(interfaceType).InstancePerLifetimeScope();
                        _logger?.LogDebug($"Registering {interfaceType.FullName} -> {serviceType.FullName}");
                    }
                }

                // 将服务注册到根容器
                builder.Update(rootLifetimeScope.ComponentRegistry);

                // 输出日志，显示已注册的服务
                _logger?.LogInformation($"[{pluginName}] Registered services:");
                foreach (var serviceType in serviceTypes)
                {
                    var interfaces = serviceType.GetInterfaces()
                        .Where(i => i != typeof(IPlugin) && !i.IsGenericType);
                    foreach (var iface in interfaces)
                    {
                        _logger?.LogDebug($"  - {iface.FullName} -> {serviceType.FullName}");
                    }
                }
            }
        }

        private void UpdateApplicationParts(Assembly assembly)
        {
            // 添加插件程序集到ApplicationPartManager并刷新路由系统
            var assemblyPart = new AssemblyPart(assembly);
            _partManager.ApplicationParts.Add(assemblyPart);
            var controllerFeature = new ControllerFeature();
            _partManager.PopulateFeature(controllerFeature);

            // 通知ASP.NET Core路由系统已更新
            if (_actionDescriptorChangeProvider is ActionDescriptorChangeProvider provider)
            {
                provider.TokenCanceled();
            }

            // 记录已添加的控制器
            var controllerTypes = assembly.GetExportedTypes()
                .Where(t => !t.IsAbstract && !t.IsInterface && t.Name.EndsWith("Controller", StringComparison.OrdinalIgnoreCase))
                .ToList();

            if (controllerTypes.Any())
            {
                _logger?.LogInformation($"Added controllers:");
                foreach (var controllerType in controllerTypes)
                {
                    _logger?.LogDebug($"  - {controllerType.FullName}");
                }
            }
        }

        private async Task CleanupPluginAsync(string pluginName)
        {
            try
            {
                // 尝试停止和卸载插件
                if (_activePlugins.TryGetValue(pluginName, out var plugin))
                {
                    try
                    {
                        await plugin.StopAsync();
                        await plugin.UnloadAsync();
                    }
                    catch (Exception ex)
                    {
                        _logger?.LogError(ex, $"Error during plugin {pluginName} cleanup");
                    }
                    finally
                    {
                        _activePlugins.Remove(pluginName);
                    }
                }

                // 释放插件的生命周期范围
                if (_pluginScopes.TryGetValue(pluginName, out var scope))
                {
                    scope.Dispose();
                    _pluginScopes.Remove(pluginName);
                }

                // 卸载插件
                if (_pluginManager.IsPluginLoaded(pluginName))
                {
                    _pluginManager.UnloadPlugin(pluginName);
                }

                // 强制GC回收
                GC.Collect();
                GC.WaitForPendingFinalizers();
            }
            catch (Exception ex)
            {
                _logger?.LogError(ex, $"Failed to cleanup plugin {pluginName}");
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (_disposed)
                return;

            if (disposing)
            {
                // 停止并卸载所有活动的插件
                foreach (var pluginName in _activePlugins.Keys.ToList())
                {
                    try
                    {
                        UnloadPlugin(pluginName);
                    }
                    catch (Exception ex)
                    {
                        _logger?.LogError(ex, $"Error unloading plugin {pluginName} during disposal");
                    }
                }

                // 清理所有剩余的插件作用域
                foreach (var scope in _pluginScopes.Values)
                {
                    scope.Dispose();
                }
                _pluginScopes.Clear();
                _activePlugins.Clear();
            }

            _disposed = true;
        }

        public string GetPluginPath(string pluginId)
        {
            if (!Directory.Exists(_pluginPath))
            {
                return null;
            }

            foreach (var pluginDir in Directory.GetDirectories(_pluginPath))
            {
                var configPath = Path.Combine(pluginDir, "plugin.json");
                if (File.Exists(configPath))
                {
                    try
                    {
                        var configJson = File.ReadAllText(configPath);
                        var pluginConfig = System.Text.Json.JsonSerializer.Deserialize<PluginConfig>(configJson);
                        if (pluginConfig.id == pluginId)
                        {
                            return pluginDir;
                        }
                    }
                    catch
                    {
                        continue;
                    }
                }
            }

            return null;
        }

        public bool pluginStatus(string configPath)
        {
            try
            {
                // 检查plugin.json是否存在，如果不存在则从DefaultPluginConfig.json复制
                if (!File.Exists(configPath))
                {
                    var defaultConfigPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "DefaultPluginConfig.json");
                    if (File.Exists(defaultConfigPath))
                    {
                        File.Copy(defaultConfigPath, configPath);
                        //Console.WriteLine($"Created plugin.json from DefaultPluginConfig.json: {configPath}");
                    }
                    else
                    {
                        //Console.WriteLine($"Plugin configuration file not found: {configPath}");
                        return false;
                    }
                }

                // 读取并解析plugin.json
                var configJson = File.ReadAllText(configPath);
                var pluginConfig = System.Text.Json.JsonSerializer.Deserialize<PluginConfig>(configJson);

                return pluginConfig.enabled;
            }
            catch (Exception)
            {
                return false;
            }
        }

    }

    public class ActionDescriptorChangeProvider : IActionDescriptorChangeProvider
    {
        private CancellationTokenSource _tokenSource;

        public ActionDescriptorChangeProvider()
        {
            _tokenSource = new CancellationTokenSource();
        }

        public IChangeToken GetChangeToken()
        {
            _tokenSource = new CancellationTokenSource();
            return new CancellationChangeToken(_tokenSource.Token);
        }

        public void TokenCanceled()
        {
            var oldTokenSource = _tokenSource;
            _tokenSource = new CancellationTokenSource();
            oldTokenSource.Cancel();
        }
    }
}
