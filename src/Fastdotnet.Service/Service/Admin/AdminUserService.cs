using Fastdotnet.Core.Models.Admin;
using Fastdotnet.Service.IService.Admin;

namespace Fastdotnet.Service
{
    public class AdminUserService : IAdminUserService
    {
        public AdminUserService()
        {
            // TODO: 添加必要的依赖注入
        }

        public Task<bool> ChangePasswordAsync(long id, string oldPassword, string newPassword)
        {
            throw new NotImplementedException();
        }

        public Task<bool> CreateAsync(FdAdminUser adminUser)
        {
            throw new NotImplementedException();
        }

        public Task<bool> DeleteAsync(long id)
        {
            throw new NotImplementedException();
        }

        public Task<FdAdminUser> GetByIdAsync(long id)
        {
            throw new NotImplementedException();
        }

        public Task<string> LoginAsync(string username, string password)
        {
            throw new NotImplementedException();
        }

        public Task<bool> UpdateAsync(FdAdminUser adminUser)
        {
            throw new NotImplementedException();
        }

        // TODO: 实现IAdminUserService接口的方法
    }
}