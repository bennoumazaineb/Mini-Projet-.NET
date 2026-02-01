namespace SAV.InterventionsAPI.Models
{
    public class UpdateInterventionRequest
    {
        public string? TechnicienNom { get; set; }
        public string? TechnicienSpecialite { get; set; }
        public DateTime? DatePlanification { get; set; }
        public string? Description { get; set; }
        public string? Statut { get; set; }
        public string? Rapport { get; set; }
        public bool? SousGarantie { get; set; }
    }
}