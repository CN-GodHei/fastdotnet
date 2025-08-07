using System.Threading.Tasks;
using Fastdotnet.Core.Initializers;
using Fastdotnet.Service.IService.Admin;
using Microsoft.Extensions.Logging;

namespace Fastdotnet.Service.Initializers
{
    /// <summary>
    /// Initializes the admin user service.
    /// </summary>
    public class AdminUserInitializer : IApplicationInitializer
    {
        private readonly IAdminUserService _adminUserService;
        private readonly ILogger<AdminUserInitializer> _logger;

        public AdminUserInitializer(IAdminUserService adminUserService, ILogger<AdminUserInitializer> logger)
        {
            _adminUserService = adminUserService;
            _logger = logger;
        }

        public Task InitializeAsync()
        {
            // Here you can call methods on _adminUserService or other services to initialize them.
            // For example, you could seed the database with a default admin user.
            _logger.LogInformation("=============== Initializing Admin User Service... Success! ===============");
            
            // Example of calling a method on the service (assuming it exists)
            // await _adminUserService.SeedDefaultUserAsync();

            return Task.CompletedTask;
        }
    }
}
