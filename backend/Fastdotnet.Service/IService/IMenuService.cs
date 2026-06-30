using Fastdotnet.Core.Dtos.Sys;
using Fastdotnet.Core.Entities.Sys;

namespace Fastdotnet.Service.IService
{
    public interface IMenuService
    {
        Task<List<FdMenuDto>> GetUserMenusAsync(string userId, SystemCategory category);
        Task<List<FdMenuDto>> BuildMenuTree(List<FdMenu> allMenus, string? parentCode);

        /// <summary>
        /// 根据菜单编码列表获取菜单按钮
        /// </summary>
        Task<List<FdMenuButton>> GetMenuButtonsByCodesAsync(List<string> menuCodes, CancellationToken cancellationToken = default);

        /// <summary>
        /// 获取角色关联的菜单ID列表
        /// </summary>
        Task<List<string>> GetRoleMenuIdsAsync(string roleId);

        /// <summary>
        /// 获取角色关联的菜单按钮ID列表
        /// </summary>
        Task<List<string>> GetRoleMenuButtonIdsAsync(string roleId);
    }
}