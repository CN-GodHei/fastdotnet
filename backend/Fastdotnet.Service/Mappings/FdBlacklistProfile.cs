
using Fastdotnet.Core.Dtos.Sys;
using Fastdotnet.Core.Entities.Sys;

namespace Fastdotnet.Service.Mappings
{
    /// <summary>
    /// 黑名单映射配置
    /// </summary>
    public class FdBlacklistProfile : Profile
    {
        public FdBlacklistProfile()
        {
            CreateMap<FdBlacklist, FdBlacklistDto>().MaskSensitiveData();
            CreateMap<CreateFdBlacklistDto, FdBlacklist>();
            CreateMap<UpdateFdBlacklistDto, FdBlacklist>();
        }
    }
}