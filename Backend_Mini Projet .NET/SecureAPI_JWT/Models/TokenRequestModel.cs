using System.ComponentModel.DataAnnotations;

namespace SecureAPI_JWT.Models
{
    public class TokenRequestModel
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;

        [Required]
        public string Password { get; set; } = string.Empty;
    }
}