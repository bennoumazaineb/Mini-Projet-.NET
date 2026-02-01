namespace SAV.InterventionsAPI.Models
{
    public class FactureResponse
    {
        public Guid InterventionId { get; set; }
        public string NumeroIntervention { get; set; } = string.Empty;
        public DateTime DateFacturation { get; set; }
        public string ClientNom { get; set; } = string.Empty; // À remplir depuis ClientsAPI
        public string TechnicienNom { get; set; } = string.Empty;
        public decimal CoutMainOeuvre { get; set; }
        public decimal CoutPieces { get; set; }
        public decimal MontantTotalHT { get; set; }
        public decimal TVA { get; set; } // 20%
        public decimal MontantTotalTTC { get; set; }
        public bool EstPayee { get; set; }
        public bool SousGarantie { get; set; }
        public string Message { get; set; } = string.Empty;
    }

    public class CalculFactureRequest
    {
        public decimal DureeHeures { get; set; } = 1;
        public List<PieceFacture> Pieces { get; set; } = new();
    }

    public class PieceFacture
    {
        public string Reference { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public int Quantite { get; set; } = 1;
        public decimal PrixUnitaire { get; set; }
    }
}