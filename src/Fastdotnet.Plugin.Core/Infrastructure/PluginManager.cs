using Fastdotnet.Plugin.Contracts;
using Microsoft.AspNetCore.Mvc.ApplicationParts;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.Loader;

namespace Fastdotnet.Plugin.Core.Infrastructure
{
    public class PluginManager
    {
        private readonly ConcurrentDictionary<string, (AssemblyLoadContext Context, Assembly Assembly, PluginConfig Config, ApplicationPart Part)> _loadedPlugins = new();
        
        private readonly ConcurrentDictionary<string, List<Type>> _pluginServiceTypes = new();
        
        private readonly ConcurrentDictionary<Type, Type> _pluginTypes = new();

        private readonly ApplicationPartManager _applicationPartManager;

        public IReadOnlyDictionary<Type, Type> PluginTypes => _pluginTypes;

        public PluginManager(ApplicationPartManager applicationPartManager)
        {
            _applicationPartManager = applicationPartManager;
        }

        public IEnumerable<Assembly> GetPluginAssemblies()
        {
            return _loadedPlugins.Values.Select(p => p.Assembly);
        }

        public bool IsPluginLoaded(string pluginId)
        {
            return _loadedPlugins.ContainsKey(pluginId);
        }

        public PluginConfig GetPluginConfig(string pluginId)
        {
            return _loadedPlugins.TryGetValue(pluginId, out var pluginInfo) ? pluginInfo.Config : null;
        }

        public IEnumerable<PluginConfig> GetLoadedPluginConfigs()
        {
            return _loadedPlugins.Values.Select(v => v.Config);
        }

        public bool IsTypeFromPluginAssembly(Type type)
        {
            return _loadedPlugins.Values.Any(p => p.Assembly == type.Assembly);
        }

        public bool TryGetPluginId(Type type, [MaybeNullWhen(false)] out string pluginId)
        {
            var pluginInfo = _loadedPlugins.Values.FirstOrDefault(p => p.Assembly == type.Assembly);
            if (pluginInfo != default)
            {
                pluginId = pluginInfo.Config.id;
                return true;
            }
            pluginId = null;
            return false;
        }

        public Assembly LoadPlugin(PluginConfig config, string pluginPath)
        {
            var alc = new PluginAssemblyLoadContext(pluginPath);
            var loadedAssembly = alc.LoadFromAssemblyPath(pluginPath);
            var part = new AssemblyPart(loadedAssembly);

            if (_loadedPlugins.TryAdd(config.id, (alc, loadedAssembly, config, part)))
            {
                var serviceTypes = new List<Type>();
                var types = loadedAssembly.GetExportedTypes().Where(t => t.IsClass && !t.IsAbstract);
                foreach (var type in types)
                {
                    foreach (var interfaceType in type.GetInterfaces())
                    {
                        _pluginTypes.TryAdd(interfaceType, type);
                        serviceTypes.Add(interfaceType);
                    }
                }
                _pluginServiceTypes.TryAdd(config.id, serviceTypes);

                _applicationPartManager.ApplicationParts.Add(part);
                
                ActionDescriptorChangeProvider.Instance.NotifyChanges();

                return loadedAssembly;
            }

            alc.Unload();
            return null;
        }

        public Assembly GetPluginAssembly(string pluginName)
        {
            return _loadedPlugins.TryGetValue(pluginName, out var pluginInfo) ? pluginInfo.Assembly : null;
        }

        public void UnloadPlugin(string pluginId)
        {
            if (_loadedPlugins.TryRemove(pluginId, out var pluginInfo))
            {
                var (context, _, _, part) = pluginInfo;

                if (part != null)
                {
                    _applicationPartManager.ApplicationParts.Remove(part);
                }
                // 关键：通知MVC框架，路由和控制器已经发生变化
                ActionDescriptorChangeProvider.Instance.NotifyChanges();

                if (_pluginServiceTypes.TryRemove(pluginId, out var serviceTypes))
                {
                    foreach (var serviceType in serviceTypes)
                    {
                        _pluginTypes.TryRemove(serviceType, out _);
                    }
                }
                
                context.Unload();

                ForceGC();
            }
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        private static void ForceGC()
        {
            GC.Collect(GC.MaxGeneration, GCCollectionMode.Forced);
            GC.WaitForPendingFinalizers();
            GC.Collect(GC.MaxGeneration, GCCollectionMode.Forced);
        }
    }

    internal class PluginAssemblyLoadContext : AssemblyLoadContext
    {
        private readonly AssemblyDependencyResolver _resolver;

        public PluginAssemblyLoadContext(string pluginPath) : base(isCollectible: true)
        {
            _resolver = new AssemblyDependencyResolver(pluginPath);
        }

        protected override Assembly Load(AssemblyName assemblyName)
        {
            string assemblyPath = _resolver.ResolveAssemblyToPath(assemblyName);
            if (assemblyPath != null)
            {
                return LoadFromAssemblyPath(assemblyPath);
            }

            var defaultAssembly = AssemblyLoadContext.Default.LoadFromAssemblyName(assemblyName);
            if (defaultAssembly != null)
            {
                return defaultAssembly;
            }

            return null;
        }
    }
}
