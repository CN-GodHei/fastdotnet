using System.ComponentModel.DataAnnotations;

namespace Fastdotnet.Core.Dtos.App
{
    /// <summary>
    /// 修改邮箱请求DTO
    /// </summary>
    public class ChangeEmailDto
    {
        /// <summary>
        /// 新邮箱地址
        /// </summary>
        [Required(ErrorMessage = "新邮箱不能为空")]
        [EmailAddress(ErrorMessage = "邮箱格式不正确")]
        public string NewEmail { get; set; }

        /// <summary>
        /// 验证码
        /// </summary>
        [Required(ErrorMessage = "验证码不能为空")]
        [StringLength(6, MinimumLength = 6, ErrorMessage = "验证码长度为6位")]
        public string VerificationCode { get; set; }
    }

    /// <summary>
    /// 修改密码请求DTO
    /// </summary>
    public class ChangePasswordDto
    {
        /// <summary>
        /// 当前密码
        /// </summary>
        [Required(ErrorMessage = "当前密码不能为空")]
        public string CurrentPassword { get; set; }

        /// <summary>
        /// 新密码
        /// </summary>
        [Required(ErrorMessage = "新密码不能为空")]
        [MinLength(6, ErrorMessage = "密码长度至少为6位")]
        [MaxLength(50, ErrorMessage = "密码长度不能超过50位")]
        public string NewPassword { get; set; }

        /// <summary>
        /// 确认新密码
        /// </summary>
        [Required(ErrorMessage = "确认密码不能为空")]
        [Compare("NewPassword", ErrorMessage = "两次输入的密码不一致")]
        public string ConfirmPassword { get; set; }
    }
}