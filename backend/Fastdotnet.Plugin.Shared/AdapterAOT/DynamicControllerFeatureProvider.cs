using Microsoft.AspNetCore.Mvc.ApplicationParts;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc;
using System.Reflection;

namespace Fastdotnet.Plugin.Shared.AdapterAOT
{
    /// <summary>
    ///从当前加载的插件动态提供控制器。
    ///此提供程序本身不扫描零件，而是依赖于PluginManager
    ///获取插件程序集的活动列表。
    /// </summary>
    public class DynamicControllerFeatureProvider : IApplicationFeatureProvider<ControllerFeature>
    {
        private readonly PluginManager _pluginManager;

        public DynamicControllerFeatureProvider(PluginManager pluginManager)
        {
            _pluginManager = pluginManager;
        }

        public void PopulateFeature(IEnumerable<ApplicationPart> parts, ControllerFeature feature)
        {
            // 我们不关心“parts”的论点，因为PluginManager是我们的事实来源。

            var pluginAssemblies = _pluginManager.GetPluginAssemblies();

            foreach (var assembly in pluginAssemblies)
            {
                try
                {
                    var candidates = assembly.GetExportedTypes()
                        .Where(x => typeof(ControllerBase).IsAssignableFrom(x) && !x.IsAbstract);

                    foreach (var candidate in candidates)
                    {
                        var typeInfo = candidate.GetTypeInfo();
                        if (!feature.Controllers.Contains(typeInfo))
                        {
                            feature.Controllers.Add(typeInfo);
                        }
                    }
                }
                catch
                {
                    // 忽略无法加载类型的程序集。
                }
            }
        }
    }
}
