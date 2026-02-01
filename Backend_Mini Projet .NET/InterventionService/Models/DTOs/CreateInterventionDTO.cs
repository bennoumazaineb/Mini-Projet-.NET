using System.ComponentModel.DataAnnotations;

namespace InterventionService.Models.DTOs
{
    public class CreateInterventionDTO
    {
        [Required]
        public int ReclamationId { get; set; }

        [Required]
        public int TechnicianId { get; set; }

        [Required]
        public string TechnicianName { get; set; } = string.Empty;

        [Required]
        [FutureDate]
        public DateTime InterventionDate { get; set; }

        [Required]
        [StringLength(500, MinimumLength = 10)]
        public string Description { get; set; } = string.Empty;

        [Required]
        public bool IsUnderWarranty { get; set; } = true;

        [Range(0, 10000)]
        public decimal? PartsCost { get; set; }

        [Range(0, 10000)]
        public decimal? LaborCost { get; set; }

        [StringLength(1000)]
        public string? Notes { get; set; }

        [StringLength(50)]
        public string? Priority { get; set; } = "Normal";

        [StringLength(200)]
        public string? Location { get; set; }

        [Range(1, 10)]
        public int? EstimatedHours { get; set; }

        [StringLength(100)]
        public string? ArticleReference { get; set; }
    }

    public class FutureDateAttribute : ValidationAttribute
    {
        public override bool IsValid(object? value)
        {
            if (value is DateTime date)
            {
                return date > DateTime.Now;
            }
            return false;
        }

        public override string FormatErrorMessage(string name)
        {
            return $"La date de {name} doit être dans le futur.";
        }
    }
}