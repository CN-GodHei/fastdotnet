using System.Reflection;
using System.Runtime.Loader;
using Autofac;
using Microsoft.AspNetCore.Mvc.ApplicationParts;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.Extensions.Logging;

namespace Fastdotnet.Plugin.Core.Infrastructure
{
    public class PluginManager
    {
        private readonly ILifetimeScope _lifetimeScope;
        private readonly ApplicationPartManager _partManager;
        private readonly Dictionary<string, AssemblyLoadContext> _loadContexts;
        private readonly Dictionary<string, Assembly> _loadedPlugins;
        private readonly ILogger<PluginManager> _logger;

        public IReadOnlyDictionary<string, Assembly> LoadedPlugins => _loadedPlugins;

        public PluginManager(ILifetimeScope lifetimeScope, ApplicationPartManager partManager, ILogger<PluginManager> logger = null)
        {
            _lifetimeScope = lifetimeScope ?? throw new ArgumentNullException(nameof(lifetimeScope));
            _partManager = partManager ?? throw new ArgumentNullException(nameof(partManager));
            _logger = logger;
            _loadContexts = new Dictionary<string, AssemblyLoadContext>();
            _loadedPlugins = new Dictionary<string, Assembly>();
        }

        public async Task LoadPluginAsync(string pluginPath)
        {
            if (string.IsNullOrEmpty(pluginPath))
                throw new ArgumentNullException(nameof(pluginPath));

            if (!File.Exists(pluginPath))
                throw new FileNotFoundException($"Plugin assembly not found at {pluginPath}");

            var pluginName = Path.GetFileNameWithoutExtension(pluginPath);
            if (_loadedPlugins.ContainsKey(pluginName))
            {
                _logger?.LogWarning($"Plugin {pluginName} is already loaded");
                throw new InvalidOperationException($"Plugin {pluginName} is already loaded");
            }

            try
            {
                var loadContext = new CollectibleAssemblyLoadContext();
                await using var fs = new FileStream(pluginPath, FileMode.Open, FileAccess.Read);
                var assembly = loadContext.LoadFromStream(fs);

                _loadContexts[pluginName] = loadContext;
                _loadedPlugins[pluginName] = assembly;

                var assemblyPart = new AssemblyPart(assembly);
                _partManager.ApplicationParts.Add(assemblyPart);

                // 重新构建控制器特性提供程序
                var feature = new ControllerFeature();
                _partManager.PopulateFeature(feature);

                _logger?.LogInformation($"Plugin {pluginName} loaded successfully");
            }
            catch (Exception ex)
            {
                _logger?.LogError(ex, $"Failed to load plugin {pluginName}");

                // 清理已分配的资源
                if (_loadContexts.TryGetValue(pluginName, out var context))
                {
                    _loadContexts.Remove(pluginName);
                    try
                    {
                        ((CollectibleAssemblyLoadContext)context).Unload();
                    }
                    catch (Exception unloadEx)
                    {
                        _logger?.LogError(unloadEx, $"Error unloading context for plugin {pluginName}");
                    }
                }

                if (_loadedPlugins.ContainsKey(pluginName))
                {
                    _loadedPlugins.Remove(pluginName);
                }

                throw; // 重新抛出异常以便调用者处理
            }
        }

        public void UnloadPlugin(string pluginName)
        {
            if (string.IsNullOrEmpty(pluginName))
                throw new ArgumentNullException(nameof(pluginName));

            if (!_loadedPlugins.ContainsKey(pluginName))
            {
                _logger?.LogWarning($"Plugin {pluginName} is not loaded");
                throw new InvalidOperationException($"Plugin {pluginName} is not loaded");
            }

            try
            {
                _logger?.LogInformation($"Unloading plugin {pluginName}");

                // 移除ApplicationPart
                var assembly = _loadedPlugins[pluginName];
                var assemblyPart = _partManager.ApplicationParts
                    .FirstOrDefault(part => part is AssemblyPart ap && ap.Assembly == assembly);

                if (assemblyPart != null)
                {
                    _partManager.ApplicationParts.Remove(assemblyPart);
                    var feature = new ControllerFeature();
                    _partManager.PopulateFeature(feature);
                    _logger?.LogDebug($"Removed assembly part for plugin {pluginName}");
                }

                // 从已加载插件字典中移除
                _loadedPlugins.Remove(pluginName);
                _logger?.LogDebug($"Removed plugin {pluginName} from loaded plugins dictionary");

                // 卸载AssemblyLoadContext
                if (_loadContexts.TryGetValue(pluginName, out var loadContext))
                {
                    _loadContexts.Remove(pluginName);

                    try
                    {
                        ((CollectibleAssemblyLoadContext)loadContext).Unload();
                        _logger?.LogDebug($"Unloaded assembly context for plugin {pluginName}");
                    }
                    catch (Exception ex)
                    {
                        _logger?.LogError(ex, $"Error unloading context for plugin {pluginName}");
                    }

                    // 强制GC回收
                    GC.Collect();
                    GC.WaitForPendingFinalizers();
                    _logger?.LogDebug($"Garbage collection completed for plugin {pluginName}");
                }

                _logger?.LogInformation($"Plugin {pluginName} unloaded successfully");
            }
            catch (Exception ex)
            {
                _logger?.LogError(ex, $"Error unloading plugin {pluginName}");
                throw; // 重新抛出异常以便调用者处理
            }
        }

        public bool IsPluginLoaded(string pluginName)
        {
            return _loadedPlugins.ContainsKey(pluginName);
        }

        public IEnumerable<string> GetLoadedPlugins()
        {
            return _loadedPlugins.Keys;
        }
    }

    public class CollectibleAssemblyLoadContext : AssemblyLoadContext
    {
        public CollectibleAssemblyLoadContext() : base(isCollectible: true)
        {
        }

        protected override Assembly Load(AssemblyName assemblyName)
        {
            // 返回null表示此上下文不处理程序集解析
            // 这样可以避免加载到默认上下文中的程序集被重复加载
            return null;
        }
    }
}
