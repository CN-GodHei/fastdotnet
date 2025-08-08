using Autofac;
using Fastdotnet.Core.Models;
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
using Fastdotnet.Plugin.Contracts;

namespace Fastdotnet.Plugin.Core.Infrastructure
{
    public interface IPluginLoadService
    {
        Task<CommonResult<IEnumerable<PluginConfig>>> ScanPluginsAsync();
        Task<CommonResult> EnablePluginAsync(string pluginId);
        Task<CommonResult> DisablePluginAsync(string pluginId);
        Task<CommonResult> UninstallPluginAsync(string pluginId);
        bool IsPluginActive(string pluginId);
        IEnumerable<PluginConfig> GetLoadedPlugins();
        IEnumerable<string> GetActivePlugins();
        void StartInstalledPlugins();
    }

    public class PluginLoadService : IPluginLoadService, IDisposable
    {
        private readonly PluginManager _pluginManager;
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<PluginLoadService> _logger;
        private readonly ConcurrentDictionary<string, IPlugin> _activePlugins = new ConcurrentDictionary<string, IPlugin>();
        private readonly string _pluginsPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "plugins");
        private bool _disposed = false;

        public PluginLoadService(PluginManager pluginManager, IServiceProvider serviceProvider, ILogger<PluginLoadService> logger = null)
        {
            _pluginManager = pluginManager;
            _serviceProvider = serviceProvider;
            _logger = logger;
        }

        public Task<CommonResult<IEnumerable<PluginConfig>>> ScanPluginsAsync()
        {
            var configs = new List<PluginConfig>();
            if (Directory.Exists(_pluginsPath))
            {
                foreach (var pluginDir in Directory.GetDirectories(_pluginsPath))
                {
                    var configPath = Path.Combine(pluginDir, "plugin.json");
                    if (File.Exists(configPath))
                    {
                        var configJson = File.ReadAllText(configPath);
                        var config = JsonSerializer.Deserialize<PluginConfig>(configJson, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                        if (config != null) configs.Add(config);
                    }
                }
            }
            return Task.FromResult(CommonResult<IEnumerable<PluginConfig>>.Success(configs));
        }

        public async Task<CommonResult> EnablePluginAsync(string pluginId)
        {
            if (IsPluginActive(pluginId)) return CommonResult.Success($"插件 {pluginId} 已启用。");

            // Step 1: Technical Load (if not already loaded)
            if (!_pluginManager.IsPluginLoaded(pluginId))
            {
                var pluginDir = Path.Combine(_pluginsPath, pluginId);
                if (!Directory.Exists(pluginDir)) return CommonResult.Error("找不到插件目录。");

                var configPath = Path.Combine(pluginDir, "plugin.json");
                if (!File.Exists(configPath)) return CommonResult.Error("找不到plugin.json。");

                var configJson = await File.ReadAllTextAsync(configPath);
                var config = JsonSerializer.Deserialize<PluginConfig>(configJson, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                if (config == null) return CommonResult.Error("无法解析plugin.json。");

                // Check dependencies before loading
                if (config.dependencies != null)
                {
                    foreach (var depId in config.dependencies)
                    {
                        if (!_pluginManager.IsPluginLoaded(depId))
                            return CommonResult.Error($"加载失败：依赖项 {depId} 未加载。");
                    }
                }

                var entryPoint = string.IsNullOrEmpty(config.entryPoint) ? config.id + ".dll" : config.entryPoint;
                var dllPath = Path.Combine(pluginDir, entryPoint);
                if (!File.Exists(dllPath)) return CommonResult.Error($"找不到入口文件 {entryPoint}。");

                // The DI container cannot be modified at runtime. Services must be resolved dynamically.
                _pluginManager.LoadPlugin(config, dllPath);
            }

            // Step 2: Business Activation
            try
            {
                var assembly = _pluginManager.GetPluginAssembly(pluginId);
                if (assembly == null) return CommonResult.Error("找不到插件程序集。");

                var pluginType = assembly.GetTypes().FirstOrDefault(t => typeof(IPlugin).IsAssignableFrom(t) && !t.IsAbstract);
                if (pluginType != null)
                {
                    using (var scope = _serviceProvider.CreateScope())
                    {
                        var pluginInstance = (IPlugin)ActivatorUtilities.CreateInstance(scope.ServiceProvider, pluginType);
                        await pluginInstance.InitializeAsync(scope.ServiceProvider);
                        await pluginInstance.StartAsync();
                        _activePlugins.TryAdd(pluginId, pluginInstance);
                    }
                }
                _logger?.LogInformation($"插件 {pluginId} 启用成功。");
                return CommonResult.Success($"插件 {pluginId} 启用成功。");
            }
            catch (Exception ex)
            {
                _logger?.LogError(ex, $"启用插件 {pluginId} 失败");
                return CommonResult.Error($"启用插件失败: {ex.Message}");
            }
        }

        public async Task<CommonResult> DisablePluginAsync(string pluginId)
        {
            if (!IsPluginActive(pluginId)) return CommonResult.Success($"插件 {pluginId} 已停用。");

            var activePlugins = GetActivePlugins();
            foreach (var otherPluginId in activePlugins)
            {
                if (otherPluginId == pluginId) continue;
                var config = _pluginManager.GetPluginConfig(otherPluginId);
                if (config?.dependencies?.Contains(pluginId) == true)
                {
                    return CommonResult.Error($"无法停用插件 {pluginId}，因为活动的插件 {otherPluginId} 依赖于它。");
                }
            }

            try
            {
                if (_activePlugins.TryRemove(pluginId, out var plugin))
                {
                    using (var scope = _serviceProvider.CreateScope())
                    {
                        await plugin.StopAsync();
                        await plugin.UnloadAsync(scope.ServiceProvider);
                    }
                }
                _pluginManager.UnloadPlugin(pluginId);
                _logger?.LogInformation($"插件 {pluginId} 已成功停用并卸载。");
                return CommonResult.Success("插件已成功停用。");
            }
            catch (Exception ex)
            {
                _logger?.LogError(ex, $"停用插件 {pluginId} 失败");
                return CommonResult.Error($"停用插件失败: {ex.Message}");
            }
        }

        public async Task<CommonResult> UninstallPluginAsync(string pluginId)
        {
            if (_pluginManager.IsPluginLoaded(pluginId))
            {
                if (IsPluginActive(pluginId))
                    return CommonResult.Error("无法卸载，请先停用插件。");
                _pluginManager.UnloadPlugin(pluginId);
            }

            try
            {
                var pluginDir = Path.Combine(_pluginsPath, pluginId);
                if (Directory.Exists(pluginDir))
                {
                    Directory.Delete(pluginDir, true);
                }
                _logger?.LogInformation($"插件 {pluginId} 已被物理删除。");
                return CommonResult.Success("插件已成功卸载。");
            }
            catch (Exception ex)
            {
                _logger?.LogError(ex, $"物理删除插件 {pluginId} 失败");
                return CommonResult.Error($"卸载插件失败: {ex.Message}");
            }
        }

        public bool IsPluginActive(string pluginId) => _activePlugins.ContainsKey(pluginId);
        public IEnumerable<string> GetActivePlugins() => _activePlugins.Keys;
        public IEnumerable<PluginConfig> GetLoadedPlugins() => _pluginManager.GetLoadedPluginConfigs();

        public void Dispose()
        {
            if (!_disposed)
            {
                var activePluginIds = _activePlugins.Keys.ToList();
                foreach (var pluginName in activePluginIds)
                {
                    DisablePluginAsync(pluginName).GetAwaiter().GetResult();
                }
                _disposed = true;
            }
            GC.SuppressFinalize(this);
        }
        /// <summary>
        /// 启动已安装的插件
        /// </summary>
        public void StartInstalledPlugins()
        {
            string[] subDirectories = Directory.GetDirectories(_pluginsPath);
            List<string> pluginFolders = new List<string>();
            foreach (string dirPath in subDirectories)
            {
                string dirName = new DirectoryInfo(dirPath).Name;
                Console.WriteLine(dirName);

                try
                {
                    _=EnablePluginAsync(dirName);
                }
                catch (Exception)
                {

                    continue;
                }
            }
        }
    }
}
