using Fastdotnet.Core.Models;
using Fastdotnet.Core.Models.Admin.Users;
using System.Threading.Tasks;

namespace Fastdotnet.Service.IService.Admin
{
    /// <summary>
    /// 后台管理员服务接口
    /// </summary>
    public interface IAdminUserService
    {
        /// <summary>
        /// 根据ID获取管理员用户
        /// </summary>
        Task<AdminUserDto?> GetAsync(string id);

        /// <summary>
        /// 分页获取管理员用户
        /// </summary>
        Task<PageResult<AdminUserDto>> GetPageAsync(PageQueryDto query);

        /// <summary>
        /// 创建新管理员
        /// </summary>
        /// <returns>新用户的ID</returns>
        Task<string> CreateAsync(CreateAdminUserDto dto);

        /// <summary>
        /// 更新管理员信息
        /// </summary>
        Task UpdateAsync(string id, UpdateAdminUserDto dto);

        /// <summary>
        /// 删除管理员
        /// </summary>
        Task DeleteAsync(string id);

        /// <summary>
        /// (由管理员)重置用户密码
        /// </summary>
        Task ResetPasswordAsync(string id, string newPassword);
        
        /// <summary>
        /// 判断指定用户是否为超级管理员
        /// </summary>
        /// <param name="userId">用户ID</param>
        /// <returns>是否为超级管理员</returns>
        Task<bool> IsSuperAdminAsync(string userId);
    }
}