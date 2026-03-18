namespace Fastdotnet.Service.IService.App
{
    /// <summary>
    /// app端人员服务
    /// </summary>
    public interface IAppUserService
    {
        
        /// <summary>
        /// 获取用户角色关联信息
        /// </summary>
        /// <param name="userId">用户ID</param>
        /// <returns>用户角色关联列表</returns>
        Task<List<FdAppUserRole>> GetUserRoleRelationsAsync(string userId);
        
        /// <summary>
        /// 获取用户按钮权限
        /// </summary>
        /// <param name="userId">用户ID</param>
        /// <returns>按钮权限列表</returns>
        Task<List<string>> GetUserButtonPermissionsAsync(string userId);
    }
}