
using Fastdotnet.Core.Dtos.Sys;
using Fastdotnet.Core.Entities.Sys;

namespace Fastdotnet.Service.Mappings
{
    /// <summary>
    /// 限流规则映射配置
    /// </summary>
    public class FdRateLimitRuleProfile : Profile
    {
        public FdRateLimitRuleProfile()
        {
            CreateMap<FdRateLimitRule, FdRateLimitRuleDto>().MaskSensitiveData();
            CreateMap<CreateFdRateLimitRuleDto, FdRateLimitRule>();
            CreateMap<UpdateFdRateLimitRuleDto, FdRateLimitRule>();
        }
    }
}