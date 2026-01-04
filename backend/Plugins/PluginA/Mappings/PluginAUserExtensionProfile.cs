using AutoMapper;
using Fastdotnet.Core.Dtos.System;
using Fastdotnet.Core.Entities.System;
using Fastdotnet.Core.Extensions;
using PluginA.Controllers;
using PluginA.Dto;
using PluginA.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PluginA.Mappings
{
    public class PluginAUserExtensionProfile : Profile
    {
        public PluginAUserExtensionProfile()
        {
            CreateMap<PluginAUserExtension, PluginAUserExtensionDto>().MaskSensitiveData();
            CreateMap<CreatePluginAUserExtensionDto, PluginAUserExtension>();
            CreateMap<UpdatePluginAUserExtensionDto, PluginAUserExtension>();
            CreateMap<CreateUserWithExtensionRequest, PluginAUserExtension>();
        }
    }
}
