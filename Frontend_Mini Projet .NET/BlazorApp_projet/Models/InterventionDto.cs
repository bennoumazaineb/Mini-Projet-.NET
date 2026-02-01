using System;
using System.ComponentModel.DataAnnotations;

namespace BlazorApp1.Models
{
    public class InterventionDto
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "La réclamation est requise")]
        public int ReclamationId { get; set; }

        [Required(ErrorMessage = "Le technicien est requis")]
        public int TechnicianId { get; set; }

        public string? TechnicianName { get; set; }

        [Required(ErrorMessage = "La date d'intervention est requise")]
        public DateTime InterventionDate { get; set; } = DateTime.Now;

        [Required(ErrorMessage = "La description est requise")]
        [StringLength(500, ErrorMessage = "La description ne peut pas dépasser 500 caractères")]
        public string Description { get; set; } = string.Empty;

        [Required(ErrorMessage = "Le statut est requis")]
        public string Status { get; set; } = "Planifiée";

        public bool IsUnderWarranty { get; set; } = true;

        [Range(0, double.MaxValue, ErrorMessage = "Le coût des pièces doit être positif")]
        public decimal? PartsCost { get; set; }

        [Range(0, double.MaxValue, ErrorMessage = "Le coût de la main d'œuvre doit être positif")]
        public decimal? LaborCost { get; set; }

        public decimal? TotalCost => IsUnderWarranty ? 0 : (PartsCost + LaborCost);

        [StringLength(1000, ErrorMessage = "Les notes ne peuvent pas dépasser 1000 caractères")]
        public string? Notes { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }
        public DateTime? CompletedAt { get; set; }

        public string Priority { get; set; } = "Normal";
        public string? Location { get; set; }

        [Range(1, 10, ErrorMessage = "Les heures estimées doivent être entre 1 et 10")]
        public int? EstimatedHours { get; set; }

        public string? ArticleReference { get; set; }

        // Properties for display
        public string? ReclamationTitle { get; set; }
        public string? ClientName { get; set; }
        public string? ArticleName { get; set; }
        public string? PriorityColor { get; set; }
        public string? StatusColor { get; set; }
    }

    public class CreateInterventionDto
    {
        [Required(ErrorMessage = "La réclamation est requise")]
        public int ReclamationId { get; set; }

        [Required(ErrorMessage = "Le technicien est requis")]
        public int TechnicianId { get; set; }

        [Required(ErrorMessage = "La date d'intervention est requise")]
        public DateTime InterventionDate { get; set; } = DateTime.Now;

        [Required(ErrorMessage = "La description est requise")]
        [StringLength(500, ErrorMessage = "La description ne peut pas dépasser 500 caractères")]
        public string Description { get; set; } = string.Empty;

        [Required(ErrorMessage = "Le statut est requis")]
        public string Status { get; set; } = "Planifiée";

        public bool IsUnderWarranty { get; set; } = true;

        [Range(0, double.MaxValue, ErrorMessage = "Le coût des pièces doit être positif")]
        public decimal? PartsCost { get; set; }

        [Range(0, double.MaxValue, ErrorMessage = "Le coût de la main d'œuvre doit être positif")]
        public decimal? LaborCost { get; set; }

        [StringLength(1000, ErrorMessage = "Les notes ne peuvent pas dépasser 1000 caractères")]
        public string? Notes { get; set; }

        [Required(ErrorMessage = "La priorité est requise")]
        public string Priority { get; set; } = "Normal";

        [StringLength(200, ErrorMessage = "Le lieu ne peut pas dépasser 200 caractères")]
        public string? Location { get; set; }

        [Range(1, 10, ErrorMessage = "Les heures estimées doivent être entre 1 et 10")]
        public int? EstimatedHours { get; set; }

        [StringLength(100, ErrorMessage = "La référence article ne peut pas dépasser 100 caractères")]
        public string? ArticleReference { get; set; }
    }

    public class UpdateInterventionDto
    {
        public DateTime? InterventionDate { get; set; }

        [StringLength(500, ErrorMessage = "La description ne peut pas dépasser 500 caractères")]
        public string? Description { get; set; }

        public string? Status { get; set; }

        public bool? IsUnderWarranty { get; set; }

        [Range(0, double.MaxValue, ErrorMessage = "Le coût des pièces doit être positif")]
        public decimal? PartsCost { get; set; }

        [Range(0, double.MaxValue, ErrorMessage = "Le coût de la main d'œuvre doit être positif")]
        public decimal? LaborCost { get; set; }

        [StringLength(1000, ErrorMessage = "Les notes ne peuvent pas dépasser 1000 caractères")]
        public string? Notes { get; set; }

        public string? Priority { get; set; }

        [StringLength(200, ErrorMessage = "Le lieu ne peut pas dépasser 200 caractères")]
        public string? Location { get; set; }

        [Range(1, 10, ErrorMessage = "Les heures estimées doivent être entre 1 et 10")]
        public int? EstimatedHours { get; set; }
    }

    public class DashboardStatsDto
    {
        public int TotalInterventions { get; set; }
        public int InterventionsPlanifiees { get; set; }
        public int InterventionsEnCours { get; set; }
        public int InterventionsTerminees { get; set; }
        public int InterventionsAnnulees { get; set; }
        public decimal TotalCost { get; set; }
        public decimal AverageCompletionTime { get; set; }
        public int InterventionsThisMonth { get; set; }
        public int UrgentInterventions { get; set; }
        public Dictionary<string, int> StatusDistribution { get; set; } = new();
        public Dictionary<string, decimal> MonthlyCosts { get; set; } = new();
    }

    public class TechnicianDto
    {
        public int Id { get; set; }
        public string UserId { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
        public string Specialization { get; set; } = string.Empty;
        public string PhoneNumber { get; set; } = string.Empty;
        public bool IsActive { get; set; } = true;
        public int ActiveInterventions { get; set; }
        public int CompletedInterventions { get; set; }
        public decimal SuccessRate { get; set; }
    }
}