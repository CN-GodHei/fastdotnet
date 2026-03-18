
using Fastdotnet.Core.Dtos.Sys;
using Fastdotnet.Core.Entities.Sys;

namespace Fastdotnet.Service.Mappings
{
    public class CodeGenConfigProfile : Profile
    {
        public CodeGenConfigProfile()
        {
            CreateMap<FdCodeGenConfig, FdCodeGenConfigDto>()
                .MaskSensitiveData()
                .ForMember(dest => dest.MaskConfig, opt => opt.MapFrom(src =>
                    DeserializeMaskConfig(src.MaskConfig)))
                .ForMember(dest => dest.ForeignKeyConfig, opt => opt.MapFrom(src =>
                    DeserializeForeignKeyConfig(src.ForeignKeyConfig)));

            CreateMap<CreateFdCodeGenConfigDto, FdCodeGenConfig>()
                .ForMember(dest => dest.MaskConfig, opt => opt.MapFrom(src =>
                    SerializeMaskConfig(src.MaskConfig)))
                .ForMember(dest => dest.ForeignKeyConfig, opt => opt.MapFrom(src =>
                    SerializeForeignKeyConfig(src.ForeignKeyConfig)));
                    
            CreateMap<UpdateFdCodeGenConfigDto, FdCodeGenConfig>()
                .ForMember(dest => dest.MaskConfig, opt => opt.MapFrom(src =>
                    SerializeMaskConfig(src.MaskConfig)))
                .ForMember(dest => dest.ForeignKeyConfig, opt => opt.MapFrom(src =>
                    SerializeForeignKeyConfig(src.ForeignKeyConfig)));

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
        
        private ForeignKeyConfigModel? DeserializeForeignKeyConfig(string? json)
        {
            if (string.IsNullOrEmpty(json))
                return null;
            
            try
            {
                return JsonConvert.DeserializeObject<ForeignKeyConfigModel>(json);
            }
            catch
            {
                return null;
            }
        }
        
        private string? SerializeForeignKeyConfig(ForeignKeyConfigModel? model)
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