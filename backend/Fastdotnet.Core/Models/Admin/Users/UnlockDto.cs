using System.ComponentModel.DataAnnotations;

namespace Fastdotnet.Core.Models.Admin.Users
{
    /// <summary>
    /// 解锁DTO
    /// </summary>
    public class UnlockDto
    {
        /// <summary>
        /// 密码
        /// </summary>
        [Required(ErrorMessage = "密码不能为空")]
        public string Password { get; set; }
    }
}