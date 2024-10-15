using System.ComponentModel.DataAnnotations;

namespace InternalAuthService.Models
{
    public class User
    {
        [Key]
        public Guid Id { get; set; }

        public required string Login { get; set; }

        public required string PasswordHash { get; set; }  

        [EmailAddress]
        public required string Email { get; set; }
    }
}
