

using Fastdotnet.Core.Dtos.Sys;

namespace Fastdotnet.Plugin.Shared.AdapterAOT
{
    public interface IPluginLoadService
    {
        Task<List<PluginInfo>> ScanPluginsAsync();
        Task<ApiResult> EnablePluginAsync(string pluginId);
        Task<ApiResult> DisablePluginAsync(string pluginId, bool ManualStop);
        Task<ApiResult> UninstallPluginAsync(string pluginId);
        Task<ApiResult> InstallPlugin(string pluginId, string Version, string UserToken);
        Task<bool> SetAuthCode(string AuthCode);
        Task<string> GetAuthCode();
        Task<bool> SetPluginLicense(SetPluginLicenseDto setPluginLicenseDto);
        Task<bool> UpdatePluginLicenseOnline(string pluginId, string UserToken);
        bool IsPluginActive(string pluginId);
        IEnumerable<PluginInfo> GetLoadedPlugins();
        IEnumerable<string> GetActivePlugins();
        Task StartInstalledPlugins();
        bool TryGetPluginScope(string pluginId, [MaybeNullWhen(false)] out ILifetimeScope scope);
    }
}
