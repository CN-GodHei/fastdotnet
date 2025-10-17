using AutoMapper;
using Fastdotnet.Core.Entities.System;
using Fastdotnet.Core.Models.System;

namespace Fastdotnet.Service.Mappings
{
    public class CodeGenConfigProfile : Profile
    {
        public CodeGenConfigProfile()
        {
            CreateMap<CodeGenConfig, CodeGenConfigDto>();
            CreateMap<CreateCodeGenConfigDto, CodeGenConfig>();
            CreateMap<UpdateCodeGenConfigDto, CodeGenConfig>();
            CreateMap<CodeGenConfig, UpdateCodeGenConfigDto>();
            CreateMap<CodeGenConfig, CreateCodeGenConfigDto>();
        }
    }
}