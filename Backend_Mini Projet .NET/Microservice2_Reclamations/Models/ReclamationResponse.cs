using System;

namespace Microservice2_Reclamations.Models
{
    public class ReclamationResponse
    {
        public int Id { get; set; }
        public string Titre { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string ClientId { get; set; } = string.Empty;
        public string ClientEmail { get; set; } = string.Empty;
        public string ClientNom { get; set; } = string.Empty;
        public string ArticleReference { get; set; } = string.Empty;
        public DateTime DateAchat { get; set; }
        public bool SousGarantie { get; set; }
        public decimal? MontantFacture { get; set; }
        public StatutReclamation Statut { get; set; }
        public string? ResponsableSAVId { get; set; }
        public string? ResponsableSAVNom { get; set; }
        public DateTime DateCreation { get; set; }
        public DateTime? DateModification { get; set; }
        public DateTime? DateCloture { get; set; }
        public string? Solution { get; set; }
        public string StatutText => Statut.ToString();
    }
}