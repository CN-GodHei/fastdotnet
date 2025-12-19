using Fastdotnet.Core.Enum;

namespace Fastdotnet.WebApi.Controllers.Admin
{
    [ApiController]
    [Route("api/admin/[controller]")]
    public class FdSystemInfoConfigController : GenericDtoControllerBase<SystemInfoConfig, CreateFdSystemInfoConfigDto, UpdateFdSystemInfoConfigDto, FdSystemInfoConfigDto>
    {
        private readonly ICurrentUser _currentUser;
        public FdSystemInfoConfigController(IBaseService<SystemInfoConfig, string> service, IMapper mapper, ICurrentUser currentUser) : base(service, mapper)
        {
            _currentUser = currentUser;
        }

        protected override async Task BeforeCreate(SystemInfoConfig entity, CreateFdSystemInfoConfigDto dto)
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

        protected override Task BeforeUpdate(SystemInfoConfig existing, SystemInfoConfig updated, UpdateFdSystemInfoConfigDto dto)
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

        protected override Task BeforeDelete(SystemInfoConfig entity)
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
        [ApiUsageScope(ApiUsageScopeEnum.Both)]
        public async Task<Dictionary<string, object>> GetPublicConfigs()
        {
            if (string.IsNullOrEmpty(_currentUser.UserType))
            {
                return null;
            }
            var configs = await _service.GetListAsync(x=>x.Belong==EnumHelper.ParseEnum<SystemCategory>(_currentUser.UserType));
            return configs.ToDictionary(c => c.Code, c => c.Value);
        }
    }
}