using Fastdotnet.Core.Entities.Sys;

namespace Fastdotnet.Core.Dtos.Sys
{
    public class MenuButtonProfile : Profile
    {
        public MenuButtonProfile()
        {
            CreateMap<FdMenuButton, FdMenuButtonDto>();
            CreateMap<CreateFdFdMenuButtonDto, FdMenuButton>();
            CreateMap<UpdateFdFdMenuButtonDto, FdMenuButton>();
        }
    }

    public class FdMenuButtonDto
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Code { get; set; }
        public string Description { get; set; }
        public string MenuCode { get; set; }
        public string Module { get; set; }
        public string Category { get; set; }
        public int Sort { get; set; }
        public string PermissionCode { get; set; }
        public bool IsEnabled { get; set; }
    }

    public class CreateFdFdMenuButtonDto
    {
        public string Name { get; set; }
        public string Code { get; set; }
        public string Description { get; set; }
        public string MenuCode { get; set; }
        public string Module { get; set; }
        public string Category { get; set; }
        public int Sort { get; set; }
        public string PermissionCode { get; set; }
        public bool IsEnabled { get; set; } = true;
    }

    public class UpdateFdFdMenuButtonDto
    {
        public string Name { get; set; }
        public string Code { get; set; }
        public string Description { get; set; }
        public string MenuCode { get; set; }
        public string Module { get; set; }
        public string Category { get; set; }
        public int Sort { get; set; }
        public string PermissionCode { get; set; }
        public bool IsEnabled { get; set; }
    }
}