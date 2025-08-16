using AutoMapper;
using Fastdotnet.Core.Entities.System;
using System.Collections.Generic;

namespace Fastdotnet.Core.Models.System
{
    public class MenuProfile : Profile
    {
        public MenuProfile()
        {
            CreateMap<FdMenu, MenuDto>();
            CreateMap<CreateMenuDto, FdMenu>();
            CreateMap<UpdateMenuDto, FdMenu>();
        }
    }

    public class MenuDto
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
        public List<MenuDto> Children { get; set; }
    }

    public class CreateMenuDto
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
    }

    public class UpdateMenuDto
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
    }
}