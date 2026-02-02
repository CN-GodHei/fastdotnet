
namespace Fastdotnet.Service.Mappings
{
    /// <summary>
    /// AdminUser的AutoMapper配置文件
    /// </summary>
    public class AdminUserProfile : Profile
    {
        public AdminUserProfile()
        {
            // Source -> Target
            CreateMap<FdAdminUser, FdAdminUserDto>().MaskSensitiveData();
            CreateMap<CreateFdAdminUserDto, FdAdminUser>();
            CreateMap<UpdateFdAdminUserDto, FdAdminUser>();
        }
    }
}
