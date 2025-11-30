using AutoMapper;
using Fastdotnet.Core.Controllers;
using Fastdotnet.Core.Entities.System;
using Fastdotnet.Core.IService;
using Fastdotnet.Core.Models.System;
using Microsoft.AspNetCore.Mvc;

namespace Fastdotnet.WebApi.Controllers.System
{
    /// <summary>
    /// FdDictData 控制器
    /// </summary>
    [Route("api/[controller]")]
    public class FdDictDataController : GenericDtoControllerBase<FdDictData, string, CreateFdDictDataDto, UpdateFdDictDataDto, FdDictDataDto>
    {
        public FdDictDataController(
            //IFdDictDataService fddictdataService,
            IBaseService<FdDictData, string> service,
            IMapper mapper) : base(service, mapper)
        {

        }
    }
}
