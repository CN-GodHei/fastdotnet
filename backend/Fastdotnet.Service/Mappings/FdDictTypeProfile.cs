using AutoMapper;
using Fastdotnet.Core.Dtos.System;
using Fastdotnet.Core.Entities.Admin;
using Fastdotnet.Core.Entities.System;
using Fastdotnet.Core.Extensions;
using Fastdotnet.Core.Dtos.Admin.Users;

namespace Fastdotnet.Service.Mappings
{
    /// <summary>
    /// AutoMapper配置文件
    /// </summary>
    public class FdDictTypeProfile : Profile
    {
        public FdDictTypeProfile()
        {
            // Source -> Target
            CreateMap<FdDictType, FdDictTypeDto>().MaskSensitiveData();
            CreateMap<CreateFdDictTypeDto, FdDictType>();
            CreateMap<UpdateFdDictTypeDto, FdDictType>();
        }
    }
}
