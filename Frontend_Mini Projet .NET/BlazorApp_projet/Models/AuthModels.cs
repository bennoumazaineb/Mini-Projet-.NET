using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace BlazorApp1.Models
{
    public class LoginRequest
    {
        [Required(ErrorMessage = "Email requis")]
        [EmailAddress(ErrorMessage = "Email invalide")]
        [JsonPropertyName("email")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "Mot de passe requis")]
        [JsonPropertyName("password")]
        public string Password { get; set; } = string.Empty;
    }

    public class RegisterRequest
    {
        [Required(ErrorMessage = "Nom d'utilisateur requis")]
        [MinLength(3, ErrorMessage = "Minimum 3 caractères")]
        [JsonPropertyName("username")]
        public string Username { get; set; } = string.Empty;

        [Required(ErrorMessage = "Email requis")]
        [EmailAddress(ErrorMessage = "Email invalide")]
        [JsonPropertyName("email")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "Mot de passe requis")]
        [MinLength(8, ErrorMessage = "Minimum 8 caractères")]
        [JsonPropertyName("password")]
        public string Password { get; set; } = string.Empty;

        [Required(ErrorMessage = "Prénom requis")]
        [JsonPropertyName("firstName")]
        public string FirstName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Nom requis")]
        [JsonPropertyName("lastName")]
        public string LastName { get; set; } = string.Empty;
    }

    public class AuthResponse
    {
        [JsonPropertyName("message")]
        public string Message { get; set; } = string.Empty;

        [JsonPropertyName("isAuthenticated")]
        public bool IsAuthenticated { get; set; }

        [JsonPropertyName("username")]
        public string Username { get; set; } = string.Empty;

        [JsonPropertyName("email")]
        public string Email { get; set; } = string.Empty;

        [JsonPropertyName("roles")]
        public List<string> Roles { get; set; } = new();

        [JsonPropertyName("token")]
        public string Token { get; set; } = string.Empty;

        [JsonPropertyName("expiresOn")]
        public DateTime ExpiresOn { get; set; }
    }

    public class UserInfo
    {
        public string Username { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public List<string> Roles { get; set; } = new();
        public string Token { get; set; } = string.Empty;
        public DateTime ExpiresOn { get; set; }
    }
}
