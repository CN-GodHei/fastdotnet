
using Fastdotnet.Core.Dtos.Sys;
using Fastdotnet.Core.Entities.Sys;

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
