using System.Collections.Generic;
using System.Threading.Tasks;

namespace Fastdotnet.Service.IService
{
    public interface IPermissionService
    {
        Task<List<string>> GetUserPermissionsAsync(long userId, string userCategory);
    }
}
