using System.ComponentModel.DataAnnotations;

namespace BlazorApp1.Models
{
    public class AddRoleModel
    {
        [Required(ErrorMessage = "ID utilisateur requis")]
        public string UserId { get; set; } = string.Empty;

        [Required(ErrorMessage = "Rôle requis")]
        public string Role { get; set; } = string.Empty;
    }
}