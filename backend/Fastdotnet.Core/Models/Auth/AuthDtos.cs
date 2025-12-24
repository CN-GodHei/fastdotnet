using System.ComponentModel.DataAnnotations;

namespace Fastdotnet.Core.Models.Auth
{
    public class SendRegistrationCodeDto
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }
    }

    public class AppRegisterDto
    {
        [Required]
        public string Username { get; set; }

        [Required]
        public string Password { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        public string VerificationCode { get; set; }
    }
    public class CheckRegistrUserNameDto
    {
        [Required]
        public string Username { get; set; }
    }

}
