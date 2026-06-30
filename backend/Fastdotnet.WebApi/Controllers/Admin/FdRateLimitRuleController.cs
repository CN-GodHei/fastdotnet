using Fastdotnet.Core.Dtos.Sys;
using Fastdotnet.Core.Entities.Sys;
using Fastdotnet.Core.Service.Sys;

namespace Fastdotnet.WebApi.Controllers.Admin
{
    /// <summary>
    /// 限流规则管理控制器
    /// </summary>
    [Route("api/admin/[controller]")]
    [ApiController]
    public class FdRateLimitRuleController : GenericDtoControllerBase<FdRateLimitRule, CreateFdRateLimitRuleDto, UpdateFdRateLimitRuleDto, FdRateLimitRuleDto>
    {
        private readonly IBaseService<FdRateLimitRule, string> _rateLimitRuleService;
        private readonly IRateLimitCacheService _rateLimitCacheService;

        public FdRateLimitRuleController(
            IBaseService<FdRateLimitRule, string> rateLimitRuleService,
            IRateLimitCacheService rateLimitCacheService) 
            : base(rateLimitRuleService)
        {
            _rateLimitRuleService = rateLimitRuleService;
            _rateLimitCacheService = rateLimitCacheService;
        }

        /// <summary>
        /// 根据类型和键获取限流规则
        /// </summary>
        [HttpGet("by-type-and-key")]
        public async Task<ActionResult<FdRateLimitRuleDto>> GetByTypeAndKey([FromQuery] string type, [FromQuery] string key)
        {
            var result = await _rateLimitRuleService.GetFirstAsync(x => x.Type == type && x.Key == key);
            if (result == null)
                return NotFound();

            return Ok(result);
        }

        /// <summary>
        /// 检查是否触发限流
        /// </summary>
        [HttpGet("check")]
        public async Task<ActionResult<bool>> IsRateLimited([FromQuery] string type, [FromQuery] string key)
        {
            var rule = await _rateLimitRuleService.GetFirstAsync(x => x.Type == type && x.Key == key);
            if (rule == null)
                return Ok(false);

            return Ok(true);
        }

        /// <summary>
        /// 创建限流规则后的回调方法
        /// </summary>
        protected override async Task AfterCreate(FdRateLimitRule entity, CreateFdRateLimitRuleDto dto)
        {
            var ruleDto = entity.Adapt<FdRateLimitRuleDto>();
            await _rateLimitCacheService.SetRateLimitRuleAsync(entity.Type, entity.Key, ruleDto);
            await base.AfterCreate(entity, dto);
        }

        /// <summary>
        /// 更新限流规则后的回调方法
        /// </summary>
        protected override async Task AfterUpdate(FdRateLimitRule entity, UpdateFdRateLimitRuleDto dto)
        {
            var ruleDto = entity.Adapt<FdRateLimitRuleDto>();
            await _rateLimitCacheService.SetRateLimitRuleAsync(entity.Type, entity.Key, ruleDto);
            await base.AfterUpdate(entity, dto);
        }

        /// <summary>
        /// 删除限流规则后的回调方法
        /// </summary>
        protected override async Task AfterDelete(string id, bool result)
        {
            await base.AfterDelete(id, result);
        }
    }
}
