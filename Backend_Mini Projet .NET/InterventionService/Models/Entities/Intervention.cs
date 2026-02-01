using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace InterventionService.Models.Entities
{
    public class Intervention
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int ReclamationId { get; set; }

        [Required]
        public int TechnicianId { get; set; }

        [Required]
        [StringLength(100)]
        public string TechnicianName { get; set; } = string.Empty;

        [Required]
        public DateTime InterventionDate { get; set; }

        [Required]
        [StringLength(500)]
        public string Description { get; set; } = string.Empty;

        [Required]
        [StringLength(50)]
        public string Status { get; set; } = "Planifiée"; // Planifiée, EnCours, Terminée, Annulée

        [Required]
        public bool IsUnderWarranty { get; set; } = true;

        [Column(TypeName = "decimal(10,2)")]
        public decimal? PartsCost { get; set; }

        [Column(TypeName = "decimal(10,2)")]
        public decimal? LaborCost { get; set; }

        [NotMapped]
        public decimal? TotalCost => IsUnderWarranty ? 0 : (PartsCost + LaborCost);

        [StringLength(1000)]
        public string? Notes { get; set; }

        [Required]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public DateTime? UpdatedAt { get; set; }

        public DateTime? CompletedAt { get; set; }

        [StringLength(50)]
        public string? Priority { get; set; } = "Normal"; // Basse, Normal, Haute, Urgente

        [StringLength(200)]
        public string? Location { get; set; }

        [Range(1, 10)]
        public int? EstimatedHours { get; set; }

        [StringLength(100)]
        public string? ArticleReference { get; set; }
    }
}