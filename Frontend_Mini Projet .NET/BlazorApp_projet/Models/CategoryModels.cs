using System.ComponentModel.DataAnnotations;

namespace BlazorApp1.Models
{
    public class Category
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Le nom de la catégorie est requis")]
        [StringLength(100, ErrorMessage = "Le nom ne peut pas dépasser 100 caractères")]
        public string Name { get; set; } = string.Empty;

        [StringLength(500, ErrorMessage = "La description ne peut pas dépasser 500 caractères")]
        public string Description { get; set; } = string.Empty;

        public virtual ICollection<Article> Articles { get; set; } = new List<Article>();
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public DateTime? UpdatedAt { get; set; }
    }

    public class CategoryCreateDto
    {
        [Required(ErrorMessage = "Le nom de la catégorie est requis")]
        [StringLength(100, ErrorMessage = "Le nom ne peut pas dépasser 100 caractères")]
        public string Name { get; set; } = string.Empty;

        [StringLength(500, ErrorMessage = "La description ne peut pas dépasser 500 caractères")]
        public string Description { get; set; } = string.Empty;
    }

    public class CategoryUpdateDto : CategoryCreateDto
    {
    }

    public class CategoryResponse
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public int ArticleCount { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }
}