

namespace Fastdotnet.Service.IService
{
    public interface IMenuService
    {
        Task<List<FdMenuDto>> GetUserMenusAsync(string userId, SystemCategory category);
        Task <List<FdMenuDto>> BuildMenuTree(List<FdMenu> allMenus, string? parentCode);

    }
}