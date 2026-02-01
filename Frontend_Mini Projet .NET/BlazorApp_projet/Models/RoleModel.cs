using System.ComponentModel.DataAnnotations;

namespace BlazorApp1.Models
{
    public class RoleModel
    {
        public string Id { get; set; } = string.Empty;

        [Required(ErrorMessage = "Le nom du rôle est requis")]
        [StringLength(50, MinimumLength = 2, ErrorMessage = "Le nom doit contenir entre 2 et 50 caractères")]
        public string Name { get; set; } = string.Empty;

        public string? Description { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public bool IsSystemRole { get; set; }
        public int UserCount { get; set; }
    }

    public class CreateRoleModel
    {
        [Required(ErrorMessage = "Le nom du rôle est requis")]
        [StringLength(50, MinimumLength = 2, ErrorMessage = "Le nom doit contenir entre 2 et 50 caractères")]
        [RegularExpression(@"^[a-zA-Z0-9]+$", ErrorMessage = "Le nom ne doit contenir que des lettres et chiffres")]
        public string Name { get; set; } = string.Empty;

        [StringLength(500, ErrorMessage = "La description ne doit pas dépasser 500 caractères")]
        public string Description { get; set; } = string.Empty;

        public RolePermissions Permissions { get; set; } = new();
    }

    public class RolePermissions
    {
        public bool UsersRead { get; set; }
        public bool UsersWrite { get; set; }
        public bool RolesRead { get; set; }
        public bool RolesWrite { get; set; }
        public bool ReclamationsRead { get; set; }
        public bool ReclamationsWrite { get; set; }
        public bool InterventionsRead { get; set; }
        public bool InterventionsWrite { get; set; }
        public bool ArticlesRead { get; set; }
        public bool ArticlesWrite { get; set; }
    }

    public class RoleApiResponse
    {
        public bool Success { get; set; }
        public string Message { get; set; } = string.Empty;
        public RoleModel? Role { get; set; }
        public List<RoleModel>? Roles { get; set; }
    }
}