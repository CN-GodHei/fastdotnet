
using Microsoft.AspNetCore.Mvc.ApplicationParts;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.Loader;

namespace Fastdotnet.Plugin.Core.Infrastructure
{
    public class PluginManager
    {
        private readonly ConcurrentDictionary<Type, Type> _pluginTypes = new ConcurrentDictionary<Type, Type>();
        private readonly ConcurrentDictionary<string, (AssemblyLoadContext, Assembly)> _loadedPlugins = new ConcurrentDictionary<string, (AssemblyLoadContext, Assembly)>();
        private readonly ApplicationPartManager _applicationPartManager;

        public IReadOnlyDictionary<Type, Type> PluginTypes => _pluginTypes;

        public PluginManager(ApplicationPartManager applicationPartManager)
        {
            _applicationPartManager = applicationPartManager;
        }

        // 【新增】辅助方法，用于判断一个类型是否来自于已加载的插件程序集。
        public bool IsTypeFromPluginAssembly(Type type)
        {
            return _loadedPlugins.Values.Any(p => p.Item2 == type.Assembly);
        }

        public Assembly LoadPlugin(string pluginName, string pluginPath)
        {
            var alc = new AssemblyLoadContext(pluginName, isCollectible: true);
            var loadedAssembly = alc.LoadFromAssemblyPath(pluginPath);

            if (_loadedPlugins.TryAdd(pluginName, (alc, loadedAssembly)))
            {
                // 注册服务
                var types = loadedAssembly.GetExportedTypes()
                    .Where(t => t.IsClass && !t.IsAbstract);

                foreach (var type in types)
                {
                    foreach (var interfaceType in type.GetInterfaces())
                    {
                        _pluginTypes.TryAdd(interfaceType, type);
                    }
                }

                // 注册控制器
                var part = new AssemblyPart(loadedAssembly);
                _applicationPartManager.ApplicationParts.Add(part);
                ActionDescriptorChangeProvider.Instance.NotifyChanges();
                return loadedAssembly;
            }
            return null;
        }

        public void UnloadPlugin(string pluginName)
        {
            if (_loadedPlugins.TryRemove(pluginName, out var pluginInfo))
            {
                var (alc, assembly) = pluginInfo;

                // 注销服务
                var types = assembly.GetExportedTypes()
                    .Where(t => t.IsClass && !t.IsAbstract);

                foreach (var type in types)
                {
                    foreach (var interfaceType in type.GetInterfaces())
                    {
                        _pluginTypes.TryRemove(interfaceType, out _);
                    }
                }

                // 注销控制器
                var part = _applicationPartManager.ApplicationParts.FirstOrDefault(p => p is AssemblyPart ap && ap.Assembly == assembly);
                if (part != null)
                {
                    _applicationPartManager.ApplicationParts.Remove(part);
                    ActionDescriptorChangeProvider.Instance.NotifyChanges();
                }

                alc.Unload();
            }
        }
    }
}
