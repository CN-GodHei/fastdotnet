using Fastdotnet.Core.Entities.System;
using Fastdotnet.Core.Initializers;
using Fastdotnet.Core.IService;
using Fastdotnet.Plugin.Contracts;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Fastdotnet.Service.Initializers
{
    /// <summary>
    /// 在应用启动时，同步所有代码中定义的权限到数据库
    /// </summary>
    public class PermissionInitializer : IApplicationInitializer
    {
        private readonly IEnumerable<IPermissionProvider> _providers;
        private readonly IRepository<FdPermission> _permissionRepository;
        private readonly ILogger<PermissionInitializer> _logger;

        public PermissionInitializer(
            IEnumerable<IPermissionProvider> providers,
            IRepository<FdPermission> permissionRepository,
            ILogger<PermissionInitializer> logger)
        {
            _providers = providers;
            _permissionRepository = permissionRepository;
            _logger = logger;
        }

        public async Task InitializeAsync()
        {
            _logger.LogInformation("开始同步权限...");

            // 1. 从所有提供者获取代码中定义的权限
            var codePermissions = _providers.SelectMany(p => p.GetPermissions()).ToList();
            var codePermissionCodes = codePermissions.Select(p => p.Code).ToList();

            // 2. 从数据库获取已存在的权限
            var dbPermissions = await _permissionRepository.GetAllAsync();
            var dbPermissionCodes = dbPermissions.Select(p => p.Code).ToList();

            // 3. 找出需要新增的权限
            var permissionsToAdd = codePermissions
                .Where(p => !dbPermissionCodes.Contains(p.Code))
                .Select(p => new FdPermission
                {
                    Module = p.Module,
                    Code = p.Code,
                    Name = p.Name,
                    Category = p.Category,
                    Type = p.Type,
                    Description = $"由 {p.Module} 模块自动注册"
                }).ToList();

            if (permissionsToAdd.Any())
            {
                await _permissionRepository.InsertRangeAsync(permissionsToAdd);
                _logger.LogInformation($"新增 {permissionsToAdd.Count} 条权限到数据库。");
            }

            // 4. 找出需要删除的权限
            var permissionsToDelete = dbPermissions
                .Where(p => !codePermissionCodes.Contains(p.Code))
                .ToList();

            if (permissionsToDelete.Any())
            {
                var idsToDelete = permissionsToDelete.Select(p => p.Id).ToList();
                await _permissionRepository.DeleteAsync(p => idsToDelete.Contains(p.Id));
                _logger.LogInformation($"从数据库中删除 {permissionsToDelete.Count} 条过时的权限。");
            }

            _logger.LogInformation("权限同步完成。");
        }
    }
}
