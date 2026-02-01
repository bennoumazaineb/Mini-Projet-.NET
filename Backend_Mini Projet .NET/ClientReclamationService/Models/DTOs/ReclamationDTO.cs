using System.ComponentModel.DataAnnotations;

namespace ReclamationService.Models.DTOs
{
    public class ReclamationDTO
    {
        public int Id { get; set; }
        public string Reference { get; set; }
        public DateTime DateCreation { get; set; }
        public string Description { get; set; }
        public string Statut { get; set; }
        public string Priorite { get; set; }

        // Info Client
        public string ClientUserId { get; set; }
        public string ClientEmail { get; set; }
        public string ClientNomComplet { get; set; }
        public string ClientTelephone { get; set; }

        // Info Article
        public string ArticleNom { get; set; }
        public string ArticleReference { get; set; }
        public DateTime DateAchat { get; set; }
        public DateTime DateFinGarantie { get; set; }
        public bool EstSousGarantie { get; set; }
        public bool GarantieExpiree { get; set; }
        public int JoursRestantsGarantie { get; set; }

        // Intervention
        public int? InterventionId { get; set; }
    }

    public class CreateReclamationDTO
    {
        [Required]
        [MaxLength(500)]
        public string Description { get; set; }

        [Required]
        [MaxLength(100)]
        public string ArticleNom { get; set; }

        [Required]
        [MaxLength(50)]
        public string ArticleReference { get; set; }

        [Required]
        public DateTime DateAchat { get; set; }

        [Required]
        [Range(1, 60)]
        public int DureeGarantieMois { get; set; } = 24; // 2 ans par défaut

        // Le ClientUserId sera extrait du token JWT pour les Clients
        // Ou fourni manuellement pour les ResponsablesSAV
        public string ClientUserId { get; set; }
    }

    public class UpdateReclamationDTO
    {
        [MaxLength(500)]
        public string Description { get; set; }

        public string Statut { get; set; }
        public string Priorite { get; set; }

        // Seul ResponsableSAV peut modifier
        public string CommentaireResponsable { get; set; }
    }
}