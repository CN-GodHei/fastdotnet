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
    public class FdDictDataProfile : Profile
    {
        public FdDictDataProfile()
        {
            // Source -> Target
            CreateMap<FdDictData, FdDictDataDto>().MaskSensitiveData();
            CreateMap<CreateFdDictDataDto, FdDictData>();
            CreateMap<UpdateFdDictDataDto, FdDictData>();
        }
    }
}
