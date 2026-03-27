using Fastdotnet.Core.IService.Sys;
using Fastdotnet.Core.Service.Sys;
using System;
using System.Collections.Generic;
using System.Text;

namespace Fastdotnet.Service.Service.Sys
{
    public class FdNationalStandardItemService : BaseService<FdNationalStandardItem>
    , IFdNationalStandardItemService
    {
        public FdNationalStandardItemService(IRepository<FdNationalStandardItem> repository) : base(repository)
        {
        }
    }
}
