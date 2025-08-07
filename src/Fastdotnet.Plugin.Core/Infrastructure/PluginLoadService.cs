using Autofac;
using Fastdotnet.Core.Models;
using Fastdotnet.Core.Plugin;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.Json;
using System.Threading.Tasks;
using System;

namespace Fastdotnet.Plugin.Core.Infrastructure
{
    public interface IPluginLoadService
    {
        Task<CommonResult> LoadPluginsAsync();
        Task<CommonResult> LoadPluginAsync(string pluginId);
        Task<CommonResult> UnloadPluginAsync(string pluginName);
        bool IsPluginLoaded(string pluginName);
        IEnumerable<string> GetLoadedPlugins();
        string GetPluginPath(string pluginId);
    }

    public class PluginLoadService : IPluginLoadService, IDisposable
    {
        private readonly PluginManager _pluginManager;
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<PluginLoadService> _logger;
        private readonly ConcurrentDictionary<string, IPlugin> _activePlugins = new ConcurrentDictionary<string, IPlugin>();
        private bool _disposed = false;
        private readonly string _pluginPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "plugins");

        public PluginLoadService(PluginManager pluginManager, IServiceProvider serviceProvider, ILogger<PluginLoadService> logger = null)
        {
            _pluginManager = pluginManager;
            _serviceProvider = serviceProvider;
            _logger = logger;
        }

        public async Task<CommonResult> LoadPluginsAsync()
        {
            if (!Directory.Exists(_pluginPath))
            {
                return CommonResult.Success("插件目录不存在，跳过加载。");
            }

            try
            {
                // 1. 读取所有插件的配置信息
                var allPluginConfigs = new Dictionary<string, (PluginConfig Config, string DllPath)>();
                foreach (var pluginDir in Directory.GetDirectories(_pluginPath))
                {
                    var configPath = Path.Combine(pluginDir, "plugin.json");
                    var dllPath = Path.Combine(pluginDir, Path.GetFileName(pluginDir) + ".dll");
                    if (File.Exists(configPath) && File.Exists(dllPath))
                    {
                        var configJson = await File.ReadAllTextAsync(configPath);
                        var config = JsonSerializer.Deserialize<PluginConfig>(configJson, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                        if (config != null && config.enabled)
                        {
                            allPluginConfigs[config.id] = (config, dllPath);
                        }
                    }
                }

                // 2. 拓扑排序以确定加载顺序
                var sortedPluginIds = TopologicalSort(allPluginConfigs.Values.Select(p => p.Config).ToList());

                // 3. 按顺序加载插件
                foreach (var pluginId in sortedPluginIds)
                {
                    if (IsPluginLoaded(pluginId)) continue;

                    var (config, dllPath) = allPluginConfigs[pluginId];
                    await LoadSinglePluginAsync(config, dllPath);
                }

                return CommonResult.Success("所有插件加载完成。");
            }
            catch (Exception ex)
            {
                _logger?.LogError(ex, "加载插件失败。");
                return CommonResult.Error($"加载插件时发生错误: {ex.Message}");
            }
        }

        public async Task<CommonResult> LoadPluginAsync(string pluginId)
        {
            if (IsPluginLoaded(pluginId)) return CommonResult.Success($"插件 {pluginId} 已加载。");

            var pluginDir = Path.Combine(_pluginPath, pluginId);
            var configPath = Path.Combine(pluginDir, "plugin.json");

            if (!File.Exists(configPath))
            {
                return CommonResult.Error($"插件 {pluginId} 的配置文件plugin.json不存在。");
            }

            var configJson = await File.ReadAllTextAsync(configPath);
            var config = JsonSerializer.Deserialize<PluginConfig>(configJson, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            if (config == null) return CommonResult.Error($"无法读取插件 {pluginId} 的配置。");

            var entryPoint = string.IsNullOrEmpty(config.entryPoint) ? config.id + ".dll" : config.entryPoint;
            var dllPath = Path.Combine(pluginDir, entryPoint);

            if (!File.Exists(dllPath))
            {
                return CommonResult.Error($"插件 {pluginId} 的入口文件 {entryPoint} 未找到。");
            }

            // 检查依赖项
            if (config.dependencies != null)
            {
                foreach (var dependency in config.dependencies)
                {
                    if (!IsPluginLoaded(dependency))
                    {
                        return CommonResult.Error($"插件 {pluginId} 的依赖项 {dependency} 未加载。");
                    }
                }
            }

            await LoadSinglePluginAsync(config, dllPath);
            return CommonResult.Success($"插件 {pluginId} 加载成功。");
        }

        private async Task LoadSinglePluginAsync(PluginConfig config, string dllPath)
        {
            try
            {
                var assembly = _pluginManager.LoadPlugin(config.id, dllPath);
                if (assembly == null) return;

                var pluginType = assembly.GetTypes().FirstOrDefault(t => typeof(IPlugin).IsAssignableFrom(t) && !t.IsAbstract);
                if (pluginType != null)
                {
                    using (var scope = _serviceProvider.CreateScope())
                    {
                        var pluginInstance = (IPlugin)ActivatorUtilities.CreateInstance(scope.ServiceProvider, pluginType);

                        // Note: We can't call ConfigureServices here as the container is already built.
                        // This is a limitation of runtime loading.

                        await pluginInstance.InitializeAsync();
                        await pluginInstance.StartAsync();
                        _activePlugins.TryAdd(config.id, pluginInstance);
                    }
                }
                _logger?.LogInformation($"插件 {config.id} 加载成功。");
            }
            catch (Exception ex)
            {
                _logger?.LogError(ex, $"加载插件 {config.id} 失败");
                await UnloadPluginAsync(config.id); // 尝试清理
            }
        }

        public async Task<CommonResult> UnloadPluginAsync(string pluginName)
        {
            if (!IsPluginLoaded(pluginName))
            {
                return CommonResult.Error($"插件 {pluginName} 未加载。");
            }

            // 检查是否有其他已加载的插件依赖此插件
            var loadedPlugins = GetLoadedPlugins();
            foreach (var loadedPluginId in loadedPlugins)
            {
                if (loadedPluginId == pluginName) continue;
                var configPath = Path.Combine(_pluginPath, loadedPluginId, "plugin.json");
                if (File.Exists(configPath))
                {
                    var configJson = await File.ReadAllTextAsync(configPath);
                    var config = JsonSerializer.Deserialize<PluginConfig>(configJson, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                    if (config?.dependencies?.Contains(pluginName) == true)
                    {
                        return CommonResult.Error($"无法卸载插件 {pluginName}，因为插件 {loadedPluginId} 依赖于它。");
                    }
                }
            }

            try
            {
                if (_activePlugins.TryRemove(pluginName, out var plugin))
                {
                    await plugin.StopAsync();
                    await plugin.UnloadAsync();
                }

                _pluginManager.UnloadPlugin(pluginName);

                _logger?.LogInformation($"插件 {pluginName} 卸载成功。");
                return CommonResult.Success("插件卸载成功。");
            }
            catch (Exception ex)
            {
                _logger?.LogError(ex, $"卸载插件 {pluginName} 失败");
                return CommonResult.Error($"卸载插件失败: {ex.Message}");
            }
        }

        public bool IsPluginLoaded(string pluginName)
        {
            return _activePlugins.ContainsKey(pluginName);
        }

        public IEnumerable<string> GetLoadedPlugins()
        {
            return _activePlugins.Keys;
        }

        public string GetPluginPath(string pluginId)
        {
            var pluginDir = Path.Combine(_pluginPath, pluginId);
            return Directory.Exists(pluginDir) ? pluginDir : null;
        }

        private List<string> TopologicalSort(List<PluginConfig> plugins)
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

        private void Visit(string nodeId, Dictionary<string, List<string>> graph, HashSet<string> visited, List<string> sorted, HashSet<string> recursionStack)
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

        public void Dispose()
        {
            if (!_disposed)
            {
                foreach (var pluginName in _activePlugins.Keys.ToList())
                {
                    UnloadPluginAsync(pluginName).GetAwaiter().GetResult();
                }
                _disposed = true;
            }
            GC.SuppressFinalize(this);
        }
    }
}
