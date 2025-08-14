using AutoMapper;
using Fastdotnet.Core.Entities.Admin;
using Fastdotnet.Core.Models.Admin.Users;

namespace Fastdotnet.Core.Models.Admin.Users
{
    /// <summary>
    /// AdminUser的AutoMapper配置文件
    /// </summary>
    public class AdminUserProfile : Profile
    {
        public AdminUserProfile()
        {
            // Source -> Target
            CreateMap<FdAdminUser, AdminUserDto>();
            CreateMap<CreateAdminUserDto, FdAdminUser>();
            CreateMap<UpdateAdminUserDto, FdAdminUser>();
        }
    }
}
