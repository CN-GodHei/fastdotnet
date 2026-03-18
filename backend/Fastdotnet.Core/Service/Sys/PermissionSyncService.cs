using Fastdotnet.Core.Entities.Sys;

namespace Fastdotnet.Core.Service.Sys
{
    /// <summary>
    /// 权限同步服务
    /// </summary>
    public class PermissionSyncService : IPermissionSyncService
    {
        private readonly IRepository<FdPermission> _permissionRepository;
        //private readonly ILogger<PermissionSyncService> _logger;

        public PermissionSyncService(
            IRepository<FdPermission> permissionRepository,
            ILogger<PermissionSyncService> logger)
        {
            _permissionRepository = permissionRepository;
            //_logger = logger;
        }

        /// <summary>
        /// 同步单个插件的权限
        /// </summary>
        /// <param name="provider">权限提供者</param>
        /// <param name="module">模块名称</param>
        /// <returns></returns>
        public async Task SyncPluginPermissionsAsync(IPermissionProvider provider, string module)
        {
            //_logger.LogInformation($"开始同步模块 {module} 的权限...");

            // 1. 从提供者获取代码中定义的权限
            var codePermissions = provider.GetPermissions().ToList();
            var codePermissionCodes = codePermissions.Select(p => p.Code).ToList();

            // 2. 从数据库获取该模块已存在的权限
            var dbPermissions = await _permissionRepository.GetListAsync(p => p.Module == module);
            var dbPermissionCodes = dbPermissions.Select(p => p.Code).ToList();

            // 3. 找出需要新增的权限
            var permissionsToAdd = codePermissions
                .Where(p => !dbPermissionCodes.Contains(p.Code))
                .Select(p => new FdPermission
                {
                    Module = module,
                    Code = p.Code,
                    Name = p.Name,
                    Category = p.Category,
                    Type = System.Enum.Parse<PermissionType>(p.Type, true),
                    Description = $"由 {p.Module} 模块自动注册"
                }).ToList();

            if (permissionsToAdd.Any())
            {
                await _permissionRepository.InsertRangeAsync(permissionsToAdd);
                //_logger.LogInformation($"为模块 {module} 新增 {permissionsToAdd.Count} 条权限到数据库。");
            }

            // 4. 找出需要删除的权限（该模块在代码中不存在但在数据库中存在的权限）
            var permissionsToDelete = dbPermissions
                .Where(p => !codePermissionCodes.Contains(p.Code))
                .ToList();

            if (permissionsToDelete.Any())
            {
                var idsToDelete = permissionsToDelete.Select(p => p.Id).ToList();
                await _permissionRepository.DeleteAsync(p => idsToDelete.Contains(p.Id));
                //_logger.LogInformation($"从数据库中删除模块 {module} 的 {permissionsToDelete.Count} 条过时的权限。");
            }

            //_logger.LogInformation($"模块 {module} 的权限同步完成。");
        }
    }
}