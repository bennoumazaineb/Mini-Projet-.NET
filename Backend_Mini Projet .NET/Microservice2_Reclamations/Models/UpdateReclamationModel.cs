using System.ComponentModel.DataAnnotations;

namespace Microservice2_Reclamations.Models
{
    public class UpdateReclamationModel
    {
        [StringLength(200)]
        public string? Titre { get; set; }

        public string? Description { get; set; }

        public StatutReclamation? Statut { get; set; }

        public string? ResponsableSAVId { get; set; }
        public string? ResponsableSAVNom { get; set; }

        public string? NotesInterne { get; set; }
        public string? Solution { get; set; }

        public decimal? MontantFacture { get; set; }

        [Range(0, 100)]
        public int? DureeGarantieMois { get; set; } = 24; // Garantie par défaut 24 mois
    }
}