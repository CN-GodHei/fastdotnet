
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
