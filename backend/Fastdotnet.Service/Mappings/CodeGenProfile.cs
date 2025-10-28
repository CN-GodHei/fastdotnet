using AutoMapper;
using Fastdotnet.Core.Entities.System;
using Fastdotnet.Core.Models.System;
using Newtonsoft.Json;

namespace Fastdotnet.Service.Mappings
{
    public class CodeGenProfile : Profile
    {
        public CodeGenProfile()
        {
            CreateMap<FdCodeGen, CodeGenConfigDto>()
                .ForMember(dest => dest.TableUniqueList, opt => opt.MapFrom(src => string.IsNullOrWhiteSpace(src.TableUniqueConfig) ? null : JsonConvert.DeserializeObject<List<TableUniqueConfigDto>>(src.TableUniqueConfig)));
            
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