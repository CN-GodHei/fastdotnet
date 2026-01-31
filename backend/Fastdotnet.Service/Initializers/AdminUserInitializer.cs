using Fastdotnet.Core.Entities.Admin;
using Fastdotnet.Core.Entities.System;
using Fastdotnet.Core.Enum;
using Fastdotnet.Core.Initializers;
using Fastdotnet.Core.IService;
using Fastdotnet.Service.IService.Admin;
using Microsoft.Extensions.Logging;
using System.Linq;
using System.Threading.Tasks;

namespace Fastdotnet.Service.Initializers
{
    public class AdminUserInitializer : IApplicationInitializer
    {
        private readonly IAdminUserService _adminUserService;
        //private readonly ILogger<AdminUserInitializer> _logger;
        private readonly IRepository<FdRole> _roleRepository;
        private readonly IRepository<FdAdminUser> _adminUserRepository;
        private readonly IRepository<FdAdminUserRole> _adminUserRoleRepository;

        public AdminUserInitializer(
            IAdminUserService adminUserService,
            //ILogger<AdminUserInitializer> logger, 
            IRepository<FdRole> roleRepository,
            IRepository<FdAdminUser> adminUserRepository,
            IRepository<FdAdminUserRole> adminUserRoleRepository)
        {
            _adminUserService = adminUserService;
            //_logger = logger;
            _roleRepository = roleRepository;
            _adminUserRepository = adminUserRepository;
            _adminUserRoleRepository = adminUserRoleRepository;
        }

        public async Task InitializeAsync()
        {
            //_logger.LogInformation("Start: Initializing Admin User and Super Admin Role...");

            // 1. 确保超级管理员角色存在
            const string superAdminRoleCode = "SUPER_ADMIN";
            var superAdminRole = await _roleRepository.GetFirstAsync(r => r.Code == superAdminRoleCode);

            if (superAdminRole == null)
            {
                //_logger.LogInformation($"Role '{superAdminRoleCode}' not found, creating it...");
                superAdminRole = new FdRole
                {
                    Name = "超级角色",
                    Code = superAdminRoleCode,
                    IsSystem = true,
                    Description = "拥有系统所有权限",
                    Belong = SystemCategory.Admin
                };
                await _roleRepository.InsertAsync(superAdminRole);
                //_logger.LogInformation("Super Admin role created successfully.");
            }

            // 2. 确保默认超级管理员用户存在
            var SuperAdminUser = await _adminUserRepository.GetFirstAsync(u => u.Username == "superadmin");
            if (SuperAdminUser == null)
            {
                //_logger.LogInformation("Default admin user not found, creating it...");
                // 在真实项目中，初始密码应从安全配置中读取
                //await _adminUserService.CreateAsync(new Core.Models.Admin.Users.CreateFdAdminUserDto { Username = "superadmin", Password = "123456",Name= "超级管理员" });
                await _adminUserRepository.InsertAsync(new FdAdminUser { Id= "11438163155878918", Username = "superadmin", Password = "123456",Name= "超级管理员" });
                SuperAdminUser = await _adminUserRepository.GetFirstAsync(u => u.Username == "superadmin");
                //_logger.LogInformation("Default admin user created successfully.");
            }

            // 3. 确保管理员用户拥有超级管理员角色
            if (SuperAdminUser != null && superAdminRole != null)
            {
                bool hasSuperAdminRole = await _adminUserRoleRepository.ExistsAsync(ur => ur.AdminUserId == SuperAdminUser.Id && ur.RoleId == superAdminRole.Id);
                if (!hasSuperAdminRole)
                {
                    //_logger.LogInformation("Assigning SUPER_ADMIN role to admin user...");
                    await _adminUserRoleRepository.InsertAsync(new FdAdminUserRole { AdminUserId = SuperAdminUser.Id, RoleId = superAdminRole.Id });
                    //_logger.LogInformation("Role assignment successful.");
                }
            }

            //_logger.LogInformation("Finish: Admin User and Super Admin Role initialization complete.");
        }
    }
}