using AutoMapper;
using Fastdotnet.Core.Dtos.System;
using Fastdotnet.Core.Entities.System;

namespace Fastdotnet.Service.Mappings
{
    /// <summary>
    /// 限流规则映射配置
    /// </summary>
    public class FdRateLimitRuleProfile : Profile
    {
        public FdRateLimitRuleProfile()
        {
            CreateMap<FdRateLimitRule, FdRateLimitRuleDto>();
            CreateMap<CreateFdRateLimitRuleDto, FdRateLimitRule>();
            CreateMap<UpdateFdRateLimitRuleDto, FdRateLimitRule>();
        }
    }
}