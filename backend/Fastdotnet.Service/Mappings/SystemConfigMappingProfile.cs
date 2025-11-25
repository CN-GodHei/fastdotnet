using AutoMapper;
using Fastdotnet.Core.Entities.System;
using Fastdotnet.Core.Extensions;
using Fastdotnet.Core.Models.System;

namespace Fastdotnet.Service.Mappings
{
    public class SystemConfigMappingProfile : Profile
    {
        public SystemConfigMappingProfile()
        {
            CreateMap<SystemInfoConfig, FdSystemInfoConfigDto>().ReverseMap().MaskSensitiveData();
            CreateMap<CreateFdSystemInfoConfigDto, SystemInfoConfig>().ReverseMap();
            CreateMap<UpdateFdSystemInfoConfigDto, SystemInfoConfig>().ReverseMap();
        }
    }
}
