using System.ComponentModel.DataAnnotations;

namespace SecureAPI_JWT.Models
{
    public class CreateUserModel
    {
        [Required]
        public string Username { get; set; } = string.Empty;

        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;

        [Required]
        public string FirstName { get; set; } = string.Empty;

        [Required]
        public string LastName { get; set; } = string.Empty;

        [Required]
        public string Role { get; set; } = string.Empty; // "Client" ou "ResponsableSAV"

        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; } = string.Empty;

        public string? PhoneNumber { get; set; }
    }
}