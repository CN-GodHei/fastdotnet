using Fastdotnet.Core.Dtos.Sys;
using Fastdotnet.Core.Entities.Sys;

namespace Fastdotnet.Service.IService.Sys
{
    /// <summary>
    /// 角色管理服务接口
    /// </summary>
    public interface IFdRoleService
    {
        /// <summary>
        /// 检查角色是否已分配给用户
        /// </summary>
        Task<bool> IsRoleAssignedToUsersAsync(string roleId, SystemCategory belong);

        /// <summary>
        /// 获取角色的权限ID列表
        /// </summary>
        Task<List<string>> GetRolePermissionIdsAsync(string roleId);

        /// <summary>
        /// 分配角色权限
        /// </summary>
        Task AssignPermissionsAsync(string roleId, AssignPermissionsDto dto);

        /// <summary>
        /// 保存角色的菜单和按钮权限（内含事务）
        /// </summary>
        Task SaveMenuButtonPermissionsAsync(string roleId, List<MenuBtnRe> menuBtnList);

        /// <summary>
        /// 在删除角色前的检查
        /// </summary>
        Task ValidateBeforeDeleteAsync(FdRole entity);
    }
}
