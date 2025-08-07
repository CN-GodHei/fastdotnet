
using Fastdotnet.Core.Models;
using Fastdotnet.Core.Plugin;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System.Collections.Concurrent; // 【优化】引入并发集合命名空间
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
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<PluginLoadService> _logger;
        // 【优化】使用 ConcurrentDictionary 确保线程安全。
        private readonly ConcurrentDictionary<string, IPlugin> _activePlugins = new ConcurrentDictionary<string, IPlugin>();
        private bool _disposed = false;
        private readonly string _pluginPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "plugins");

        public PluginLoadService(PluginManager pluginManager, IServiceProvider serviceProvider, ILogger<PluginLoadService> logger = null)
        {
            _pluginManager = pluginManager;
            _serviceProvider = serviceProvider;
            _logger = logger;
        }

        public async Task<CommonResult<bool>> LoadPluginAsync(string pluginPath)
        {
            var pluginName = Path.GetFileNameWithoutExtension(pluginPath);
            if (IsPluginLoaded(pluginName))
            {
                return CommonResult<bool>.Error($"插件 {pluginName} 已加载。");
            }

            try
            {
                var assembly = _pluginManager.LoadPlugin(pluginName, pluginPath);
                if (assembly == null)
                {
                    return CommonResult<bool>.Error($"插件 {pluginName} 加载失败。");
                }

                var pluginType = assembly.GetTypes().FirstOrDefault(t => typeof(IPlugin).IsAssignableFrom(t) && !t.IsAbstract);
                if (pluginType != null)
                {
                    using (var scope = _serviceProvider.CreateScope())
                    {
                        var plugin = (IPlugin)ActivatorUtilities.CreateInstance(scope.ServiceProvider, pluginType);
                        await plugin.InitializeAsync();
                        await plugin.StartAsync();
                        // ConcurrentDictionary 的 AddOrUpdate 方法是线程安全的。
                        _activePlugins.TryAdd(pluginName, plugin);
                    }
                }

                _logger?.LogInformation($"插件 {pluginName} 加载成功。");
                return CommonResult<bool>.Success(true, "插件加载成功。");
            }
            catch (Exception ex)
            {
                _logger?.LogError(ex, $"加载插件 {pluginName} 失败");
                await UnloadPlugin(pluginName); // 尝试清理
                return CommonResult<bool>.Error($"加载插件失败: {ex.Message}");
            }
        }

        public async Task<CommonResult<bool>> UnloadPlugin(string pluginName)
        {
            if (!IsPluginLoaded(pluginName))
            {
                return CommonResult<bool>.Error($"插件 {pluginName} 未加载。");
            }

            try
            {
                // ConcurrentDictionary 的 TryRemove 方法是线程安全的。
                if (_activePlugins.TryRemove(pluginName, out var plugin))
                {
                    await plugin.StopAsync();
                    await plugin.UnloadAsync();
                }

                _pluginManager.UnloadPlugin(pluginName);

                _logger?.LogInformation($"插件 {pluginName} 卸载成功。");
                return CommonResult<bool>.Success(true, "插件卸载成功。");
            }
            catch (Exception ex)
            {
                _logger?.LogError(ex, $"卸载插件 {pluginName} 失败");
                return CommonResult<bool>.Error($"卸载插件失败: {ex.Message}");
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
                        var pluginConfig = JsonSerializer.Deserialize<PluginConfig>(configJson);
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
                if (!File.Exists(configPath))
                {
                    var defaultConfigPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "DefaultPluginConfig.json");
                    if (File.Exists(defaultConfigPath))
                    {
                        File.Copy(defaultConfigPath, configPath);
                    }
                    else
                    {
                        return false;
                    }
                }

                var configJson = File.ReadAllText(configPath);
                var pluginConfig = JsonSerializer.Deserialize<PluginConfig>(configJson);

                return pluginConfig.enabled;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public void Dispose()
        {
            if (!_disposed)
            {
                foreach (var pluginName in _activePlugins.Keys.ToList())
                {
                    UnloadPlugin(pluginName).GetAwaiter().GetResult();
                }
                _disposed = true;
            }
            GC.SuppressFinalize(this);
        }
    }
}
