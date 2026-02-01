using System.ComponentModel.DataAnnotations;

namespace ArticleService.Models
{
    public class Category
    {
        public int Id { get; set; }

        [Required]
        [StringLength(100)]
        public string Name { get; set; }

        [StringLength(500)]
        public string Description { get; set; }

        // Relation 1-n avec Article
        public virtual ICollection<Article> Articles { get; set; } = new List<Article>();

        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public DateTime? UpdatedAt { get; set; }
    }
}