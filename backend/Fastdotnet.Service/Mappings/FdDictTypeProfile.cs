using AutoMapper;
using Fastdotnet.Core.Entities.Admin;
using Fastdotnet.Core.Entities.System;
using Fastdotnet.Core.Extensions;
using Fastdotnet.Core.Models.Admin.Users;
using Fastdotnet.Core.Models.System;

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
