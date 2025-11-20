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
        public EmailConfigController(IBaseService<EmailConfig, string> service, IMapper mapper) : base(service, mapper)
        {
        }

        /// <summary>
        /// 获取唯一的邮件配置
        /// </summary>
        [HttpGet("GetConfig")]
        public async Task<EmailConfigDto> GetConfig()
        {
            var config = await _service.GetFirstAsync(e => true);
            if (config == null)
            {
                throw new BusinessException("邮件配置不存在，请检查种子数据是否已正确初始化。");
            }
            return _mapper.Map<EmailConfigDto>(config);
        }

        /// <summary>
        /// 更新唯一的邮件配置
        /// </summary>
        [HttpPost("UpdateConfig")]
        public async Task<EmailConfigDto> UpdateConfig(UpdateEmailConfigDto dto)
        {
            var existing = await _service.GetFirstAsync(e => true);
            if (existing == null)
            {
                throw new BusinessException("邮件配置不存在，请检查种子数据是否已正确初始化。");
            }

            _mapper.Map(dto, existing);
            var result = await _service.UpdateAsync(existing);
            return _mapper.Map<EmailConfigDto>(result);
        }

        #region === 禁用通用接口 ===

        [NonAction]
        public override Task<List<EmailConfigDto>> GetAll(CancellationToken cancellationToken = default) => throw new BusinessException("此功能对邮件配置无效。");

        [NonAction]
        public override Task<EmailConfigDto> GetById(string id, CancellationToken cancellationToken = default) => throw new BusinessException("此功能对邮件配置无效。");

        [NonAction]
        public override Task<EmailConfigDto> Create(CreateEmailConfigDto dto) => throw new BusinessException("此功能对邮件配置无效，请使用UpdateConfig进行更新。");

        [NonAction]
        public override Task<bool> Delete(string id) => throw new BusinessException("邮件配置不允许删除。");

        #endregion
    }
}
