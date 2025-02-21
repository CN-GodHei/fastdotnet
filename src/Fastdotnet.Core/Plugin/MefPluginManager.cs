using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using System.Reflection;
using System.Runtime.Loader;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Mvc.ApplicationParts;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;

namespace Fastdotnet.Core.Plugin
{
    public class MefPluginManager
    {
        private readonly IServiceCollection _services;
        private readonly string _pluginPath;
        private CompositionContainer _container;
        private readonly Dictionary<string, IMefPlugin> _loadedPlugins;
        private readonly Dictionary<string, AssemblyPart> _loadedAssemblyParts;
        private IEndpointRouteBuilder _endpointRouteBuilder;
        private RouteManager _routeManager;
        private FileSystemWatcher _watcher;

        [ImportMany]
        private IEnumerable<IMefPlugin> Plugins { get; set; }

        public MefPluginManager(IServiceCollection services, string pluginPath)
        {
            _services = services;
            _pluginPath = pluginPath;
            _loadedPlugins = new Dictionary<string, IMefPlugin>();
            _loadedAssemblyParts = new Dictionary<string, AssemblyPart>();
            InitializeContainer();
            InitializeFileWatcher();
        }

        private void InitializeContainer()
        {
            var catalog = new AggregateCatalog();

            // 添加当前程序集中的元数据导出
            var metadataCatalog = new AssemblyCatalog(typeof(PluginMetadataImpl).Assembly);
            catalog.Catalogs.Add(metadataCatalog);


            if (Directory.Exists(_pluginPath))
            {
                // 遍历插件目录下的所有子目录
                foreach (var pluginDir in Directory.GetDirectories(_pluginPath))
                {
                    // 检查plugin.json配置
                    var pluginJsonPath = Path.Combine(pluginDir, "plugin.json");
                    if (!File.Exists(pluginJsonPath) || !pluginStatus(pluginJsonPath))
                    {
                        Console.WriteLine($"跳过插件目录 {pluginDir}: plugin.json不存在或未启用");
                        continue;
                    }

                    // 获取目录下的所有dll文件
                    var dllFiles = Directory.GetFiles(pluginDir, "*.dll");
                    foreach (var dllFile in dllFiles)
                    {
                        try
                        {
                            var fileName = Path.GetFileName(dllFile);
                            // 排除核心程序集和系统程序集
                            if (fileName.StartsWith("Fastdotnet.Core", StringComparison.OrdinalIgnoreCase) ||
                                fileName.StartsWith("System.", StringComparison.OrdinalIgnoreCase) ||
                                fileName.StartsWith("Microsoft.", StringComparison.OrdinalIgnoreCase))
                            {
                                Console.WriteLine($"跳过系统程序集: {fileName}");
                                continue;
                            }

                            var assembly = Assembly.LoadFrom(dllFile);
                            var hasPluginTypes = assembly.GetTypes()
                                .Any(t => !t.IsAbstract && typeof(IMefPlugin).IsAssignableFrom(t));

                            if (!hasPluginTypes)
                            {
                                Console.WriteLine($"跳过非插件程序集: {fileName}");
                                continue;
                            }

                            Console.WriteLine($"加载插件程序集: {fileName}");
                            var pluginAssemblyCatalog = new AssemblyCatalog(assembly);
                            catalog.Catalogs.Add(pluginAssemblyCatalog);
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine($"Error loading plugin assembly {dllFile}: {ex.Message}");
                        }
                    }
                }
            }


            _container = new CompositionContainer(catalog);
            _container.ComposeParts(this);
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

        public void LoadAllPlugins()
        {
            if (!Directory.Exists(_pluginPath))
            {                
                Console.WriteLine($"Plugin directory {_pluginPath} does not exist.");
                return;
            }
        
            Console.WriteLine($"开始加载插件，当前MEF容器中的插件数量: {Plugins?.Count() ?? 0}");
            Console.WriteLine($"MEF容器状态: {(_container != null ? "已初始化" : "未初始化")}");
        
            // 遍历插件目录
            foreach (var pluginDir in Directory.GetDirectories(_pluginPath))
            {
                Console.WriteLine($"\n正在处理插件目录: {pluginDir}");
                var pluginJsonPath = Path.Combine(pluginDir, "plugin.json");
                if (!File.Exists(pluginJsonPath))
                {
                    Console.WriteLine($"Plugin.json not found in {pluginDir}");
                    continue;
                }
        
                try
                {
                    var pluginConfig = System.Text.Json.JsonSerializer.Deserialize<PluginConfig>(File.ReadAllText(pluginJsonPath));
                    if (pluginConfig == null || !pluginConfig.enabled)
                    {
                        Console.WriteLine($"Plugin in {pluginDir} is disabled or has invalid configuration.");
                        continue;
                    }
        
                    // 获取目录下的所有dll文件
                    var dllFiles = Directory.GetFiles(pluginDir, "*.dll");
                    foreach (var dllFile in dllFiles)
                    {
                        if (Path.GetFileName(dllFile).Equals("Fastdotnet.Core.dll", StringComparison.OrdinalIgnoreCase))
                        {
                            continue;
                        }
                        Console.WriteLine($"\n正在处理DLL文件: {dllFile}");
                        Console.WriteLine($"当前已加载的Plugins数量: {Plugins?.Count() ?? 0}");
                        Console.WriteLine($"当前已加载的插件实例数量: {_loadedPlugins.Count}");
        
                        // 根据dll文件位置匹配插件
                        var pluginsInDll = Plugins.Where(p => {
                            var pluginLocation = Path.GetFullPath(p.GetType().Assembly.Location);
                            var currentDll = Path.GetFullPath(dllFile);
                            return string.Equals(pluginLocation, currentDll, StringComparison.OrdinalIgnoreCase);
                        });
        
                        var matchedCount = pluginsInDll.Count();
                        Console.WriteLine($"在DLL中找到的匹配插件数量: {matchedCount}");
        
                        if (matchedCount == 0)
                        {
                            Console.WriteLine($"警告：在DLL {dllFile} 中未找到任何插件类型");
                            Console.WriteLine("请检查：");
                            Console.WriteLine("1. 插件类是否正确标记了[Export(typeof(IMefPlugin))]特性");
                            Console.WriteLine("2. 插件类是否正确实现了IMefPlugin接口");
                            Console.WriteLine("3. 插件程序集是否包含了所有必要的依赖");
                            continue;
                        }

                        // 直接加载找到的插件，让MEF容器处理元数据
                        foreach (var plugin in pluginsInDll)
                        {
                            if (plugin == null)
                            {
                                Console.WriteLine("Found null plugin instance");
                                continue;
                            }

                            Console.WriteLine($"准备加载插件: {plugin.Id}");
                            LoadPlugin(plugin);
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error loading plugin configuration from {pluginDir}: {ex.Message}");
                    Console.WriteLine($"Exception details: {ex}");
                }
            }
        }

        private class PluginConfig
        {
            public bool enabled { get; set; } = true;
        }

        private void LoadPlugin(IMefPlugin plugin)
        {
            if (_loadedPlugins.ContainsKey(plugin.Id))
            {
                return;
            }

            // 检查依赖
            if (!CheckDependencies(plugin))
            {
                Console.WriteLine($"Plugin {plugin.Id} dependencies not satisfied");
                return;
            }

            try
            {
                plugin.Initialize();
                if (plugin.Metadata.AutoStart)
                {
                    plugin.Start();
                }

                _loadedPlugins.Add(plugin.Id, plugin);

                // 注册插件中的控制器
                var assembly = plugin.GetType().Assembly;
                var controllers = assembly.GetTypes()
                    .Where(t => t.IsClass && !t.IsAbstract && t.Name.EndsWith("Controller"));

                foreach (var controller in controllers)
                {
                    _routeManager?.RegisterPluginController(controller, plugin.Id);
                }

                Console.WriteLine($"Plugin {plugin.Id} loaded successfully");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error loading plugin {plugin.Id}: {ex.Message}");
            }
        }

        private bool CheckDependencies(IMefPlugin plugin)
        {
            if (plugin.Metadata.Dependencies == null || !plugin.Metadata.Dependencies.Any())
            {
                return true;
            }

            return plugin.Metadata.Dependencies.All(dep => _loadedPlugins.ContainsKey(dep));
        }

        private void OnPluginChanged(object sender, FileSystemEventArgs e)
        {
            ReloadContainer();
        }

        private void OnPluginDeleted(object sender, FileSystemEventArgs e)
        {
            var pluginId = Path.GetFileNameWithoutExtension(e.Name);
            if (_loadedPlugins.TryGetValue(pluginId, out var plugin))
            {
                UnloadPlugin(plugin);
            }
        }

        private void UnloadPlugin(IMefPlugin plugin)
        {
            try
            {
                plugin.Stop();
                _loadedPlugins.Remove(plugin.Id);

                // 移除插件的路由
                var assembly = plugin.GetType().Assembly;
                var controllers = assembly.GetTypes()
                    .Where(t => t.IsClass && !t.IsAbstract && t.Name.EndsWith("Controller"));

                foreach (var controller in controllers)
                {
                    _routeManager?.UnregisterPluginController(controller, plugin.Id);
                }

                Console.WriteLine($"Plugin {plugin.Id} unloaded successfully");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error unloading plugin {plugin.Id}: {ex.Message}");
            }
        }

        private void ReloadContainer()
        {
            var oldPlugins = new List<IMefPlugin>(_loadedPlugins.Values);
            foreach (var plugin in oldPlugins)
            {
                UnloadPlugin(plugin);
            }

            _container.Dispose();
            InitializeContainer();
            LoadAllPlugins();
        }

        public void SetEndpointRouteBuilder(IEndpointRouteBuilder endpointRouteBuilder)
        {
            _endpointRouteBuilder = endpointRouteBuilder;
            _routeManager = new RouteManager(_endpointRouteBuilder, endpointRouteBuilder.ServiceProvider);
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