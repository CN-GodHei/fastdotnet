using Fastdotnet.Core.Entities.Admin;
using Fastdotnet.Core.Entities.App;
using Fastdotnet.Core.Entities.System;
using Fastdotnet.Core.IService;
using Fastdotnet.Service.IService;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Fastdotnet.Service.Service
{
    public class PermissionService : IPermissionService
    {
        private readonly IRepository<FdAdminUserRole> _adminUserRoleRepository;
        private readonly IRepository<FdAppUserRole> _appUserRoleRepository;
        private readonly IRepository<FdRolePermission> _rolePermissionRepository;
        private readonly IRepository<FdPermission> _permissionRepository;

        public PermissionService(
            IRepository<FdAdminUserRole> adminUserRoleRepository,
            IRepository<FdAppUserRole> appUserRoleRepository,
            IRepository<FdRolePermission> rolePermissionRepository,
            IRepository<FdPermission> permissionRepository)
        {
            _adminUserRoleRepository = adminUserRoleRepository;
            _appUserRoleRepository = appUserRoleRepository;
            _rolePermissionRepository = rolePermissionRepository;
            _permissionRepository = permissionRepository;
        }

        public async Task<List<string>> GetUserPermissionsAsync(long userId, string userCategory)
        {
            List<long> roleIds;
            if (userCategory == "Admin")
            {
                var userRoles = await _adminUserRoleRepository.GetListAsync(ur => ur.AdminUserId == userId);
                roleIds = userRoles.Select(ur => ur.RoleId).ToList();
            }
            else
            {
                var userRoles = await _appUserRoleRepository.GetListAsync(ur => ur.AppUserId == userId);
                roleIds = userRoles.Select(ur => ur.RoleId).ToList();
            }

            if (!roleIds.Any())
            {
                return new List<string>();
            }

            var rolePermissions = await _rolePermissionRepository.GetListAsync(rp => roleIds.Contains(rp.RoleId));
            var permissionIds = rolePermissions.Select(rp => rp.PermissionId).Distinct().ToList();

            if (!permissionIds.Any())
            {
                return new List<string>();
            }

            var permissions = await _permissionRepository.GetListAsync(p => permissionIds.Contains(p.Id));
            return permissions.Select(p => p.Code).ToList();
        }
    }
}
