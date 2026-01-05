using AutoMapper;
using Fastdotnet.Core.Dtos.System;
using Fastdotnet.Core.Entities.System;
using Fastdotnet.Core.Extensions;

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
