using System.ComponentModel.DataAnnotations;

namespace InternalAuthService.Models
{
    public class RegisterUserModel
    {
        [Required]
        public required string Login { get; set; }

        [Required]
        public required string Password { get; set; }

        [Required]
        public required string PasswordRe { get; set; }

        [Required]
        [EmailAddress]
        public required string Email { get; set; }
    }
}
