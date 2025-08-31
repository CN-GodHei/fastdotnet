using Autofac;
using Autofac.Extensions.DependencyInjection;
using Fastdotnet.Core.Initializers;
using Fastdotnet.Core.IService;
using Fastdotnet.Core.Middleware;
using Fastdotnet.Core.Models;
using Fastdotnet.Core.Service;
using Fastdotnet.Orm;
using Fastdotnet.Plugin.Contracts;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Threading.Tasks;

namespace Fastdotnet.Plugin.Core.Infrastructure
{
    public interface IPluginLoadService
    {
        Task<List<PluginConfig>> ScanPluginsAsync();
        Task<ApiResult> EnablePluginAsync(string pluginId);
        Task<ApiResult> DisablePluginAsync(string pluginId);
        Task<ApiResult> UninstallPluginAsync(string pluginId);
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
        private readonly ILoggerFactory _loggerFactory;
        private readonly PluginStaticFileProviderRegistry _registry;
        private readonly IConfiguration _configuration;

        private readonly ConcurrentDictionary<string, ILifetimeScope> _pluginScopes = new();
        private readonly string _pluginsPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "plugins");
        private bool _disposed = false;

        public PluginLoadService(PluginManager pluginManager, ILifetimeScope lifetimeScope, ILogger<PluginLoadService> logger, ILoggerFactory loggerFactory, PluginStaticFileProviderRegistry registry, IConfiguration configuration)
        {
            _pluginManager = pluginManager;
            _rootLifetimeScope = lifetimeScope;
            _logger = logger;
            _loggerFactory = loggerFactory;
            _registry = registry;
            _configuration = configuration;
        }

        public bool TryGetPluginScope(string pluginId, [MaybeNullWhen(false)] out ILifetimeScope scope)
        {
            return _pluginScopes.TryGetValue(pluginId, out scope);
        }

        public async Task<List<PluginConfig>> ScanPluginsAsync()
        {
            var configs = new List<PluginConfig>();
            if (Directory.Exists(_pluginsPath))
            {
                foreach (var pluginDir in Directory.GetDirectories(_pluginsPath))
                {
                    var configPath = Path.Combine(pluginDir, "plugin.json");
                    if (File.Exists(configPath))
                    {
                        var configJson = await File.ReadAllTextAsync(configPath);
                        var config = JsonSerializer.Deserialize<PluginConfig>(configJson, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                        if (config != null) configs.Add(config);
                    }
                }
            }
            return configs;
        }

        public async Task<ApiResult> EnablePluginAsync(string pluginId)
        {
            if (IsPluginActive(pluginId)) return new ApiResult() { Code = 200, Msg = $"插件 {pluginId} 已启用。" };

            var pluginDir = Path.Combine(_pluginsPath, pluginId);
            if (!Directory.Exists(pluginDir)) return new ApiResult() { Code = -1, Msg = $"找不到插件目录" };

            // --- 最终的授权验证逻辑 ---

            bool skipLicenseCheck = false;

            // 1. 官方插件豁免 (永久有效)
            if (pluginId.Equals("11365281228129286", StringComparison.OrdinalIgnoreCase))
            {
                skipLicenseCheck = true;
                _logger.LogInformation($"插件 {pluginId} 是官方核心插件，跳过授权验证。");
            }

            // 2. 开发者模式豁免 (仅在Debug编译模式下生效)
#if DEBUG
            var devModeEnabled = _configuration.GetValue<bool>("PluginSettings:EnableDeveloperMode");
            if (devModeEnabled)
            {
                skipLicenseCheck = true;
                _logger.LogWarning($"开发者模式已启用，所有插件的授权验证将被跳过。请确保不要在生产环境中使用此配置。");
            }
#endif

            if (!skipLicenseCheck)
            {
                var licenseValidator = new Security.LicenseValidator();
                if (!licenseValidator.Validate(pluginDir))
                {
                    await UpdatePluginConfigEnabledAsync(pluginId, false);
                    _logger.LogError($"插件 {pluginId} 的授权验证失败。请检查授权文件是否有效。");
                    return new ApiResult() { Code = -1, Msg = $"插件 '{pluginId}' 授权验证失败。" };
                }
                _logger.LogInformation($"插件 {pluginId} 授权验证通过。");
            }
            
            // --- 授权验证结束 ---

            Assembly assembly = _pluginManager.GetPluginAssembly(pluginId);
            System.Runtime.Loader.AssemblyLoadContext loadContext = _pluginManager.GetPluginContext(pluginId);

            if (assembly == null)
            {
                var configPath = Path.Combine(pluginDir, "plugin.json");
                if (!File.Exists(configPath)) return new ApiResult() { Code = -1, Msg = $"找不到plugin.json。" };

                var configJson = await File.ReadAllTextAsync(configPath);
                var config = JsonSerializer.Deserialize<PluginConfig>(configJson, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                if (config == null) return new ApiResult() { Code = -1, Msg = $"无法解析plugin.json。" };

                if (config.dependencies != null)
                {
                    foreach (var depId in config.dependencies)
                    {
                        if (!IsPluginActive(depId))
                            return new ApiResult() { Code = -1, Msg = $"加载失败：依赖项 {depId} 未加载或未启用。" };
                    }
                }

                var entryPoint = string.IsNullOrEmpty(config.entryPoint) ? config.id + ".dll" : config.entryPoint;
                var dllPath = Path.Combine(pluginDir, entryPoint);
                if (!File.Exists(dllPath)) return new ApiResult() { Code = -1, Msg = $"找不到入口文件 {entryPoint}。" };

                var loadResult = _pluginManager.LoadPlugin(config, dllPath);
                if (loadResult == null)
                {
                    return new ApiResult() { Code = -1, Msg = $"插件程序集加载失败。" };
                }

                assembly = loadResult.Value.Assembly;
                loadContext = loadResult.Value.Context;
            }

            try
            {
                if (assembly == null || loadContext == null) return new ApiResult() { Code = -1, Msg = $"无法获取插件程序集或加载上下文。" };
                var pluginType = assembly.GetTypes().FirstOrDefault(t => typeof(IPlugin).IsAssignableFrom(t) && !t.IsAbstract);
                if (pluginType != null)
                {
                    var pluginScope = _rootLifetimeScope.BeginLoadContextLifetimeScope(loadContext, builder =>
                    {
                        var tempPluginInstance = (IPlugin)Activator.CreateInstance(pluginType);
                        tempPluginInstance.ConfigureServices(builder);

                        builder.RegisterType(pluginType).As<IPlugin>().InstancePerLifetimeScope();
                        builder.RegisterAssemblyTypes(assembly)
                               .Where(t => typeof(ControllerBase).IsAssignableFrom(t))
                               .AsSelf()
                               .InstancePerDependency();

                        // 注册插件中的IPermissionProvider实现
                        builder.RegisterAssemblyTypes(assembly)
                               .Where(t => typeof(IPermissionProvider).IsAssignableFrom(t) && !t.IsAbstract)
                               .As<IPermissionProvider>()
                               .InstancePerDependency();

                        // 注册通用仓储服务，解决插件控制器中IRepository依赖注入问题
                        builder.RegisterGeneric(typeof(Repository<>)).As(typeof(IRepository<>)).InstancePerDependency();
                        builder.RegisterGeneric(typeof(Repository<,>)).As(typeof(IRepository<,>)).InstancePerDependency();

                        // 注册插件中的IApplicationInitializer实现
                        builder.RegisterAssemblyTypes(assembly)
                               .Where(t => typeof(IApplicationInitializer).IsAssignableFrom(t) && !t.IsAbstract)
                               .As<IApplicationInitializer>()
                               .InstancePerLifetimeScope();
                    });

                    _pluginScopes.TryAdd(pluginId, pluginScope);

                    var pluginInstance = pluginScope.Resolve<IPlugin>();

                    // 添加插件的CodeFirst支持
                    // 使用已经加载的程序集，避免重复加载
                    var assemblyForCodeFirst = _pluginManager.GetPluginAssembly(pluginId);
                    if (assemblyForCodeFirst != null)
                    {
                        var serviceProvider = new AutofacServiceProvider(pluginScope);
                        serviceProvider.UsePluginCodeFirst(assemblyForCodeFirst);

                        // 执行插件的种子数据初始化器
                        try
                        {
                            var initializers = pluginScope.Resolve<IEnumerable<IApplicationInitializer>>();
                            // 仅执行当前插件的初始化器
                            var pluginInitializers = initializers.Where(i => i.GetType().Assembly == assemblyForCodeFirst);
                            foreach (var initializer in pluginInitializers)
                            {
                                await initializer.InitializeAsync();
                            }
                        }
                        catch (Exception ex)
                        {
                            _logger?.LogWarning(ex, $"执行插件 {pluginId} 的数据初始化时发生错误");
                        }

                        // 同步插件权限
                        try
                        {
                            // 只获取插件程序集中的IPermissionProvider实现
                            var permissionProviders = pluginScope.Resolve<IEnumerable<IPermissionProvider>>()
                                .Where(p => p.GetType().Assembly == assemblyForCodeFirst);

                            var permissionSyncService = pluginScope.Resolve<IPermissionSyncService>();

                            foreach (var provider in permissionProviders)
                            {
                                await permissionSyncService.SyncPluginPermissionsAsync(provider, pluginId);
                            }
                        }
                        catch (Exception ex)
                        {
                            _logger?.LogWarning(ex, $"同步插件 {pluginId} 的权限时发生错误");
                        }
                    }

                    await pluginInstance.InitializeAsync(new AutofacServiceProvider(pluginScope));
                    await pluginInstance.StartAsync();
                }

                // Register static files
                var pluginWwwRoot = Path.Combine(pluginDir, "wwwroot");
                if (Directory.Exists(pluginWwwRoot))
                {
                    _registry.Register($"/plugins/{pluginId}", pluginWwwRoot);
                }
                // 更新 plugin.json 文件，将 enabled 设置为 true
                await UpdatePluginConfigEnabledAsync(pluginId, true);
                _logger?.LogInformation($"插件 {pluginId} 启用成功。");
                return new ApiResult() { Code = 200, Msg = $"插件 {pluginId} 启用成功。" };
            }
            catch (Exception ex)
            {
                _logger?.LogError(ex, $"启用插件 {pluginId} 失败");
                await DisablePluginAsync(pluginId);
                return new ApiResult() { Code = -1, Msg = $"启用插件失败: {ex.Message}" };
            }
        }

        public async Task<ApiResult> DisablePluginAsync(string pluginId)
        {
            // Unregister static files first
            _registry.Unregister($"/plugins/{pluginId}");

            if (!IsPluginActive(pluginId)) return new ApiResult() { Code = 200, Msg = $"插件 {pluginId} 已停用。" };

            var activePlugins = GetActivePlugins();
            foreach (var otherPluginId in activePlugins)
            {
                if (otherPluginId == pluginId) continue;
                var config = _pluginManager.GetPluginConfig(otherPluginId);
                if (config?.dependencies?.Contains(pluginId) == true)
                {
                    return new ApiResult() { Code = -1, Msg = $"无法停用插件 {pluginId}，因为活动的插件 {otherPluginId} 依赖于它。" };
                }
            }

            var pluginConfig = _pluginManager.GetPluginConfig(pluginId);
            string dllPath = string.Empty;
            if (pluginConfig != null)
            {
                var pluginDir = Path.Combine(_pluginsPath, pluginId);
                var entryPoint = string.IsNullOrEmpty(pluginConfig.entryPoint) ? pluginConfig.id + ".dll" : pluginConfig.entryPoint;
                dllPath = Path.Combine(pluginDir, entryPoint);
            }

            try
            {
                // 更新 plugin.json 文件，将 enabled 设置为 false
                await UpdatePluginConfigEnabledAsync(pluginId, false);
                var assembly = _pluginManager.GetPluginAssembly(pluginId);

                if (_pluginScopes.TryRemove(pluginId, out var pluginScope))
                {
                    var plugin = pluginScope.Resolve<IPlugin>();
                    await plugin.StopAsync();
                    await plugin.UnloadAsync(new AutofacServiceProvider(pluginScope));

                    pluginScope.Dispose();
                    _logger?.LogInformation("插件 [{pluginId}] 的DI作用域已成功销毁。", pluginId);
                }

                if (assembly != null)
                {
                    SqlSugarServiceCollectionExtensions.RemovePluginCodeFirst(new AutofacServiceProvider(_rootLifetimeScope), assembly);
                }

                _pluginManager.UnloadPlugin(pluginId);

                _logger?.LogInformation($"插件 {pluginId} 已成功停用并卸载。");

                // 如果文件仍然被锁定，启动破坏性诊断
                if (!string.IsNullOrEmpty(dllPath) && new FileInfo(dllPath).Exists && IsFileLocked(new FileInfo(dllPath)))
                {
                    await PluginDiagnostics.PerformDestructiveDiagnostics(dllPath, new AutofacServiceProvider(_rootLifetimeScope), _logger);
                }

                return new ApiResult() { Code = 200, Msg = $"插件已成功停用。" };
            }
            catch (Exception ex)
            {
                _logger?.LogError(ex, $"停用插件 {pluginId} 失败");
                return new ApiResult() { Code = -1, Msg = $"停用插件失败: {ex.Message}" };
            }
        }

        private bool IsFileLocked(FileInfo file)
        {
            try
            {
                using (FileStream stream = file.Open(FileMode.Open, FileAccess.ReadWrite, FileShare.None))
                {
                    stream.Close();
                }
            }
            catch (IOException)
            {
                return true;
            }
            return false;
        }

        public async Task<ApiResult> UninstallPluginAsync(string pluginId)
        {
            if (IsPluginActive(pluginId))
            {
                return new ApiResult() { Code = -1, Msg = $"无法卸载，请先停用插件。" };
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
                return new ApiResult() { Code = 200, Msg = $"插件已成功卸载。" };
            }
            catch (Exception ex)
            {
                _logger?.LogError(ex, $"物理删除插件 {pluginId} 失败");
                return new ApiResult() { Code = -1, Msg = $"卸载插件失败: {ex.Message}" };
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

        private async Task UpdatePluginConfigEnabledAsync(string pluginId, bool enabled)
        {
            var pluginDir = Path.Combine(_pluginsPath, pluginId);
            var configPath = Path.Combine(pluginDir, "plugin.json");

            // 尝试从插件管理器获取已有配置
            PluginConfig? pluginConfig = _pluginManager.GetPluginConfig(pluginId);

            // 如果缓存中没有，尝试从文件加载
            if (pluginConfig == null)
            {
                if (!File.Exists(configPath))
                {
                    _logger?.LogWarning($"插件 {pluginId} 的 plugin.json 文件不存在，无法更新 enabled 状态。");
                    return; // 或抛出异常：throw new FileNotFoundException($"插件配置文件不存在：{configPath}");
                }

                try
                {
                    string jsonContent = await File.ReadAllTextAsync(configPath, Encoding.UTF8);
                    pluginConfig = JsonSerializer.Deserialize<PluginConfig>(jsonContent);
                    if (pluginConfig == null)
                    {
                        _logger?.LogError($"无法反序列化插件 {pluginId} 的 plugin.json 文件。");
                        return;
                    }
                }
                catch (Exception ex)
                {
                    _logger?.LogError(ex, $"读取或解析插件 {pluginId} 的 plugin.json 时发生错误。");
                    throw; // 可根据需要改为不抛出
                }
            }

            // 确保配置对象存在后再更新
            if (pluginConfig != null)
            {
                pluginConfig.enabled = enabled; // 注意：属性名应统一为 PascalCase，如 Enabled（假设类中是这样定义的）

                var options = new JsonSerializerOptions
                {
                    WriteIndented = true,
                    Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping // 防止中文被编码为 \uXXXX
                };

                try
                {
                    var updatedConfigJson = JsonSerializer.Serialize(pluginConfig, options);
                    await File.WriteAllTextAsync(configPath, updatedConfigJson, Encoding.UTF8);
                    _logger?.LogInformation($"插件 {pluginId} 的 plugin.json 文件已更新，Enabled 设置为 {enabled}。");
                }
                catch (Exception ex)
                {
                    _logger?.LogError(ex, $"写入插件 {pluginId} 的 plugin.json 文件失败。");
                    throw;
                }
            }
        }


    }
}
