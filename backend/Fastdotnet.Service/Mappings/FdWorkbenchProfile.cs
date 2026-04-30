using AutoMapper;
using Fastdotnet.Core.Dtos.App;
using Fastdotnet.Core.Dtos.Sys;
using Fastdotnet.Core.Entities.App;
using Fastdotnet.Core.Entities.Sys;

namespace Fastdotnet.Service.Mappings
{
    public class FdWorkbenchProfile : Profile
    {
        public FdWorkbenchProfile()
        {
            // 管理端卡片库映射
            CreateMap<FdWorkbenchCard, FdWorkbenchCardDto>();
            CreateMap<CreateFdWorkbenchCardDto, FdWorkbenchCard>();
            CreateMap<UpdateFdWorkbenchCardDto, FdWorkbenchCard>();

            // 应用端布局映射
            CreateMap<FdUserLayout, FdUserLayoutDto>();
            CreateMap<SaveFdUserLayoutDto, FdUserLayout>();
        }
    }
}
