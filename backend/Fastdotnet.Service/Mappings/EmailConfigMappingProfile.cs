using AutoMapper;
using Fastdotnet.Core.Entities.System;
using Fastdotnet.Core.Extensions;
using Fastdotnet.Core.Models.System;

namespace Fastdotnet.Service.Mappings
{
    public class EmailConfigMappingProfile : Profile
    {
        public EmailConfigMappingProfile()
        {
            // 用于读取数据（从实体映射到输出DTO）
            CreateMap<EmailConfig, FdEmailConfigDto>().MaskSensitiveData();

            // 用于创建数据（从创建DTO映射到实体）
            CreateMap<FdCreateEmailConfigDto, EmailConfig>();

            // 用于更新数据（从更新DTO映射到实体）
            CreateMap<FdUpdateEmailConfigDto, EmailConfig>();
        }
    }
}
