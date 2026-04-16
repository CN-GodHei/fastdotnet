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
        private readonly IRepository<FdDictData, string> _dataRepository;
        private readonly IBaseService<FdDictData> _baseService;

        public FdDictDataService(
            IBaseService<FdDictData> baseServic,
            IRepository<FdDictData, string> dataRepository
            )
        {
            _baseService = baseServic;
            _dataRepository = dataRepository;
        }

        public async Task<List<FdDictData>> GetUserConfig()
        {
            string[] codes = ["CODE_10_01", "CODE_10_02", "CODE_10_03"];
            return await _baseService.GetListAsync(w => codes.Contains(w.Code));
        }
    }
}
