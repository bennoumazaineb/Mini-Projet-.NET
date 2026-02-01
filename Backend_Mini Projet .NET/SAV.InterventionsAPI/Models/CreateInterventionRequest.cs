using System.ComponentModel.DataAnnotations;

namespace SAV.InterventionsAPI.Models
{
    public class CreateInterventionRequest
    {
        [Required]
        public Guid ReclamationId { get; set; }

        [Required]
        public string TechnicienNom { get; set; } = string.Empty;

        public string TechnicienSpecialite { get; set; } = "Generaliste";

        [Required]
        public DateTime DatePlanification { get; set; }

        [Required]
        public string Description { get; set; } = string.Empty;

        [Required]
        public bool SousGarantie { get; set; } = true;
    }
}