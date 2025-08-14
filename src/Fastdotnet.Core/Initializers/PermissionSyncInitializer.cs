using Fastdotnet.Core.Service;
using Fastdotnet.Plugin.Contracts;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Fastdotnet.Core.Initializers
{
    /// <summary>
    /// 在应用启动时同步所有权限到数据库
    /// </summary>
    public class PermissionSyncInitializer : IApplicationInitializer
    {
        private readonly IEnumerable<IPermissionProvider> _permissionProviders;
        private readonly IPermissionSyncService _permissionSyncService;
        private readonly ILogger<PermissionSyncInitializer> _logger;

        public PermissionSyncInitializer(
            IEnumerable<IPermissionProvider> permissionProviders,
            IPermissionSyncService permissionSyncService,
            ILogger<PermissionSyncInitializer> logger)
        {
            _permissionProviders = permissionProviders;
            _permissionSyncService = permissionSyncService;
            _logger = logger;
        }

        public async Task InitializeAsync()
        {
            _logger.LogInformation("开始同步所有权限...");

            // 同步所有权限提供者的权限
            foreach (var provider in _permissionProviders)
            {
                // 对于框架权限，模块名为"System"
                // 对于插件权限，模块名应该从提供者的类型中推断
                var module = "System";
                if (provider.GetType().FullName.StartsWith("Plugin"))
                {
                    // 对于插件，使用插件ID作为模块名
                    module = provider.GetType().Assembly.GetName().Name;
                }

                await _permissionSyncService.SyncPluginPermissionsAsync(provider, module);
            }

            _logger.LogInformation("所有权限同步完成。");
        }
    }
}