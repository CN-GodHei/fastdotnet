
namespace Fastdotnet.Core.Dtos.Auth
{
    public class LoginDto
    {
        [Required(ErrorMessage = "用户名不能为空")]
        public string Username { get; set; } = string.Empty;

        [Required(ErrorMessage = "密码不能为空")]
        public string Password { get; set; } = string.Empty;
        
        /// <summary>
        /// 验证码ID
        /// </summary>
        public string CaptchaId { get; set; } = string.Empty;
        
        /// <summary>
        /// 用户输入的验证码
        /// </summary>
        public string CaptchaCode { get; set; } = string.Empty;
    }

    public class LoginResultDto
    {
        /// <summary>
        /// token
        /// </summary>
        public string Token { get; set; } = string.Empty;
    }
}
