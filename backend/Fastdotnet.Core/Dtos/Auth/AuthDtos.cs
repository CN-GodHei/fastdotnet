
namespace Fastdotnet.Core.Dtos.Auth
{
    public class SendRegistrationCodeDto
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;
    }

    public class AppRegisterDto
    {
        [Required]
        public string Username { get; set; } = string.Empty;

        [Required]
        public string Password { get; set; } = string.Empty;

        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;

        [Required]
        public string VerificationCode { get; set; } = string.Empty;
    }
    public class CheckRegistrUserNameDto
    {
        [Required]
        public string Username { get; set; } = string.Empty;

    }
}