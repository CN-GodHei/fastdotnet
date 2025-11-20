using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Fastdotnet.Core.Controllers;
using Fastdotnet.Core.Entities.System;
using Fastdotnet.Core.Exceptions;
using Fastdotnet.Core.IService;
using Fastdotnet.Core.Models.System;
using Fastdotnet.Core.Utils;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Fastdotnet.WebApi.Controllers.Admin
{
    [ApiController]
    [Route("api/admin/[controller]")]
    public class SystemConfigController : GenericDtoControllerBase<SystemConfig, SystemConfigDto, SystemConfigDto, SystemConfigDto>
    {
        public SystemConfigController(IBaseService<SystemConfig, string> service, IMapper mapper) : base(service, mapper)
        {
        }

        protected override async Task BeforeCreate(SystemConfig entity, SystemConfigDto dto)
        {
            // 如果前端提供了Code，检查是否重复
            if (!string.IsNullOrWhiteSpace(entity.Code))
            {
                var exists = await _service.ExistsAsync(e => e.Code == entity.Code);
                if (exists)
                {
                    throw new BusinessException($"配置编码 '{entity.Code}' 已存在，请使用不同的编码。");
                }
            }
            else // 如果前端没有提供Code，则自动生成一个
            {
                entity.Code = $"CONFIG_{SnowflakeIdGenerator.NextStrId()}";
            }

            await base.BeforeCreate(entity, dto);
        }

        protected override Task BeforeUpdate(SystemConfig existing, SystemConfig updated, SystemConfigDto dto)
        {
            // 不允许修改Code
            updated.Code = existing.Code;

            // 对系统配置，只允许修改Value
            if (existing.IsSystem)
            {
                updated.Name = existing.Name;
                updated.Description = existing.Description;
            }

            return base.BeforeUpdate(existing, updated, dto);
        }

        protected override Task BeforeDelete(SystemConfig entity)
        {
            if (entity.IsSystem)
            {
                throw new BusinessException($"系统内置配置 '{entity.Name}' 不允许删除。");
            }
            return base.BeforeDelete(entity);
        }


        // --- 以下是公开接口 ---

        /// <summary>
        /// [Public] 获取所有系统配置项（用于客户端初始化）
        /// </summary>
        /// <returns>返回一个包含所有配置的字典</returns>
        [HttpGet("public/all")]
        [AllowAnonymous] // 此接口允许匿名访问
        public async Task<Dictionary<string, object>> GetPublicConfigs()
        {
            var configs = await _service.GetAllAsync();
            return configs.ToDictionary(c => c.Code, c => c.Value);
        }
    }
}