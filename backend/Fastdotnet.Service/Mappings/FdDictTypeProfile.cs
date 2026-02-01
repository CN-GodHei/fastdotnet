
namespace Fastdotnet.Service.Mappings
{
    /// <summary>
    /// AutoMapper配置文件
    /// </summary>
    public class FdDictTypeProfile : Profile
    {
        public FdDictTypeProfile()
        {
            // Source -> Target
            CreateMap<FdDictType, FdDictTypeDto>().MaskSensitiveData();
            CreateMap<CreateFdDictTypeDto, FdDictType>();
            CreateMap<UpdateFdDictTypeDto, FdDictType>();
        }
    }
}
