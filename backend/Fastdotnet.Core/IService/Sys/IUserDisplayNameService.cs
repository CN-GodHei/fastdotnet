using Fastdotnet.Core.Enum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fastdotnet.Core.IService.Sys
{
    public interface IUserDisplayNameService
    {
        Task<Dictionary<string, string>> GetDisplayNamesAsync(IEnumerable<string> userIds, SystemCategory systemCategory);
    }
}
