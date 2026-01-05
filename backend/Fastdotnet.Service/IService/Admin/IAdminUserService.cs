using Fastdotnet.Core.Dtos;
using Fastdotnet.Core.Dtos.Admin.Users;
using Fastdotnet.Core.Entities.Admin;
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
        Task<FdAdminUserDto?> GetAsync(string id);

        /// <summary>
        /// 分页获取管理员用户
        /// </summary>
        Task<PageResult<FdAdminUserDto>> GetPageAsync(PageQueryDto query);

        /// <summary>
        /// 创建新管理员
        /// </summary>
        /// <returns>新用户的ID</returns>
        Task<string> CreateAsync(CreateFdAdminUserDto dto);

        /// <summary>
        /// 更新管理员信息
        /// </summary>
        Task UpdateAsync(string id, UpdateFdAdminUserDto dto);

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
        
        /// <summary>
        /// 获取用户角色关联信息
        /// </summary>
        /// <param name="userId">用户ID</param>
        /// <returns>用户角色关联列表</returns>
        Task<List<FdAdminUserRole>> GetUserRoleRelationsAsync(string userId);
        
        /// <summary>
        /// 获取用户按钮权限
        /// </summary>
        /// <param name="userId">用户ID</param>
        /// <returns>按钮权限列表</returns>
        Task<List<string>> GetUserButtonPermissionsAsync(string userId);
    }
}