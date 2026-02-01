
namespace Fastdotnet.Service.Mappings
{
    /// <summary>
    /// AppUser的AutoMapper配置文件
    /// </summary>
    public class FdAppUserProfile : Profile
    {
        public FdAppUserProfile()
        {
            // Source -> Target
            CreateMap<FdAppUser, FdAppUserDto>().MaskSensitiveData();
            CreateMap<CreateFdAppUserDto, FdAppUser>();
            CreateMap<UpdateFdAppUserDto, FdAppUser>();
        }
    }
}
