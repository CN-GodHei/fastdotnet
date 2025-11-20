using AutoMapper;
using Fastdotnet.Core.Constants;
using Fastdotnet.Core.Controllers;
using Fastdotnet.Core.Entities.System;
using Fastdotnet.Core.Exceptions;
using Fastdotnet.Core.IService;
using Fastdotnet.Core.Models.System;
using Fastdotnet.Core.Service;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SqlSugar;
using System;

namespace Fastdotnet.WebApi.Controllers.System
{
    /// <summary>
    /// 代码生成相关接口
    /// </summary>
    [Route("api/[controller]")]
    public class CodeGenConfigController : GenericDtoControllerBase<FdCodeGenConfig, string, CreateFdCodeGenConfigDto, UpdateFdCodeGenConfigDto, FdCodeGenConfigDto>
    {

        public CodeGenConfigController(
            IBaseService<FdCodeGenConfig, string> service,
            IMapper mapper) : base(service, mapper)
        {
        }
    }
}