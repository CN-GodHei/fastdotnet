using Autofac;
using Autofac.Extensions.DependencyInjection;
using Fastdotnet.Core.Models;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.Json;
using System.Threading.Tasks;
using System;
using Fastdotnet.Plugin.Contracts;
using Fastdotnet.Orm;
using Microsoft.AspNetCore.Mvc;

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
        bool TryGetPluginScope(string pluginId, [MaybeNullWhen(false)] out ILifetimeScope scope);
    }

    public class PluginLoadService : IPluginLoadService, IDisposable
    {
        private readonly PluginManager _pluginManager;
        private readonly ILifetimeScope _rootLifetimeScope;
        private readonly ILogger<PluginLoadService> _logger;
        
        private readonly ConcurrentDictionary<string, ILifetimeScope> _pluginScopes = new();
        private readonly string _pluginsPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "plugins");
        private bool _disposed = false;

        public PluginLoadService(PluginManager pluginManager, ILifetimeScope lifetimeScope, ILogger<PluginLoadService> logger = null)
        {
            _pluginManager = pluginManager;
            _rootLifetimeScope = lifetimeScope;
            _logger = logger;
        }

        public bool TryGetPluginScope(string pluginId, [MaybeNullWhen(false)] out ILifetimeScope scope)
        {
            return _pluginScopes.TryGetValue(pluginId, out scope);
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

            var assembly = _pluginManager.GetPluginAssembly(pluginId);
            if (assembly == null)
            {
                 var pluginDir = Path.Combine(_pluginsPath, pluginId);
                if (!Directory.Exists(pluginDir)) return CommonResult.Error("找不到插件目录。");

                var configPath = Path.Combine(pluginDir, "plugin.json");
                if (!File.Exists(configPath)) return CommonResult.Error("找不到plugin.json。");

                var configJson = await File.ReadAllTextAsync(configPath);
                var config = JsonSerializer.Deserialize<PluginConfig>(configJson, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                if (config == null) return CommonResult.Error("无法解析plugin.json。");

                if (config.dependencies != null)
                {
                    foreach (var depId in config.dependencies)
                    {
                        if (!IsPluginActive(depId))
                            return CommonResult.Error($"加载失败：依赖项 {depId} 未加载或未启用。");
                    }
                }

                var entryPoint = string.IsNullOrEmpty(config.entryPoint) ? config.id + ".dll" : config.entryPoint;
                var dllPath = Path.Combine(pluginDir, entryPoint);
                if (!File.Exists(dllPath)) return CommonResult.Error($"找不到入口文件 {entryPoint}。");

                assembly = _pluginManager.LoadPlugin(config, dllPath);
                if (assembly == null) return CommonResult.Error("插件程序集加载失败。");
            }

            try
            {
                SqlSugarServiceCollectionExtensions.UsePluginCodeFirst(new AutofacServiceProvider(_rootLifetimeScope), assembly);

                var pluginType = assembly.GetTypes().FirstOrDefault(t => typeof(IPlugin).IsAssignableFrom(t) && !t.IsAbstract);
                if (pluginType != null)
                {
                    var pluginScope = _rootLifetimeScope.BeginLifetimeScope(builder =>
                    {
                        var tempPluginInstance = (IPlugin)Activator.CreateInstance(pluginType);
                        tempPluginInstance.ConfigureServices(builder);
                        
                        builder.RegisterType(pluginType).As<IPlugin>().InstancePerLifetimeScope();
                        builder.RegisterAssemblyTypes(assembly)
                               .Where(t => typeof(ControllerBase).IsAssignableFrom(t))
                               .AsSelf()
                               .InstancePerDependency();
                    });

                    _pluginScopes.TryAdd(pluginId, pluginScope);

                    var pluginInstance = pluginScope.Resolve<IPlugin>();
                    
                    // 插件的 InitializeAsync 需要一个 IServiceProvider，它自己的子容器就是最好的选择
                    await pluginInstance.InitializeAsync(new AutofacServiceProvider(pluginScope));
                    await pluginInstance.StartAsync();
                }
                _logger?.LogInformation($"插件 {pluginId} 启用成功。");
                return CommonResult.Success($"插件 {pluginId} 启用成功。");
            }
            catch (Exception ex)
            {
                _logger?.LogError(ex, $"启用插件 {pluginId} 失败");
                await DisablePluginAsync(pluginId);
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
                var assembly = _pluginManager.GetPluginAssembly(pluginId);

                // 核心修复：在销毁作用域之前，先执行插件自己的清理逻辑
                if (_pluginScopes.TryRemove(pluginId, out var pluginScope))
                {
                    // 1. 从子容器中解析出插件实例
                    var plugin = pluginScope.Resolve<IPlugin>();
                    
                    // 2. 执行插件的停止和卸载方法，让它自己清理（比如注销中间件）
                    await plugin.StopAsync();
                    await plugin.UnloadAsync(new AutofacServiceProvider(pluginScope));
                    
                    // 3. 彻底销毁子容器，释放所有DI引用
                    pluginScope.Dispose();
                    _logger?.LogInformation("插件 [{pluginId}] 的DI作用域已成功销毁。", pluginId);
                }

                // 4. 清理ORM缓存
                if (assembly != null)
                {
                    SqlSugarServiceCollectionExtensions.RemovePluginCodeFirst(new AutofacServiceProvider(_rootLifetimeScope), assembly);
                }

                // 5. 卸载程序集
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
            if (IsPluginActive(pluginId))
            {
                return CommonResult.Error("无法卸载，请先停用插件。");
            }
            
            if (_pluginManager.IsPluginLoaded(pluginId))
            {
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

        public bool IsPluginActive(string pluginId) => _pluginScopes.ContainsKey(pluginId);
        public IEnumerable<string> GetActivePlugins() => _pluginScopes.Keys;
        
        public IEnumerable<PluginConfig> GetLoadedPlugins() => _pluginManager.GetLoadedPluginConfigs();

        public void Dispose()
        {
            if (!_disposed)
            {
                var activePluginIds = _pluginScopes.Keys.ToList();
                foreach (var pluginName in activePluginIds)
                {
                    DisablePluginAsync(pluginName).GetAwaiter().GetResult();
                }
                _disposed = true;
            }
            GC.SuppressFinalize(this);
        }
        
        public void StartInstalledPlugins()
        {
            var pluginDirs = Directory.GetDirectories(_pluginsPath)
                .Select(d => new DirectoryInfo(d).Name)
                .ToList();

            foreach (var pluginId in pluginDirs)
            {
                try
                {
                    _ = EnablePluginAsync(pluginId).GetAwaiter().GetResult();
                }
                catch (Exception ex)
                {
                    _logger?.LogError(ex, "启动已安装插件 [{pluginId}] 时发生错误。", pluginId);
                }
            }
        }
    }
}
