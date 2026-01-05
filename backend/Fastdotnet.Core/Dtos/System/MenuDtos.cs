using AutoMapper;
using Fastdotnet.Core.Entities.System;
using System.Collections.Generic;
using System.Linq;
using System;
using Fastdotnet.Core.Enum;
using Fastdotnet.Core.Dtos.Common;

namespace Fastdotnet.Core.Dtos.System
{
    public class MenuProfile : Profile
    {
        public MenuProfile()
        {
            CreateMap<FdMenu, FdMenuDto>();
            CreateMap<CreateFdMenuDto, FdMenu>();
            CreateMap<UpdateFdMenuDto, FdMenu>();
        }
    }

    public class FdMenuDto
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Code { get; set; }
        public string Path { get; set; }
        public string Icon { get; set; }
        public string? ParentCode { get; set; }
        public int Sort { get; set; }
        public MenuType Type { get; set; }
        public string Module { get; set; }
        public string Category { get; set; }
        public bool IsExternal { get; set; }
        public string ExternalUrl { get; set; }
        public bool IsEnabled { get; set; }
        public string PermissionCode { get; set; }
        
        // 新增字段 - 用于对接 vue-next-admin
        public string? Component { get; set; }
        public string? PluginId { get; set; }
        public bool IsHide { get; set; }
        public bool IsKeepAlive { get; set; }
        public bool IsAffix { get; set; }
        public bool IsIframe { get; set; }
        public bool IsFdMicroApp { get; set; }
        public bool isLink { get; set; }
        public string Title { get; set; }

        public bool SupportWeb { get; set; } = false;
        public bool SupportDesktop { get; set; } = false;
        public bool SupportMobile { get; set; } = false;

        public List<FdMenuDto> Children { get; set; }

        public UserRefDto Creator { get; set; }
        public UserRefDto Updater { get; set; }
        public UserRefDto Deleter { get; set; }
    }

    public class CreateFdMenuDto
    {
        public string Name { get; set; }
        public string Path { get; set; }
        public string Icon { get; set; }
        public string? ParentCode { get; set; }
        public int Sort { get; set; }
        public MenuType Type { get; set; }
        public string Module { get; set; }
        public string Category { get; set; }
        public bool IsExternal { get; set; }
        public string ExternalUrl { get; set; }
        public bool IsEnabled { get; set; } = true;
        public string PermissionCode { get; set; }
        
        // 新增字段 - 用于对接 vue-next-admin
        public string? Component { get; set; }
        public bool IsHide { get; set; } = false;
        public bool IsKeepAlive { get; set; } = true;
        public bool IsAffix { get; set; } = false;
        public bool IsIframe { get; set; } = false;
        public bool IsFdMicroApp { get; set; }
        public bool isLink { get; set; }
        public string Title { get; set; }
        public bool SupportWeb { get; set; } = false;
        public bool SupportDesktop { get; set; } = false;
        public bool SupportMobile { get; set; } = false;
    }

    public class UpdateFdMenuDto
    {
        public string Name { get; set; }
        public string Code { get; set; }
        public string Path { get; set; }
        public string Icon { get; set; }
        public string? ParentCode { get; set; }
        public int Sort { get; set; }
        public MenuType Type { get; set; }
        public string Module { get; set; }
        public string Category { get; set; }
        public bool IsExternal { get; set; }
        public string ExternalUrl { get; set; }
        public bool IsEnabled { get; set; }
        public string PermissionCode { get; set; }
        
        // 新增字段 - 用于对接 vue-next-admin
        public string? Component { get; set; }
        public bool IsHide { get; set; }
        public bool IsKeepAlive { get; set; }
        public bool IsAffix { get; set; }
        public bool IsFdMicroApp { get; set; }
        public bool IsIframe { get; set; }
        public bool isLink { get; set; }
        public string Title { get; set; }

        public bool SupportWeb { get; set; } = false;
        public bool SupportDesktop { get; set; } = false;
        public bool SupportMobile { get; set; } = false;
    }
}