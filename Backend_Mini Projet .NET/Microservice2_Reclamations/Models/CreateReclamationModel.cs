using System;
using System.ComponentModel.DataAnnotations;

namespace Microservice2_Reclamations.Models
{
    public class CreateReclamationModel
    {
        [Required]
        [StringLength(200)]
        public string Titre { get; set; } = string.Empty;

        [Required]
        public string Description { get; set; } = string.Empty;

        [Required]
        public string ArticleReference { get; set; } = string.Empty;

        [Required]
        public DateTime DateAchat { get; set; }

        [Required]
        [Range(1, 1000000)]
        public decimal PrixAchat { get; set; }

        // Client info (vient du token JWT)
        public string? ClientId { get; set; }
        public string? ClientEmail { get; set; }
        public string? ClientNom { get; set; }
    }
}