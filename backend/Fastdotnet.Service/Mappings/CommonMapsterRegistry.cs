using Mapster;

namespace Fastdotnet.Service.Mappings
{
    /// <summary>
    /// 通用 Mapster 映射配置（类型转换、全局规则等）
    /// </summary>
    public class CommonMapsterRegistry : IRegister
    {
        public void Register(TypeAdapterConfig config)
        {
            // ✅ 全局：string → UserRefDto
            config.NewConfig<string, UserRefDto>()
                .MapWith(id => new UserRefDto { Id = id ?? string.Empty });

            // 未来可扩展其他通用映射，例如：
            // config.NewConfig<DateTime, string>().MapWith(...);
        }
    }
}
