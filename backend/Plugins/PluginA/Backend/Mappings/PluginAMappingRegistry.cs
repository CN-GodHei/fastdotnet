using Fastdotnet.Core.Extensions;
using Mapster;
using PluginA.Controllers;
using PluginA.Dto;
using PluginA.Entities;

namespace PluginA.Mappings
{
    /// <summary>
    /// PluginA DTO 映射配置（从 AutoMapper Profile 迁移）
    /// </summary>
    public class PluginAMappingRegistry : IRegister
    {
        public void Register(TypeAdapterConfig config)
        {
            // PluginATest 映射
            config.NewConfig<PluginATest, PluginATestDto>()
                .MaskSensitiveData();
            config.NewConfig<PluginATestCreateDto, PluginATest>();
            config.NewConfig<PluginATestUpdateDto, PluginATest>()
                .IgnoreNullValues(true);

            // PluginAUserExtension 映射
            config.NewConfig<PluginAUserExtension, PluginAUserExtensionDto>()
                .MaskSensitiveData();
            config.NewConfig<CreatePluginAUserExtensionDto, PluginAUserExtension>();
            config.NewConfig<UpdatePluginAUserExtensionDto, PluginAUserExtension>();
            config.NewConfig<CreateUserWithExtensionRequest, PluginAUserExtension>();
        }
    }
}
