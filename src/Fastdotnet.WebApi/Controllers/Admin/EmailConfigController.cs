using System.Threading.Tasks;
using AutoMapper;
using Fastdotnet.Core.Controllers;
using Fastdotnet.Core.Entities.System;
using Fastdotnet.Core.Exceptions;
using Fastdotnet.Core.IService;
using Fastdotnet.Core.Models.System;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Fastdotnet.WebApi.Controllers.Admin
{
    [ApiController]
    [Route("api/admin/[controller]")]
    [Authorize]
    public class EmailConfigController : GenericDtoControllerBase<EmailConfig, CreateEmailConfigDto, UpdateEmailConfigDto, EmailConfigDto>
    {
        public EmailConfigController(IRepository<EmailConfig, string> repository, IMapper mapper) : base(repository, mapper)
        {
        }

        /// <summary>
        /// 获取唯一的邮件配置
        /// </summary>
        [HttpGet("GetConfig")]
        public async Task<EmailConfigDto> GetConfig()
        {
            var config = await _repository.GetFirstAsync(e => true);
            if (config == null)
            {
                throw new BusinessException("邮件配置不存在，请检查种子数据是否已正确初始化。");
            }
            return _mapper.Map<EmailConfigDto>(config);
        }

        /// <summary>
        /// 更新唯一的邮件配置
        /// </summary>
        [HttpPut("UpdateConfig")]
        public async Task<IActionResult> UpdateConfig([FromBody] UpdateEmailConfigDto dto)
        {
            var existingConfig = await _repository.GetFirstAsync(e => true);
            if (existingConfig == null)
            {
                throw new BusinessException("邮件配置不存在，无法更新。");
            }

            _mapper.Map(dto, existingConfig);
            await _repository.UpdateAsync(existingConfig);
            return Ok();
        }

        #region === 禁用通用接口 ===

        [NonAction]
        public override Task<System.Collections.Generic.List<EmailConfigDto>> GetAll() => throw new BusinessException("此功能对邮件配置无效。");

        [NonAction]
        public override Task<EmailConfigDto> GetById(string id) => throw new BusinessException("此功能对邮件配置无效。");

        [NonAction]
        public override Task<EmailConfigDto> Create(CreateEmailConfigDto dto) => throw new BusinessException("此功能对邮件配置无效，请使用UpdateConfig进行更新。");

        [NonAction]
        public override Task<bool> Delete(string id) => throw new BusinessException("邮件配置不允许删除。");

        #endregion
    }
}
