
using Fastdotnet.Core.Dtos.Sys;
using Fastdotnet.Core.Entities.Sys;

namespace Fastdotnet.Service.Mappings
{
    public class FdNationalStandardDictMappingProfile : Profile
    {
        public FdNationalStandardDictMappingProfile()
        {
            CreateMap<FdNationalStandardDict, FdNationalStandardDictDto>()
                .ForMember(t => t.ExtraObject,
                tout => tout.MapFrom(src => string.IsNullOrWhiteSpace(src.Extra) ? null
                : JsonConvert.DeserializeObject<Dictionary<string, object>>(src.Extra)));
            CreateMap<CreateFdNationalStandardDictDto, FdNationalStandardDict>()
                .ForMember(t => t.Extra, topt => topt.MapFrom(src => src.ExtraObject != null ? JsonConvert.SerializeObject(src.ExtraObject) : null));
            CreateMap<UpdateFdNationalStandardDictDto, FdNationalStandardDict>()
                .ForMember(t => t.Extra, topt => topt.MapFrom(src => src.ExtraObject != null ? JsonConvert.SerializeObject(src.ExtraObject) : null));
        }
    }
}
