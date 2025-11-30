using AutoMapper;
using Fastdotnet.Core.Controllers;
using Fastdotnet.Core.Entities.System;
using Fastdotnet.Core.IService;
using Fastdotnet.Core.Models.System;
using Microsoft.AspNetCore.Mvc;

namespace Fastdotnet.WebApi.Controllers.System
{
    /// <summary>
    /// FdDictType 控制器
    /// </summary>
    [Route("api/[controller]")]
    public class FdDictTypeController : GenericDtoControllerBase<FdDictType, string, CreateFdDictTypeDto, UpdateFdDictTypeDto, FdDictTypeDto>
    {
        public FdDictTypeController(
            //IFdDictTypeService fddicttypeService,
            IBaseService<FdDictType, string> service,
            IMapper mapper) : base(service, mapper)
        {

        }
    }
}
