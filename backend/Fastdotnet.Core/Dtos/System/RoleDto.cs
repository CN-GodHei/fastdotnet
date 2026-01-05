using System;
using System.ComponentModel.DataAnnotations;

namespace Fastdotnet.Core.Dtos.System
{
    public class FdRoleDto
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Code { get; set; }
        public string? Description { get; set; }
        public string Category { get; set; }
        public bool IsSystem { get; set; }
        public DateTime CreatedAt { get; set; }
    }

    public class CreateFdRoleDto
    {
        [Required(ErrorMessage = "角色名称不能为空")]
        public string Name { get; set; }

        public string? Description { get; set; }

        [Required(ErrorMessage = "角色分类不能为空")]
        public string Category { get; set; } // "Admin" or "User"
    }

    public class UpdateFdRoleDto
    {
        [Required(ErrorMessage = "角色名称不能为空")]
        public string Name { get; set; }

        public string? Description { get; set; }
    }
}