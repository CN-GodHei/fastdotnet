using Autofac;
using Fastdotnet.Core.Dtos;
using Fastdotnet.Plugin.Contracts;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fastdotnet.Plugin.Shared.AdapterAOT
{
    public interface IPluginLoadService
    {
        Task<List<PluginInfo>> ScanPluginsAsync();
        Task<ApiResult> EnablePluginAsync(string pluginId);
        Task<ApiResult> DisablePluginAsync(string pluginId);
        Task<ApiResult> UninstallPluginAsync(string pluginId);
        bool IsPluginActive(string pluginId);
        IEnumerable<PluginInfo> GetLoadedPlugins();
        IEnumerable<string> GetActivePlugins();
        void StartInstalledPlugins();
        bool TryGetPluginScope(string pluginId, [MaybeNullWhen(false)] out ILifetimeScope scope);
    }
}
