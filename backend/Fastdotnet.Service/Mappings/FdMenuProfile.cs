using AutoMapper;
using Fastdotnet.Core.Entities.Admin;
using Fastdotnet.Core.Entities.System;
using Fastdotnet.Core.Extensions;
using Fastdotnet.Core.Models.Admin.Users;
using Fastdotnet.Core.Models.System;

namespace Fastdotnet.Service.Mappings
{
    /// <summary>
    /// FdMenu的AutoMapper配置文件
    /// </summary>
    public class FdMenuProfile : Profile
    {
        public FdMenuProfile()
        {
            // Source -> Target
            CreateMap<FdMenu, FdMenuDto>().MaskSensitiveData();
            CreateMap<CreateFdMenuDto, FdMenu>();
            CreateMap<UpdateFdMenuDto, FdMenu>();
        }
    }
}
