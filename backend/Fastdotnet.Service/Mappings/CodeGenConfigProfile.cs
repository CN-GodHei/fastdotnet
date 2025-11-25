using AutoMapper;
using Fastdotnet.Core.Entities.System;
using Fastdotnet.Core.Extensions;
using Fastdotnet.Core.Models.System;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Fastdotnet.Service.Mappings
{
    public class CodeGenConfigProfile : Profile
    {
        public CodeGenConfigProfile()
        {
            CreateMap<FdCodeGenConfig, FdCodeGenConfigDto>()
                .MaskSensitiveData()
                .ForMember(dest => dest.MaskConfig, opt => opt.MapFrom(src =>
                    DeserializeMaskConfig(src.MaskConfig)));

            CreateMap<CreateFdCodeGenConfigDto, FdCodeGenConfig>()
                .ForMember(dest => dest.MaskConfig, opt => opt.MapFrom(src =>
                    SerializeMaskConfig(src.MaskConfig)));
            CreateMap<UpdateFdCodeGenConfigDto, FdCodeGenConfig>()
                .ForMember(dest => dest.MaskConfig, opt => opt.MapFrom(src =>
                    SerializeMaskConfig(src.MaskConfig)));

        }
        private MaskConfigModel? DeserializeMaskConfig(string? json)
        {
            if (string.IsNullOrEmpty(json))
                return null;
            
            try
            {
                return JsonConvert.DeserializeObject<MaskConfigModel>(json);
            }
            catch
            {
                return null;
            }
        }
        
        private string? SerializeMaskConfig(MaskConfigModel? model)
        {
            if (model == null)
                return null;
            
            try
            {
                return JsonConvert.SerializeObject(model);
            }
            catch
            {
                return null;
            }
        }
    }
}