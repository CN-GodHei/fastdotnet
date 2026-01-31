using Fastdotnet.Core.Dtos.System;
using Fastdotnet.Core.Entities.System;
using Fastdotnet.Core.Enum;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Fastdotnet.Service.IService
{
    public interface IMenuService
    {
        Task<List<FdMenuDto>> GetUserMenusAsync(string userId, SystemCategory category);
        Task <List<FdMenuDto>> BuildMenuTree(List<FdMenu> allMenus, string? parentCode);

    }
}