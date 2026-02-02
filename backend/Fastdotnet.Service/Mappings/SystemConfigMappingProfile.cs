
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
