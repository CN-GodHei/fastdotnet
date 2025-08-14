using System.ComponentModel.DataAnnotations;

namespace Fastdotnet.Core.Models.Auth
{
    public class LoginDto
    {
        [Required(ErrorMessage = "用户名不能为空")]
        public string Username { get; set; }

        [Required(ErrorMessage = "密码不能为空")]
        public string Password { get; set; }
    }
}
