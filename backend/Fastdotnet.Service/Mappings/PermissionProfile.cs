using AutoMapper;
using Fastdotnet.Core.Entities.System;
using Fastdotnet.Core.Extensions;
using Fastdotnet.Core.Models.System;

namespace Fastdotnet.Service.Mappings
{
    public class PermissionProfile : Profile
    {
        public PermissionProfile()
        {
            CreateMap<FdPermission, FdPermissionDto>().MaskSensitiveData();
        }
    }
}
