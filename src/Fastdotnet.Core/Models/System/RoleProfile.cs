using AutoMapper;
using Fastdotnet.Core.Entities.System;

namespace Fastdotnet.Core.Models.System
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
