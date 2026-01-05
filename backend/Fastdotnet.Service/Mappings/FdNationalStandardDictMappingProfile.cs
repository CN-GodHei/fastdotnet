using AutoMapper;
using Fastdotnet.Core.Dtos.System;
using Fastdotnet.Core.Entities.System;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
