using Fastdotnet.Core.Entities.System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Fastdotnet.Service.IService
{
    public interface IMenuService
    {
        Task<List<FdMenu>> GetUserMenusAsync(long userId, string category);
    }
}