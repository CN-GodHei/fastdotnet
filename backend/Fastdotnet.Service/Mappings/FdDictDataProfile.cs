using Fastdotnet.Core.Dtos.Sys;
using Fastdotnet.Core.Entities.Sys;

namespace Fastdotnet.Service.Mappings
{
    /// <summary>
    /// AutoMapper 配置文件
    /// </summary>
    public class FdDictDataProfile : Profile
    {
        public FdDictDataProfile()
        {
            // Entity -> DTO（枚举自动转换为 int）
            CreateMap<FdDictData, FdDictDataDto>()
                .ForMember(dest => dest.ValueType, opt => opt.MapFrom(src => (int)src.ValueType))
                .MaskSensitiveData();
            CreateMap<FdDictData, FdDictDataSimple>();
            CreateMap<FdDictData, FdDictDataMinimal>();
            // DTO -> Entity（int 自动转换为枚举）
            CreateMap<CreateFdDictDataDto, FdDictData>()
                .ForMember(dest => dest.ValueType, opt => opt.MapFrom(src => (DictValueType)src.ValueType));
            
            CreateMap<UpdateFdDictDataDto, FdDictData>()
                .ForMember(dest => dest.ValueType, opt => opt.MapFrom(src => (DictValueType)src.ValueType));
        }
    }
}
