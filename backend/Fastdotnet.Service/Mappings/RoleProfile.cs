using AutoMapper;
using Fastdotnet.Core.Dtos.System;
using Fastdotnet.Core.Entities.System;
using Fastdotnet.Core.Extensions;

namespace Fastdotnet.Service.Mappings
{
    public class RoleProfile : Profile
    {
        public RoleProfile()
        {
            CreateMap<FdRole, FdRoleDto>().MaskSensitiveData();
            CreateMap<CreateFdRoleDto, FdRole>();
            CreateMap<UpdateFdRoleDto, FdRole>();
        }
    }
}
