
using Fastdotnet.Core.Dtos.Sys;
using Fastdotnet.Core.Entities.Sys;

namespace Fastdotnet.Service.Mappings
{
    /// <summary>
    /// FdMenu的AutoMapper配置文件
    /// </summary>
    public class FdMenuProfile : Profile
    {
        public FdMenuProfile()
        {
            // Source -> Target
            CreateMap<FdMenu, FdMenuDto>().MaskSensitiveData()
            .ForMember(dest => dest.Creator, opt => opt.MapFrom(src => src.CreatedBy))
            .ForMember(dest => dest.Updater, opt => opt.MapFrom(src => src.UpdatedBy))
            .ForMember(dest => dest.Deleter, opt => opt.MapFrom(src => src.DeletedBy));
            CreateMap<CreateFdMenuDto, FdMenu>();
            CreateMap<UpdateFdMenuDto, FdMenu>();
        }
    }
}
