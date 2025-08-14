using System.ComponentModel.DataAnnotations;

namespace Fastdotnet.Core.Models.System
{
    public class UpdateRoleDto
    {
        [Required(ErrorMessage = "角色名称不能为空")]
        public string Name { get; set; }

        public string? Description { get; set; }
    }
}
