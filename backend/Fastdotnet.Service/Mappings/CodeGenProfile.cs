using AutoMapper;
using Fastdotnet.Core.Dtos.System;
using Fastdotnet.Core.Entities.System;
using Fastdotnet.Core.Extensions;
using Newtonsoft.Json;

namespace Fastdotnet.Service.Mappings
{
    public class CodeGenProfile : Profile
    {
        public CodeGenProfile()
        {
            CreateMap<FdCodeGen, CodeGenConfigDto>()
                .ForMember(dest => dest.TableUniqueList, opt => opt.MapFrom(src => string.IsNullOrWhiteSpace(src.TableUniqueConfig) ? null : JsonConvert.DeserializeObject<List<TableUniqueConfigDto>>(src.TableUniqueConfig))).MaskSensitiveData();
            
            CreateMap<CreateCodeGenDto, FdCodeGen>()
                .ForMember(dest => dest.TableUniqueConfig, opt => opt.MapFrom(src => src.TableUniqueList != null ? JsonConvert.SerializeObject(src.TableUniqueList) : null));
                
            CreateMap<UpdateCodeGenDto, FdCodeGen>()
                .ForMember(dest => dest.TableUniqueConfig, opt => opt.MapFrom(src => src.TableUniqueList != null ? JsonConvert.SerializeObject(src.TableUniqueList) : null));
                
            CreateMap<FdCodeGen, UpdateCodeGenDto>()
                .ForMember(dest => dest.TableUniqueList, opt => opt.MapFrom(src => string.IsNullOrWhiteSpace(src.TableUniqueConfig) ? null : JsonConvert.DeserializeObject<List<TableUniqueConfigDto>>(src.TableUniqueConfig)));
                
            CreateMap<FdCodeGen, CreateCodeGenDto>()
                .ForMember(dest => dest.TableUniqueList, opt => opt.MapFrom(src => string.IsNullOrWhiteSpace(src.TableUniqueConfig) ? null : JsonConvert.DeserializeObject<List<TableUniqueConfigDto>>(src.TableUniqueConfig)));
            
            // 添加 TableUniqueConfig 映射
            CreateMap<TableUniqueConfigDto, TableUniqueConfig>();
            CreateMap<TableUniqueConfig, TableUniqueConfigDto>();
        }
    }
}