using AutoMapper;
using Fastdotnet.Core.Constants;
using Fastdotnet.Core.Controllers;
using Fastdotnet.Core.Entities.System;
using Fastdotnet.Core.Exceptions;
using Fastdotnet.Core.IService;
using Fastdotnet.Core.Models.System;
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
        private readonly ICodeGenConfigService _codeGenConfigService;
        public CodeGenConfigController(
            IBaseService<FdCodeGenConfig, string> service,
            ICodeGenConfigService codeGenConfigService,
            IMapper mapper) : base(service, mapper)
        {
            _codeGenConfigService = codeGenConfigService;
        }

        protected override async Task BeforeUpdateMany(List<FdCodeGenConfig> existingEntity, List<UpdateFdCodeGenConfigDto> dto)
        {
            foreach (var item in dto)
            {
                if (item.EffectType == "ForeignKey")
                {
                    var TableCol = await _codeGenConfigService.GetTableColumnListAsync(item.FkTableName);
                    var col = TableCol.Where(x => x.ColumnName == item.FkLinkColumnName).FirstOrDefault();
                    item.FkColumnNetType = _codeGenConfigService.GetEffectTypeByColumnName(col.ColumnName, col.DataType);
                }
            }

            await base.BeforeUpdateMany(existingEntity, dto);
        }
    }
}