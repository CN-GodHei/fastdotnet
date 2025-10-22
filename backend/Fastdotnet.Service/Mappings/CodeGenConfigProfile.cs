using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using Fastdotnet.Core.Entities.System;
using Fastdotnet.Core.Models.System;

namespace Fastdotnet.Service.Mappings
{
    public class CodeGenConfigProfile : Profile
    {
        public CodeGenConfigProfile()
        {
            CreateMap<FdCodeGenConfig, FdCodeGenConfigDto>();
            CreateMap<CreateFdCodeGenConfigDto, FdCodeGenConfig>();
            CreateMap<UpdateFdCodeGenConfigDto, FdCodeGenConfig>();
            CreateMap<FdCodeGenConfig, UpdateFdCodeGenConfigDto>();
            CreateMap<FdCodeGenConfig, CreateFdCodeGenConfigDto>();
        }
    }
}
