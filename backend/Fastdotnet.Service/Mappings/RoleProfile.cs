using AutoMapper;
using Fastdotnet.Core.Entities.System;
using Fastdotnet.Core.Models.System;

namespace Fastdotnet.Service.Mappings
{
    public class RoleProfile : Profile
    {
        public RoleProfile()
        {
            CreateMap<FdRole, FdRoleDto>();
            CreateMap<CreateFdRoleDto, FdRole>();
            CreateMap<UpdateFdRoleDto, FdRole>();
        }
    }
}
