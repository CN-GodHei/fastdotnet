using System.Collections.Concurrent;
using System.Reflection;
using System.Runtime.Loader;
using System.Xml;
using Fastdotnet.Core.Models;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json.Linq;

namespace Fastdotnet.Core.Plugin
{
    public class PluginManager
    {
        private readonly IServiceCollection _services;
        private readonly string _pluginPath;
        private IEndpointRouteBuilder _endpointRouteBuilder;
        private readonly ConcurrentDictionary<string, IPlugin> _loadedPlugins;
        private readonly ConcurrentDictionary<string, Assembly> _loadedAssemblies;
        private FileSystemWatcher _watcher;
        private IEndpointRouteBuilder _app;
        private RouteManager _routeManager;

        public PluginManager(IServiceCollection services, string pluginPath)
        {
            _services = services;
            _pluginPath = pluginPath;
            _loadedPlugins = new ConcurrentDictionary<string, IPlugin>();
            _loadedAssemblies = new ConcurrentDictionary<string, Assembly>();
            InitializeFileWatcher();
            PreloadPlugins(); // 预加载插件
        }

        private void InitializeFileWatcher()
        {
            _watcher = new FileSystemWatcher(_pluginPath)
            {
                Filter = "*.dll",
                EnableRaisingEvents = true,
                IncludeSubdirectories = true
            };

            _watcher.Created += OnPluginChanged;
            _watcher.Changed += OnPluginChanged;
            _watcher.Deleted += OnPluginDeleted;
        }

        private void PreloadPlugins()
        {
            if (!Directory.Exists(_pluginPath))
            {
                return;
            }

            foreach (var pluginDir in Directory.GetDirectories(_pluginPath))
            {
                var dllFiles = Directory.GetFiles(pluginDir, "*.dll");
                foreach (var dllFile in dllFiles)
                {
                    try
                    {
                        RegisterPluginServices(dllFile);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Error pre-loading plugin {dllFile}: {ex.Message}");
                    }
                }
            }
        }

        private void RegisterPluginServices(string pluginPath)
        {
            var assemblyName = Path.GetFileNameWithoutExtension(pluginPath);

            // 检查程序集是否已经加载
            if (_loadedAssemblies.TryGetValue(assemblyName, out var existingAssembly))
            {
                Console.WriteLine($"Assembly {assemblyName} is already loaded, skipping...");
                return;
            }

            var assembly = AssemblyLoadContext.Default.LoadFromAssemblyPath(pluginPath);
            _loadedAssemblies.TryAdd(assemblyName, assembly);

            Console.WriteLine($"开始加载程序集 {assemblyName}");
            var allTypes = assembly.GetTypes();
            Console.WriteLine($"程序集 {assemblyName} 中包含 {allTypes.Length} 个类型");

            // 注册带有PluginService特性的服务
            var serviceTypes = assembly.GetTypes()
                .Where(t => t.GetInterfaces().Any(i => i.GetCustomAttribute<PluginServiceAttribute>() != null));
            foreach (var serviceType in serviceTypes)
            {
                var serviceInterface = serviceType.GetInterfaces()
                    .FirstOrDefault(i => i.GetCustomAttribute<PluginServiceAttribute>() != null);
                if (serviceInterface != null)
                {
                    _services.AddScoped(serviceInterface, serviceType);
                }
            }

            // 注册插件中的所有控制器
            var controllerTypes = assembly.GetTypes()
                .Where(t => t.Name.EndsWith("Controller") && !t.IsAbstract && !t.IsInterface);
            var controllerList = controllerTypes.ToList();
            Console.WriteLine($"找到 {controllerList.Count} 个控制器类型");
            foreach (var controllerType in controllerList)
            {
                Console.WriteLine($"正在注册控制器: {controllerType.FullName}");
                _services.AddTransient(controllerType);
            }
        }

        public CommonResult<bool> LoadPlugin(string pluginPath)
        {
            var pluginDir = Path.GetDirectoryName(pluginPath);
            var configPath = Path.Combine(pluginDir, "plugin.json");
            try
            {

                // 检查插件是否已经加载
                var assemblyName = Path.GetFileNameWithoutExtension(pluginPath);

                if (_loadedAssemblies.TryGetValue(assemblyName, out var assembly))
                {
                    // 如果程序集已加载，只需要注册路由
                    RegisterPluginRoutes(assembly, assemblyName);
                }
                else
                {
                    // 如果程序集未加载，先注册服务
                    RegisterPluginServices(pluginPath);
                    assembly = _loadedAssemblies[assemblyName];
                    RegisterPluginRoutes(assembly, assemblyName);
                }
                // 读取并修改配置
                var config = JObject.Parse(System.IO.File.ReadAllText(configPath));
                config["enabled"] = true;
                System.IO.File.WriteAllText(configPath, config.ToString((Newtonsoft.Json.Formatting)Formatting.Indented));
            }
            catch (Exception ex)
            {
                var config = JObject.Parse(System.IO.File.ReadAllText(configPath));
                config["enabled"] = true;
                System.IO.File.WriteAllText(configPath, config.ToString((Newtonsoft.Json.Formatting)Formatting.Indented));
                Console.WriteLine($"Error loading plugin {pluginPath}: {ex.Message}");
                return CommonResult<bool>.Error($"插件加载失败: {ex.Message}");
            }
            return CommonResult<bool>.Success(true, "插件加载成功");
        }

        private void RegisterPluginRoutes(Assembly assembly, string assemblyName)
        {
            if (_routeManager != null)
            {
                var controllerTypes = assembly.GetTypes()
                    .Where(t => t.Name.EndsWith("Controller") && !t.IsAbstract && !t.IsInterface);
                foreach (var controllerType in controllerTypes)
                {
                    _routeManager.RegisterPluginController(controllerType, assemblyName);
                }
            }
        }

        public void LoadAllPlugins()
        {
            if (!Directory.Exists(_pluginPath))
            {
                return;
            }

            foreach (var pluginDir in Directory.GetDirectories(_pluginPath))
            {
                var configPath = Path.Combine(pluginDir, "plugin.json");
                if (!pluginStatus(configPath))
                {
                    continue;
                }
                var dllFiles = Directory.GetFiles(pluginDir, "*.dll");
                foreach (var dllFile in dllFiles)
                {
                    try
                    {
                        // 只注册路由，因为服务已经在PreloadPlugins中注册
                        var assemblyName = Path.GetFileNameWithoutExtension(dllFile);
                        if (_loadedAssemblies.TryGetValue(assemblyName, out var assembly))
                        {
                            RegisterPluginRoutes(assembly, assemblyName);
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Error loading plugin {dllFile}: {ex.Message}");
                    }
                }
            }
        }

        public void SetEndpointRouteBuilder(IEndpointRouteBuilder endpointRouteBuilder)
        {
            Console.WriteLine("开始设置EndpointRouteBuilder...");
            _endpointRouteBuilder = endpointRouteBuilder;
            _app = endpointRouteBuilder;
            Console.WriteLine($"EndpointRouteBuilder设置完成，类型为: {endpointRouteBuilder.GetType().FullName}");

            _routeManager = new RouteManager(_endpointRouteBuilder, _app.ServiceProvider);
            Console.WriteLine("开始重新注册已加载插件的路由...");
        }

        private void OnPluginChanged(object sender, FileSystemEventArgs e)
        {
            // 处理插件变更
            LoadPlugin(e.FullPath);
        }

        private void OnPluginDeleted(object sender, FileSystemEventArgs e)
        {
            // 处理插件删除
            var assemblyName = Path.GetFileNameWithoutExtension(e.FullPath);
            if (_loadedPlugins.TryRemove(assemblyName, out var plugin))
            {
                plugin.Stop();
            }
            _loadedAssemblies.TryRemove(assemblyName, out _);
        }

        public void UnloadPlugin(string pluginId)
        {
            var pluginPath = GetPluginPath(pluginId);
            if (pluginPath != null)
            {
                var dllFiles = Directory.GetFiles(pluginPath, "*.dll");
                foreach (var dllFile in dllFiles)
                {
                    var assemblyName = Path.GetFileNameWithoutExtension(dllFile);
                    if (_loadedAssemblies.TryGetValue(assemblyName, out var assembly))
                    {
                        // 注销路由
                        if (_routeManager != null)
                        {
                            var controllerTypes = assembly.GetTypes()
                                .Where(t => t.Name.EndsWith("Controller") && !t.IsAbstract && !t.IsInterface);
                            foreach (var controllerType in controllerTypes)
                            {
                                _routeManager.UnregisterPluginController(controllerType, assemblyName);
                            }
                        }
                    }
                }

                // 更新插件配置
                var configPath = Path.Combine(pluginPath, "plugin.json");
                if (File.Exists(configPath))
                {
                    var config = JObject.Parse(File.ReadAllText(configPath));
                    config["enabled"] = false;
                    //File.WriteAllText(configPath, config.ToString(Newtonsoft.Json.Formatting.Indented));
                }
            }

            if (_loadedPlugins.TryRemove(pluginId, out var plugin))
            {
                plugin.Stop();
            }
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

        private bool pluginStatus(string configPath)
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
}