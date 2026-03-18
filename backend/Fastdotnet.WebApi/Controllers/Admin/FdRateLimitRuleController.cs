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
    public class FdRatelimitRuleController : GenericDtoControllerBase<FdRateLimitRule, CreateFdRateLimitRuleDto, UpdateFdRateLimitRuleDto, FdRateLimitRuleDto>
    {
        private readonly IBaseService<FdRateLimitRule, string> _rateLimitRuleService;
        private readonly IRateLimitCacheService _rateLimitCacheService;

        public FdRatelimitRuleController(
            IBaseService<FdRateLimitRule, string> rateLimitRuleService, 
            IMapper mapper,
            IRateLimitCacheService rateLimitCacheService) 
            : base(rateLimitRuleService, mapper)
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
        /// <remarks>
        /// 注意：这个方法仅作演示用途。实际的限流检查应该在中间件中完成，
        /// 而不是通过API调用。这里只是为了展示如何在控制器中使用仓储。
        /// </remarks>
        [HttpGet("check")]
        public async Task<ActionResult<bool>> IsRateLimited([FromQuery] string type, [FromQuery] string key)
        {
            // 由于这是个简单的检查，我们直接在控制器中实现
            // 在实际项目中，如果逻辑复杂，还是建议使用服务层
            var rule = await _rateLimitRuleService.GetFirstAsync(x => x.Type == type && x.Key == key);
            if (rule == null)
                return Ok(false);

            // 这里只是一个简化的示例，实际的限流逻辑会在中间件中实现
            return Ok(true);
        }

        /// <summary>
        /// 创建限流规则后的回调方法
        /// </summary>
        protected override async Task AfterCreate(FdRateLimitRule entity, CreateFdRateLimitRuleDto dto)
        {
            // 添加到缓存
            var ruleDto = _mapper.Map<FdRateLimitRuleDto>(entity);
            await _rateLimitCacheService.SetRateLimitRuleAsync(entity.Type, entity.Key, ruleDto);
            await base.AfterCreate(entity, dto);
        }

        /// <summary>
        /// 更新限流规则后的回调方法
        /// </summary>
        protected override async Task AfterUpdate(FdRateLimitRule entity, UpdateFdRateLimitRuleDto dto)
        {
            // 更新缓存
            var ruleDto = _mapper.Map<FdRateLimitRuleDto>(entity);
            await _rateLimitCacheService.SetRateLimitRuleAsync(entity.Type, entity.Key, ruleDto);
            await base.AfterUpdate(entity, dto);
        }

        /// <summary>
        /// 删除限流规则后的回调方法
        /// </summary>
        protected override async Task AfterDelete(string id, bool result)
        {
            // 从缓存中移除
            // 注意：这里我们无法直接获取到entity的信息，所以只能通过其他方式处理
            // 在实际项目中，可能需要通过ID查询entity来获取Type和Key
            await base.AfterDelete(id, result);
        }
    }
}