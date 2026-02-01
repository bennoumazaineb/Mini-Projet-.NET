using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ReclamationService.Models
{
    public enum StatutReclamation
    {
        Nouvelle,
        EnCours,
        Traitee,
        Annulee,
        EnAttentePiece
    }

    public enum PrioriteReclamation
    {
        Basse,
        Normale,
        Haute,
        Urgente
    }

    public class Reclamation
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string Reference { get; set; } = GenerateReference();

        [Required]
        public DateTime DateCreation { get; set; } = DateTime.UtcNow;

        [Required]
        [MaxLength(500)]
        public string Description { get; set; }

        public StatutReclamation Statut { get; set; } = StatutReclamation.Nouvelle;

        public PrioriteReclamation Priorite { get; set; } = PrioriteReclamation.Normale;

        // Référence au Client dans le microservice Auth
        [Required]
        public string ClientUserId { get; set; }

        // Données du Client (copiées pour performance)
        public string ClientEmail { get; set; }
        public string ClientNomComplet { get; set; }
        public string ClientTelephone { get; set; }

        // Informations sur l'article
        [Required]
        [MaxLength(100)]
        public string ArticleNom { get; set; }

        [Required]
        [MaxLength(50)]
        public string ArticleReference { get; set; }

        [Required]
        public DateTime DateAchat { get; set; }

        [Required]
        public DateTime DateFinGarantie { get; set; }

        public bool EstSousGarantie => DateTime.UtcNow <= DateFinGarantie;

        // Pour le suivi des interventions techniques
        public int? InterventionId { get; set; }

        // Méthode pour générer référence
        private static string GenerateReference()
        {
            return $"REC-{DateTime.Now:yyyyMMdd}-{Guid.NewGuid().ToString("N").Substring(0, 8).ToUpper()}";
        }

        // Calculer si la garantie est expirée
        public bool GarantieExpiree => DateTime.UtcNow > DateFinGarantie;

        // Jours restants de garantie
        public int JoursRestantsGarantie => GarantieExpiree ? 0 : (DateFinGarantie - DateTime.UtcNow).Days;
    }
}