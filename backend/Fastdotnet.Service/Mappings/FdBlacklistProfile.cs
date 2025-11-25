using AutoMapper;
using Fastdotnet.Core.Dtos.System;
using Fastdotnet.Core.Entities.System;
using Fastdotnet.Core.Extensions;

namespace Fastdotnet.Service.Mappings
{
    /// <summary>
    /// 黑名单映射配置
    /// </summary>
    public class FdBlacklistProfile : Profile
    {
        public FdBlacklistProfile()
        {
            CreateMap<FdBlacklist, FdBlacklistDto>().MaskSensitiveData();
            CreateMap<CreateFdBlacklistDto, FdBlacklist>();
            CreateMap<UpdateFdBlacklistDto, FdBlacklist>();
        }
    }
}