using Autofac.Core;
using Fastdotnet.Core.Service.Sys;
using Fastdotnet.Service.IService.Sys;
using System;
using System.Collections.Generic;
using System.Text;

namespace Fastdotnet.Service.Service.Sys
{
    public class FdDictDataService : IFdDictDataService
    {
        private readonly IBaseService<FdDictData> _baseService;

        public FdDictDataService(
            IBaseService<FdDictData> baseServic
            )
        {
            _baseService = baseServic;
        }

        public async Task<List<FdDictData>> GetUserConfig()
        {
            string[] codes = ["CODE_10_01", "CODE_10_02", "CODE_10_03"];
            return await _baseService.GetListAsync(w => codes.Contains(w.Code));
        }
    }
}
