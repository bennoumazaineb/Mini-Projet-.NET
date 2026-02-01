using System.ComponentModel.DataAnnotations;

namespace BlazorApp1.Models
{
    public enum StatutReclamation
    {
        [Display(Name = "Nouvelle")]
        Nouvelle = 1,

        [Display(Name = "En cours")]
        EnCours = 2,

        [Display(Name = "En attente")]
        EnAttente = 3,

        [Display(Name = "Résolue")]
        Resolue = 4,

        [Display(Name = "Clôturée")]
        Cloturee = 5,

        [Display(Name = "Annulée")]
        Annulee = 6
    }

    // Extension pour obtenir le nom d'affichage
    public static class StatutReclamationExtensions
    {
        public static string GetDisplayName(this StatutReclamation statut)
        {
            return statut switch
            {
                StatutReclamation.Nouvelle => "Nouvelle",
                StatutReclamation.EnCours => "En cours",
                StatutReclamation.EnAttente => "En attente",
                StatutReclamation.Resolue => "Résolue",
                StatutReclamation.Cloturee => "Clôturée",
                StatutReclamation.Annulee => "Annulée",
                _ => statut.ToString()
            };
        }

        public static string GetBadgeClass(this StatutReclamation statut)
        {
            return statut switch
            {
                StatutReclamation.Nouvelle => "badge-nouvelle",
                StatutReclamation.EnCours => "badge-encours",
                StatutReclamation.EnAttente => "badge-attente",
                StatutReclamation.Resolue => "badge-resolue",
                StatutReclamation.Cloturee => "badge-cloture",
                StatutReclamation.Annulee => "badge-annule",
                _ => "badge-default"
            };
        }
    }
}