using Fastdotnet.Core.Dtos.Sys;
using Fastdotnet.Core.Entities.Sys;

namespace Fastdotnet.WebApi.Controllers.Admin
{
    [ApiController]
    [Route("api/admin/[controller]")]
    [Authorize]
    public class FdEmailConfigController : GenericDtoControllerBase<EmailConfig, FdCreateEmailConfigDto, FdUpdateEmailConfigDto, FdEmailConfigDto>
    {
        private readonly IEmailService _emailService;
        public FdEmailConfigController(IBaseService<EmailConfig, string> service, IMapper mapper, IEmailService emailService) : base(service, mapper)
        {
            _emailService = emailService;
        }

        /// <summary>
        /// 获取唯一的邮件配置
        /// </summary>
        [HttpGet("GetConfig")]
        public async Task<FdEmailConfigDto> GetConfig()
        {
            var config = await _service.GetFirstAsync(e => true);
            if (config == null)
            {
                throw new BusinessException("邮件配置不存在，请检查种子数据是否已正确初始化。");
            }
            return _mapper.Map<FdEmailConfigDto>(config);
        }

        /// <summary>
        /// 更新唯一的邮件配置
        /// </summary>
        [HttpPost("UpdateConfig")]
        public async Task<FdEmailConfigDto> UpdateConfig(FdUpdateEmailConfigDto dto)
        {
            var existing = await _service.GetFirstAsync(e => true);
            if (existing == null)
            {
                throw new BusinessException("邮件配置不存在，请检查种子数据是否已正确初始化。");
            }

            _mapper.Map(dto, existing);
            var result = await _service.UpdateAsync(existing);
            return _mapper.Map<FdEmailConfigDto>(result);
        }

        /// <summary>
        /// 测试发送邮件
        /// </summary>
        [HttpPost("TestSend")]
        public async Task<bool> TestSend(TestSendEmailDto dto)
        {
            // 获取当前邮件配置
            var config = await _service.GetFirstAsync(e => true);
            if (config == null)
            {
                throw new BusinessException("邮件配置不存在，请先配置邮件服务。");
            }

            try
            {
                // 使用配置的邮件服务发送测试邮件
                await _emailService.SendEmailAsync(dto.ToEmail, dto.Subject, dto.Body);
                return true;
            }
            catch (Exception ex)
            {
                throw new BusinessException($"邮件发送失败: {ex.Message}");
            }
        }



        #region === 禁用通用接口 ===

        [NonAction]
        public override Task<List<FdEmailConfigDto>> GetAll(CancellationToken cancellationToken = default) => throw new BusinessException("此功能对邮件配置无效。");

        [NonAction]
        public override Task<FdEmailConfigDto> GetById(string id, CancellationToken cancellationToken = default) => throw new BusinessException("此功能对邮件配置无效。");

        [NonAction]
        public override Task<FdEmailConfigDto> Create(FdCreateEmailConfigDto dto) => throw new BusinessException("此功能对邮件配置无效，请使用UpdateConfig进行更新。");

        [NonAction]
        public override Task<bool> Delete(string id) => throw new BusinessException("邮件配置不允许删除。");

        #endregion
    }
}
