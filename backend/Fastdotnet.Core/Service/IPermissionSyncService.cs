using Fastdotnet.Plugin.Contracts;
using System.Threading.Tasks;

namespace Fastdotnet.Core.Service
{
    /// <summary>
    /// 权限同步服务接口
    /// </summary>
    public interface IPermissionSyncService
    {
        /// <summary>
        /// 同步单个插件的权限
        /// </summary>
        /// <param name="provider">权限提供者</param>
        /// <param name="module">模块名称</param>
        /// <returns></returns>
        Task SyncPluginPermissionsAsync(IPermissionProvider provider, string module);
    }
}