using Fastdotnet.Core.Entities.System;
using Fastdotnet.Core.Models.System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Fastdotnet.Service.IService
{
    public interface IMenuService
    {
        Task<List<MenuDto>> GetUserMenusAsync(string userId, string category);
        Task <List<MenuDto>> BuildMenuTree(List<FdMenu> allMenus, string? parentCode);

    }
}