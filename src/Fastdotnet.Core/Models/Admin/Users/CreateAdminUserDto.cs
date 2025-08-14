using System.ComponentModel.DataAnnotations;

namespace Fastdotnet.Core.Models.Admin.Users
{
    /// <summary>
    /// 用于创建管理员的DTO
    /// </summary>
    public class CreateAdminUserDto
    {
        [Required(ErrorMessage = "用户名不能为空")]
        [StringLength(50, MinimumLength = 3, ErrorMessage = "用户名长度必须在3到50个字符之间")]
        public string Username { get; set; }

        [Required(ErrorMessage = "密码不能为空")]
        [StringLength(100, MinimumLength = 6, ErrorMessage = "密码长度至少为6个字符")]
        public string Password { get; set; }

        public string? FullName { get; set; }

        [EmailAddress(ErrorMessage = "无效的邮箱地址")]
        public string? Email { get; set; }

        public string? Phone { get; set; }

        public bool IsActive { get; set; } = true;
    }
}
