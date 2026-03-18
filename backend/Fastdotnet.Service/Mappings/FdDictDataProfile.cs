
using Fastdotnet.Core.Dtos.Sys;
using Fastdotnet.Core.Entities.Sys;

namespace Fastdotnet.Service.Mappings
{
    /// <summary>
    /// AutoMapper配置文件
    /// </summary>
    public class FdDictDataProfile : Profile
    {
        public FdDictDataProfile()
        {
            // Source -> Target
            CreateMap<FdDictData, FdDictDataDto>().MaskSensitiveData();
            CreateMap<CreateFdDictDataDto, FdDictData>();
            CreateMap<UpdateFdDictDataDto, FdDictData>();
        }
    }
}
