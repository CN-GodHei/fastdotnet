using System;
using System.Collections.Concurrent;
using System.IO;
using System.Reflection;
using System.Runtime.Loader;
using Microsoft.Extensions.DependencyInjection;

namespace Fastdotnet.Core.Plugin
{
    public class PluginManager
    {
        private readonly IServiceCollection _services;
        private readonly string _pluginPath;
        private readonly ConcurrentDictionary<string, IPlugin> _loadedPlugins;
        private FileSystemWatcher _watcher;

        public PluginManager(IServiceCollection services, string pluginPath)
        {
            _services = services;
            _pluginPath = pluginPath;
            _loadedPlugins = new ConcurrentDictionary<string, IPlugin>();
            InitializeFileWatcher();
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

        private void OnPluginChanged(object sender, FileSystemEventArgs e)
        {
            if (Path.GetExtension(e.FullPath).Equals(".dll", StringComparison.OrdinalIgnoreCase))
            {
                LoadPlugin(e.FullPath);
            }
        }

        private void OnPluginDeleted(object sender, FileSystemEventArgs e)
        {
            if (Path.GetExtension(e.FullPath).Equals(".dll", StringComparison.OrdinalIgnoreCase))
            {
                UnloadPlugin(e.FullPath);
            }
        }

        public void LoadPlugin(string pluginPath)
        {
            try
            {
                var assembly = AssemblyLoadContext.Default.LoadFromAssemblyPath(pluginPath);
                var pluginTypes = assembly.GetTypes()
                    .Where(t => typeof(IPlugin).IsAssignableFrom(t) && !t.IsInterface && !t.IsAbstract);

                foreach (var pluginType in pluginTypes)
                {
                    var plugin = (IPlugin)Activator.CreateInstance(pluginType);
                    if (plugin != null)
                    {
                        if (_loadedPlugins.TryAdd(plugin.Id, plugin))
                        {
                            // 注册插件服务
                            _services.AddSingleton(pluginType, plugin);
                            plugin.Initialize();
                            plugin.Start();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                // 记录加载插件时的错误
                Console.WriteLine($"Error loading plugin {pluginPath}: {ex.Message}");
            }
        }

        public void UnloadPlugin(string pluginPath)
        {
            var pluginId = Path.GetFileNameWithoutExtension(pluginPath);
            if (_loadedPlugins.TryRemove(pluginId, out var plugin))
            {
                try
                {
                    plugin.Stop();
                    // 注意：在实际应用中，可能需要更复杂的服务注销逻辑
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error unloading plugin {pluginId}: {ex.Message}");
                }
            }
        }

        public void LoadAllPlugins()
        {
            var pluginFiles = Directory.GetFiles(_pluginPath, "*.dll", SearchOption.AllDirectories);
            foreach (var file in pluginFiles)
            {
                LoadPlugin(file);
            }
        }

        public IPlugin GetPlugin(string pluginId)
        {
            _loadedPlugins.TryGetValue(pluginId, out var plugin);
            return plugin;
        }

        public IEnumerable<IPlugin> GetAllPlugins()
        {
            return _loadedPlugins.Values;
        }
    }
}