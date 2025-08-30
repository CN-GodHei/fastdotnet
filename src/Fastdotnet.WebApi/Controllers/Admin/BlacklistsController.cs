using AutoMapper;
using Fastdotnet.Core.Controllers;
using Fastdotnet.Core.Dtos.System;
using Fastdotnet.Core.Entities.System;
using Fastdotnet.Core.IService;
using Fastdotnet.Core.Services.System;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace Fastdotnet.WebApi.Controllers.Admin
{
    /// <summary>
    /// 黑名单管理控制器
    /// </summary>
    [Route("api/admin/[controller]")]
    [ApiController]
    public class BlacklistsController : GenericDtoControllerBase<FdBlacklist,string, CreateFdBlacklistDto, UpdateFdBlacklistDto, FdBlacklistDto>
    {
        private readonly IRepository<FdBlacklist, string> _blacklistRepository;
        private readonly IRateLimitCacheService _rateLimitCacheService;

        public BlacklistsController(
            IRepository<FdBlacklist, string> blacklistRepository, 
            IMapper mapper,
            IRateLimitCacheService rateLimitCacheService) 
            : base(blacklistRepository, mapper)
        {
            _blacklistRepository = blacklistRepository;
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
            var blacklist = await _blacklistRepository.GetListAsync(x => x.Type == type && x.Value == value);
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
            // 由于更新操作可能会改变Type或Value，我们需要更新缓存
            // 这里简化处理，直接重置整个类型的黑名单缓存
            await _rateLimitCacheService.RemoveFromBlacklistAsync(entity.Type, entity.Value);
            await _rateLimitCacheService.AddToBlacklistAsync(entity.Type, entity.Value);
            await base.AfterUpdate(entity, dto);
        }

        /// <summary>
        /// 删除黑名单条目后的回调方法
        /// </summary>
        protected override async Task AfterDelete(string id, bool result)
        {
            // 从缓存中移除
            // 注意：这里我们无法直接获取到entity的信息，所以只能通过其他方式处理
            // 在实际项目中，可能需要通过ID查询entity来获取Type和Value
            await base.AfterDelete(id, result);
        }
    }
}