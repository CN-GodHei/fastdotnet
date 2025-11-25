using AutoMapper;
using Fastdotnet.Core.Entities.System;
using Fastdotnet.Core.Extensions;
using Fastdotnet.Core.Models.System;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fastdotnet.Service.Mappings
{
    public class CodeGenConfigProfile : Profile
    {
        public CodeGenConfigProfile()
        {
            CreateMap<FdCodeGenConfig, FdCodeGenConfigDto>().MaskSensitiveData();
            CreateMap<CreateFdCodeGenConfigDto, FdCodeGenConfig>();
            CreateMap<UpdateFdCodeGenConfigDto, FdCodeGenConfig>();
            CreateMap<FdCodeGenConfig, UpdateFdCodeGenConfigDto>();
            CreateMap<FdCodeGenConfig, CreateFdCodeGenConfigDto>();
        }
    }
}
