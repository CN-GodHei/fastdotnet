
using Fastdotnet.Core.Dtos.Sys;
using Fastdotnet.Core.Entities.Sys;

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
