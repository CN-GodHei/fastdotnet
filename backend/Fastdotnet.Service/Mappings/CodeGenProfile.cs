using AutoMapper;
using Fastdotnet.Core.Entities.System;
using Fastdotnet.Core.Models.System;

namespace Fastdotnet.Service.Mappings
{
    public class CodeGenProfile : Profile
    {
        public CodeGenProfile()
        {
            CreateMap<FdCodeGen, CodeGenConfigDto>();
            CreateMap<CreateCodeGenDto, FdCodeGen>();
            CreateMap<UpdateCodeGenDto, FdCodeGen>();
            CreateMap<FdCodeGen, UpdateCodeGenDto>();
            CreateMap<FdCodeGen, CreateCodeGenDto>();
        }
    }
}