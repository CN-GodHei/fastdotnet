using Fastdotnet.Core.Entities.Sys;

namespace Fastdotnet.Core.Dtos.Sys
{
    // 已迁移到 CoreDtoMappingRegistry.cs
    //public class MenuButtonProfile : Profile
    //{
    //    public MenuButtonProfile()
    //    {
    //        CreateMap<FdMenuButton, FdMenuButtonDto>();
    //        CreateMap<CreateFdFdMenuButtonDto, FdMenuButton>();
    //        CreateMap<UpdateFdFdMenuButtonDto, FdMenuButton>();
    //    }
    //}

    public class FdMenuButtonDto
    {
        public string Id { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string Code { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string MenuCode { get; set; } = string.Empty;
        public string Module { get; set; } = string.Empty;
        public string Category { get; set; } = string.Empty;
        public int Sort { get; set; }
        public string PermissionCode { get; set; } = string.Empty;
        public bool IsEnabled { get; set; }
    }

    public class CreateFdFdMenuButtonDto
    {
        public string Name { get; set; } = string.Empty;
        public string Code { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string MenuCode { get; set; } = string.Empty;
        public string Module { get; set; } = string.Empty;
        public string Category { get; set; } = string.Empty;
        public int Sort { get; set; }
        public string PermissionCode { get; set; } = string.Empty;
        public bool IsEnabled { get; set; } = true;
    }

    public class UpdateFdFdMenuButtonDto
    {
        public string Name { get; set; } = string.Empty;
        public string Code { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string MenuCode { get; set; } = string.Empty;
        public string Module { get; set; } = string.Empty;
        public string Category { get; set; } = string.Empty;
        public int Sort { get; set; }
        public string PermissionCode { get; set; } = string.Empty;
        public bool IsEnabled { get; set; }
    }
}