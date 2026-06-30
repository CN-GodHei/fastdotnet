using Fastdotnet.Core.Entities.Sys;

namespace Fastdotnet.Core.Dtos.Sys
{
    // 已迁移到 CoreDtoMappingRegistry.cs
    //public class MenuProfile : Profile
    //{
    //    public MenuProfile()
    //    {
    //        CreateMap<FdMenu, FdMenuDto>();
    //        CreateMap<CreateFdMenuDto, FdMenu>();
    //        CreateMap<UpdateFdMenuDto, FdMenu>();
    //    }
    //}

    public class FdMenuDto
    {
        public string Id { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string Code { get; set; } = string.Empty;
        public string Path { get; set; } = string.Empty;
        public string Icon { get; set; } = string.Empty;
        public string? ParentCode { get; set; }
        public int Sort { get; set; }
        public MenuType Type { get; set; }
        public string Module { get; set; } = string.Empty;
        public string Category { get; set; } = string.Empty;
        public bool IsExternal { get; set; }
        public string ExternalUrl { get; set; } = string.Empty;
        public bool IsEnabled { get; set; }
        public string PermissionCode { get; set; } = string.Empty;

        public string? Component { get; set; }
        public string? PluginId { get; set; }
        public bool IsHide { get; set; }
        public bool IsKeepAlive { get; set; }
        public bool IsAffix { get; set; }
        public bool IsIframe { get; set; }
        public bool IsFdMicroApp { get; set; }
        public bool isLink { get; set; }
        public string Title { get; set; } = string.Empty;

        public bool SupportWeb { get; set; } = false;
        public bool SupportDesktop { get; set; } = false;
        public bool SupportMobile { get; set; } = false;

        public List<FdMenuDto> Children { get; set; } = new();

        public UserRefDto Creator { get; set; } = null!;
        public UserRefDto Updater { get; set; } = null!;
        public UserRefDto Deleter { get; set; } = null!;
    }

    public class CreateFdMenuDto
    {
        public string Name { get; set; } = string.Empty;
        public string Path { get; set; } = string.Empty;
        public string Icon { get; set; } = string.Empty;
        public string? ParentCode { get; set; }
        public int Sort { get; set; }
        public MenuType Type { get; set; }
        public string Module { get; set; } = string.Empty;
        public string Category { get; set; } = string.Empty;
        public bool IsExternal { get; set; }
        public string ExternalUrl { get; set; } = string.Empty;
        public bool IsEnabled { get; set; } = true;
        public string PermissionCode { get; set; } = string.Empty;

        public string? Component { get; set; }
        public bool IsHide { get; set; } = false;
        public bool IsKeepAlive { get; set; } = true;
        public bool IsAffix { get; set; } = false;
        public bool IsIframe { get; set; } = false;
        public bool IsFdMicroApp { get; set; }
        public bool isLink { get; set; }
        public string Title { get; set; } = string.Empty;
        public bool SupportWeb { get; set; } = false;
        public bool SupportDesktop { get; set; } = false;
        public bool SupportMobile { get; set; } = false;
    }

    public class UpdateFdMenuDto
    {
        public string Name { get; set; } = string.Empty;
        public string Code { get; set; } = string.Empty;
        public string Path { get; set; } = string.Empty;
        public string Icon { get; set; } = string.Empty;
        public string? ParentCode { get; set; }
        public int Sort { get; set; }
        public MenuType Type { get; set; }
        public string Module { get; set; } = string.Empty;
        public string Category { get; set; } = string.Empty;
        public bool IsExternal { get; set; }
        public string ExternalUrl { get; set; } = string.Empty;
        public bool IsEnabled { get; set; }
        public string PermissionCode { get; set; } = string.Empty;

        public string? Component { get; set; }
        public bool IsHide { get; set; }
        public bool IsKeepAlive { get; set; }
        public bool IsAffix { get; set; }
        public bool IsFdMicroApp { get; set; }
        public bool IsIframe { get; set; }
        public bool isLink { get; set; }
        public string Title { get; set; } = string.Empty;

        public bool SupportWeb { get; set; } = false;
        public bool SupportDesktop { get; set; } = false;
        public bool SupportMobile { get; set; } = false;
    }

    /// <summary>
    /// 菜单按钮关系
    /// </summary>
    public record MenuBtnRe
    {
        public string Id { get; set; } = string.Empty;

        public string Name { get; set; } = string.Empty;
        public string Title { get; set; } = string.Empty;
        public DataStatus DataStatus { get; set; } // 添加 DataStatus 字段
        public bool Exist { get; set; }
        public List<MenuBtnRe> Children { get; set; } = new();
        //public List<IdNameStatusDto> BtnList { get; set; }
        public List<MenuBtnReStatusDto> BtnList { get; set; } = new();
    }

    public record MenuBtnReStatusDto(string Id, string Name, DataStatus DataStatus, bool Exist);

}