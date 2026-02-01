using System.ComponentModel.DataAnnotations;

namespace InterventionService.Models.DTOs
{
    public class UpdateInterventionDTO
    {
        [StringLength(500, MinimumLength = 10)]
        public string? Description { get; set; }

        [StringLength(50)]
        public string? Status { get; set; }

        public bool? IsUnderWarranty { get; set; }

        [Range(0, 10000)]
        public decimal? PartsCost { get; set; }

        [Range(0, 10000)]
        public decimal? LaborCost { get; set; }

        [StringLength(1000)]
        public string? Notes { get; set; }

        [StringLength(50)]
        public string? Priority { get; set; }

        [StringLength(200)]
        public string? Location { get; set; }

        [Range(1, 10)]
        public int? EstimatedHours { get; set; }

        [StringLength(100)]
        public string? ArticleReference { get; set; }
    }
}