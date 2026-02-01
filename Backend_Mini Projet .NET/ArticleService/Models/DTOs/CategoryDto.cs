using System.ComponentModel.DataAnnotations;

namespace ArticleService.Models.DTOs
{
    public class CategoryDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public int ArticleCount { get; set; }
        public DateTime CreatedAt { get; set; }
    }

    public class CategoryCreateDto
    {
        [Required]
        [StringLength(100)]
        public string Name { get; set; }

        [StringLength(500)]
        public string Description { get; set; }
    }

    public class CategoryUpdateDto
    {
        [StringLength(100)]
        public string Name { get; set; }

        [StringLength(500)]
        public string Description { get; set; }
    }
}