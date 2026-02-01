namespace Microservice2_Reclamations.Models
{
    public enum StatutReclamation
    {
        NonTraitee = 0,    // Non encore traitée
        EnCours = 1,       // En cours de traitement
        Traitee = 2,       // Traitée
        Annulee = 3        // Annulée
    }
}