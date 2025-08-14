using System.ComponentModel.DataAnnotations;

namespace Fastdotnet.Core.Models.System
{
    public class CreateRoleDto
    {
        [Required(ErrorMessage = "角色名称不能为空")]
        public string Name { get; set; }

        public string? Description { get; set; }

        [Required(ErrorMessage = "角色分类不能为空")]
        public string Category { get; set; } // "Admin" or "User"
    }
}