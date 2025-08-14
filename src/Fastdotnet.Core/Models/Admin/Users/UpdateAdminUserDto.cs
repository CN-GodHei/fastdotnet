using System.ComponentModel.DataAnnotations;

namespace Fastdotnet.Core.Models.Admin.Users
{
    /// <summary>
    /// 用于更新管理员的DTO
    /// </summary>
    public class UpdateAdminUserDto
    {
        public string? FullName { get; set; }

        [EmailAddress(ErrorMessage = "无效的邮箱地址")]
        public string? Email { get; set; }

        public string? Phone { get; set; }

        public bool? IsActive { get; set; }
    }
}
