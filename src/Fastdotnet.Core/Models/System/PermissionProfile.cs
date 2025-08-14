using AutoMapper;
using Fastdotnet.Core.Entities.System;

namespace Fastdotnet.Core.Models.System
{
    public class PermissionProfile : Profile
    {
        public PermissionProfile()
        {
            CreateMap<FdPermission, PermissionDto>();
        }
    }
}
