namespace SAV.InterventionsAPI.Models
{
    public class Intervention
    {
        public Guid Id { get; set; }
        public string Numero { get; set; } = string.Empty;
        public Guid ReclamationId { get; set; }          // Lien avec la réclamation
        public string TechnicienNom { get; set; } = string.Empty;
        public string TechnicienSpecialite { get; set; } = "Generaliste";
        public DateTime DatePlanification { get; set; }
        public DateTime? DateDebut { get; set; }
        public DateTime? DateFin { get; set; }
        public string Statut { get; set; } = "Planifiee"; // Planifiee, EnCours, Terminee, Annulee
        public string Description { get; set; } = string.Empty;
        public string Rapport { get; set; } = string.Empty;
        public bool SousGarantie { get; set; } = true;

        // Facturation (si pas sous garantie)
        public decimal? CoutMainOeuvre { get; set; }
        public decimal? CoutPieces { get; set; }
        public decimal? MontantFacture { get; set; }
        public DateTime? DateFacturation { get; set; }
        public bool FacturePayee { get; set; }

        // Métadonnées
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }
        public string? CreatedBy { get; set; } // Email du responsable SAV
    }

    public static class InterventionStatut
    {
        public const string Planifiee = "Planifiee";
        public const string EnCours = "EnCours";
        public const string Terminee = "Terminee";
        public const string Annulee = "Annulee";
    }

    public static class SpecialiteTechnicien
    {
        public const string Chauffage = "Chauffage";
        public const string Sanitaire = "Sanitaire";
        public const string Generaliste = "Generaliste";
    }
}