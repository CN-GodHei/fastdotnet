using Fastdotnet.Plugin.Contracts;
using Microsoft.AspNetCore.Mvc.ApplicationParts;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reflection;
using System.Runtime.Loader;
using System.Text;
using System.Threading.Tasks;

namespace Fastdotnet.Plugin.Shared.AdapterAOT
{
    public class PluginManager
    {
        private readonly ConcurrentDictionary<string, (AssemblyLoadContext Context, Assembly Assembly, PluginInfo Config, ApplicationPart Part)> _loadedPlugins = new();

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

        public PluginInfo GetPluginInfo(string pluginId)
        {
            return _loadedPlugins.TryGetValue(pluginId, out var pluginInfo) ? pluginInfo.Config : null;
        }

        public IEnumerable<PluginInfo> GetLoadedPluginInfos()
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

        public (Assembly Assembly, AssemblyLoadContext Context)? LoadPlugin(PluginInfo config, string pluginPath)
        {
            var alc = new PluginAssemblyLoadContext(pluginPath);

            // Load the assembly into a byte array and then from a memory stream
            // to avoid locking the original DLL file on disk.
            var assemblyBytes = File.ReadAllBytes(pluginPath);
            using var memoryStream = new MemoryStream(assemblyBytes);
            var loadedAssembly = alc.LoadFromStream(memoryStream);
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

                return (loadedAssembly, alc);
            }

            alc.Unload();
            return null;
        }

        public Assembly GetPluginAssembly(string pluginName)
        {
            return _loadedPlugins.TryGetValue(pluginName, out var pluginInfo) ? pluginInfo.Assembly : null;
        }

        public AssemblyLoadContext GetPluginContext(string pluginId)
        {
            return _loadedPlugins.TryGetValue(pluginId, out var pluginInfo) ? pluginInfo.Context : null;
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

                // After extensive testing, it has been determined that two full, blocking GC cycles are
                // required to reliably unload the AssemblyLoadContext after its associated Autofac
                // child scope has been disposed. The first cycle handles finalization of objects,
                // and the second cycle collects the now-unreferenced AssemblyLoadContext itself.
                GC.Collect(GC.MaxGeneration, GCCollectionMode.Forced, blocking: true);
                GC.WaitForPendingFinalizers();
                GC.Collect(GC.MaxGeneration, GCCollectionMode.Forced, blocking: true);

                GC.Collect(GC.MaxGeneration, GCCollectionMode.Forced, blocking: true);
                GC.WaitForPendingFinalizers();
                GC.Collect(GC.MaxGeneration, GCCollectionMode.Forced, blocking: true);
            }
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
