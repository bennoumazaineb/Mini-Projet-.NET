using System.ComponentModel.DataAnnotations;

namespace BlazorApp1.Models
{
    public class CreateUserModel
    {
        [Required(ErrorMessage = "Nom d'utilisateur requis")]
        public string Username { get; set; } = string.Empty;

        [Required(ErrorMessage = "Email requis")]
        [EmailAddress(ErrorMessage = "Format email invalide")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "Prénom requis")]
        public string FirstName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Nom requis")]
        public string LastName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Rôle requis")]
        public string Role { get; set; } = string.Empty;

        [Required(ErrorMessage = "Mot de passe requis")]
        [MinLength(6, ErrorMessage = "Minimum 6 caractères")]
        public string Password { get; set; } = string.Empty;

        public string? PhoneNumber { get; set; }
    }
}