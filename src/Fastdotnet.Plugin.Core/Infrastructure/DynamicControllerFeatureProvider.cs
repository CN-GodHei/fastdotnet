using Microsoft.AspNetCore.Mvc.ApplicationParts;
using Microsoft.AspNetCore.Mvc.Controllers;
using System.Collections.Generic;
using System.Reflection;
using System;
using System.Linq;
using Microsoft.AspNetCore.Mvc;

namespace Fastdotnet.Plugin.Core.Infrastructure
{
    /// <summary>
    /// Dynamically provides controllers from currently loaded plugins.
    /// This provider does not scan parts itself, but relies on PluginManager
    /// to get the active list of plugin assemblies.
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
            // We don't care about the 'parts' argument because PluginManager is our source of truth.
            
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
                    // Ignore assemblies that fail to load types.
                }
            }
        }
    }
}