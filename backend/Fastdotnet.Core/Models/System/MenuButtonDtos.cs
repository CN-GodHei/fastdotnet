using AutoMapper;
using Fastdotnet.Core.Entities.System;

namespace Fastdotnet.Core.Models.System
{
    public class MenuButtonProfile : Profile
    {
        public MenuButtonProfile()
        {
            CreateMap<FdMenuButton, MenuButtonDto>();
            CreateMap<CreateMenuButtonDto, FdMenuButton>();
            CreateMap<UpdateMenuButtonDto, FdMenuButton>();
        }
    }

    public class MenuButtonDto
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

    public class CreateMenuButtonDto
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

    public class UpdateMenuButtonDto
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