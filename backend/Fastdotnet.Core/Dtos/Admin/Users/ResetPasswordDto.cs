using System.ComponentModel.DataAnnotations;

namespace Fastdotnet.Core.Dtos.Admin.Users
{
    public class ResetPasswordDto
    {
        [Required(ErrorMessage = "新密码不能为空")]
        [StringLength(100, MinimumLength = 6, ErrorMessage = "密码长度至少为6个字符")]
        public string NewPassword { get; set; }
    }
}
