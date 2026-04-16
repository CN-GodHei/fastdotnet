using System;
using System.Collections.Generic;
using System.Text;

namespace Fastdotnet.Service.IService.Sys
{
    public interface IFdDictDataService 
    {
        public Task<List<FdDictData>> GetUserConfig();
    }
}
