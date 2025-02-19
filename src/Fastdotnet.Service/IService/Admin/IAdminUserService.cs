using System.Threading.Tasks;
using Fastdotnet.Core.Models.Admin;

namespace Fastdotnet.Service.IService.Admin
{
    /// <summary>
    /// 管理员服务接口
    /// </summary>
    public interface IAdminUserService
    {
        /// <summary>
        /// 管理员登录
        /// </summary>
        /// <param name="username">用户名</param>
        /// <param name="password">密码</param>
        /// <returns>登录成功返回token，失败返回null</returns>
        Task<string> LoginAsync(string username, string password);

        /// <summary>
        /// 获取管理员信息
        /// </summary>
        /// <param name="id">管理员ID</param>
        /// <returns>管理员信息</returns>
        Task<FdAdminUser> GetByIdAsync(long id);

        /// <summary>
        /// 创建管理员
        /// </summary>
        /// <param name="adminUser">管理员信息</param>
        /// <returns>创建成功返回true</returns>
        Task<bool> CreateAsync(FdAdminUser adminUser);

        /// <summary>
        /// 更新管理员信息
        /// </summary>
        /// <param name="adminUser">管理员信息</param>
        /// <returns>更新成功返回true</returns>
        Task<bool> UpdateAsync(FdAdminUser adminUser);

        /// <summary>
        /// 删除管理员
        /// </summary>
        /// <param name="id">管理员ID</param>
        /// <returns>删除成功返回true</returns>
        Task<bool> DeleteAsync(long id);

        /// <summary>
        /// 修改密码
        /// </summary>
        /// <param name="id">管理员ID</param>
        /// <param name="oldPassword">旧密码</param>
        /// <param name="newPassword">新密码</param>
        /// <returns>修改成功返回true</returns>
        Task<bool> ChangePasswordAsync(long id, string oldPassword, string newPassword);
    }
}