using AutoMapper;
using Fastdotnet.Core.Extensions;
using PluginA.Dto;
using PluginA.Entities;

namespace PluginA.Controllers
{
    /// <summary>
    /// PluginATest实体和DTO之间的映射配置
    /// </summary>
    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile()
        {
            CreateMap<PluginATest, PluginATestDto>()
                .MaskSensitiveData();
            CreateMap<PluginATestCreateDto, PluginATest>();
            CreateMap<PluginATestUpdateDto, PluginATest>()
                .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));
        }
    }
}