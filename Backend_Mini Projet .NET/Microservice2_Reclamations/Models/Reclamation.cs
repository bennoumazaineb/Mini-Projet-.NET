using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Microservice2_Reclamations.Models
{
    public class Reclamation
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string Titre { get; set; } = string.Empty;

        [Required]
        public string Description { get; set; } = string.Empty;

        [Required]
        public string ClientId { get; set; } = string.Empty; // ID du client (de l'AuthService)

        public string ClientEmail { get; set; } = string.Empty;
        public string ClientNom { get; set; } = string.Empty;

        public string ArticleReference { get; set; } = string.Empty; // Référence de l'article
        public DateTime DateAchat { get; set; } // Date d'achat pour vérifier garantie

        public bool SousGarantie { get; set; } // Calculé automatiquement
        public decimal? MontantFacture { get; set; } // Si hors garantie

        public StatutReclamation Statut { get; set; } = StatutReclamation.NonTraitee;

        public string? ResponsableSAVId { get; set; } // ID du responsable assigné
        public string? ResponsableSAVNom { get; set; }

        public DateTime DateCreation { get; set; } = DateTime.UtcNow;
        public DateTime? DateModification { get; set; }
        public DateTime? DateCloture { get; set; }

        public string? NotesInterne { get; set; } // Notes pour le responsable SAV
        public string? Solution { get; set; } // Solution apportée
    }
}