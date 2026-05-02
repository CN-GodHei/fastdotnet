using Fastdotnet.Core.Dtos.Sys;
using Fastdotnet.Core.Entities.Sys;
using Fastdotnet.Core.Extensions;
using Mapster;

namespace Fastdotnet.Service.Mappings
{
    /// <summary>
    /// FdDictData 的 Mapster 映射配置（试点）
    /// </summary>
    public class FdDictDataMapsterRegistry : IRegister
    {
        public void Register(TypeAdapterConfig config)
        {
            // Entity -> DTO（枚举自动转换为 int，启用敏感数据脱敏）
            config.NewConfig<FdDictData, FdDictDataDto>()
                .Map(dest => dest.ValueType, src => (int)src.ValueType)
                .MaskSensitiveData();
            
            config.NewConfig<FdDictData, FdDictDataSimple>();
            config.NewConfig<FdDictData, FdDictDataMinimal>();
            
            // DTO -> Entity（int 自动转换为枚举）
            config.NewConfig<CreateFdDictDataDto, FdDictData>()
                .Map(dest => dest.ValueType, src => (DictValueType)src.ValueType);
            
            config.NewConfig<UpdateFdDictDataDto, FdDictData>()
                .Map(dest => dest.ValueType, src => (DictValueType)src.ValueType);
        }
    }
}
