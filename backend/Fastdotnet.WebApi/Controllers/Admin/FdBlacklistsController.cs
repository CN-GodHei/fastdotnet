using Fastdotnet.Core.Dtos.Sys;
using Fastdotnet.Core.Entities.Sys;
using Fastdotnet.Core.Service.Sys;

namespace Fastdotnet.WebApi.Controllers.Admin
{
    /// <summary>
    /// 黑名单管理控制器
    /// </summary>
    [Route("api/admin/[controller]")]
    [ApiController]
    public class FdBlacklistsController : GenericDtoControllerBase<FdBlacklist,string, CreateFdBlacklistDto, UpdateFdBlacklistDto, FdBlacklistDto>
    {
        private readonly IBaseService<FdBlacklist, string> _blacklistService;
        private readonly IRateLimitCacheService _rateLimitCacheService;

        public FdBlacklistsController(
            IBaseService<FdBlacklist, string> blacklistService, 
            IMapper mapper,
            IRateLimitCacheService rateLimitCacheService) 
            : base(blacklistService, mapper)
        {
            _blacklistService = blacklistService;
            _rateLimitCacheService = rateLimitCacheService;
        }

        /// <summary>
        /// 检查值是否在黑名单中
        /// </summary>
        [HttpGet("check")]
        public async Task<ActionResult<bool>> IsBlacklisted([FromQuery] string type, [FromQuery] string value)
        {
            // 由于这是个简单的检查，我们直接在控制器中实现
            // 在实际项目中，如果逻辑复杂，还是建议使用服务层
            var blacklist = await _blacklistService.GetListAsync(x => x.Type == type && x.Value == value);
            return Ok(blacklist.Any());
        }

        /// <summary>
        /// 创建黑名单条目后的回调方法
        /// </summary>
        protected override async Task AfterCreate(FdBlacklist entity, CreateFdBlacklistDto dto)
        {
            // 添加到缓存
            await _rateLimitCacheService.AddToBlacklistAsync(entity.Type, entity.Value);
            await base.AfterCreate(entity, dto);
        }

        /// <summary>
        /// 更新黑名单条目后的回调方法
        /// </summary>
        protected override async Task AfterUpdate(FdBlacklist entity, UpdateFdBlacklistDto dto)
        {
            // 更新缓存中的黑名单
            await _rateLimitCacheService.RemoveFromBlacklistAsync(entity.Type, entity.Value);
            await _rateLimitCacheService.AddToBlacklistAsync(entity.Type, entity.Value);
            await base.AfterUpdate(entity, dto);
        }

        /// <summary>
        /// 删除黑名单条目后的回调方法
        /// </summary>
        protected override async Task AfterDelete(string id, bool result)
        {
            if (result)
            {
                // 从缓存中移除
                var entity = await _blacklistService.GetByIdAsync(id);
                if (entity != null)
                {
                    await _rateLimitCacheService.RemoveFromBlacklistAsync(entity.Type, entity.Value);
                }
            }
            await base.AfterDelete(id, result);
        }
    }
}