using AutoMapper;
using Fastdotnet.Core.Entities.System;
using Fastdotnet.Core.Models.System;

namespace Fastdotnet.Service.Mappings
{
    public class RoleProfile : Profile
    {
        public RoleProfile()
        {
            CreateMap<FdRole, RoleDto>();
            CreateMap<CreateRoleDto, FdRole>();
            CreateMap<UpdateRoleDto, FdRole>();
        }
    }
}
