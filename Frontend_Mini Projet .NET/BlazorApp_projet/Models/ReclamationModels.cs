using System;
using System.ComponentModel.DataAnnotations;
using System.Text.Json;
using System.Text.Json.Serialization;
using static BlazorApp_projet.Pages.GestionReclamations;

namespace BlazorApp1.Models
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

        // MODIFIEZ CES PROPRIÉTÉS :
        [JsonPropertyName("statut")]
        private object? _statutRaw { get; set; }

        [JsonIgnore]
        public string Statut
        {
            get
            {
                if (_statutRaw == null) return "NonTraitee";

                // Si c'est déjà une string
                if (_statutRaw is string str)
                    return str;

                // Si c'est un nombre (enum)
                if (_statutRaw is int intValue)
                {
                    return intValue switch
                    {
                        0 => "NonTraitee",
                        1 => "EnCours",
                        2 => "Traitee",
                        3 => "Annulee",
                        _ => "NonTraitee"
                    };
                }

                // Si c'est JsonElement (depuis la désérialisation)
                if (_statutRaw is JsonElement jsonElement)
                {
                    if (jsonElement.ValueKind == JsonValueKind.String)
                        return jsonElement.GetString() ?? "NonTraitee";

                    if (jsonElement.ValueKind == JsonValueKind.Number)
                    {
                        return jsonElement.GetInt32() switch
                        {
                            0 => "NonTraitee",
                            1 => "EnCours",
                            2 => "Traitee",
                            3 => "Annulee",
                            _ => "NonTraitee"
                        };
                    }
                }

                return "NonTraitee";
            }
            set => _statutRaw = value;
        }

        [JsonIgnore]
        public string StatutText => Statut switch
        {
            "NonTraitee" => "Non traitée",
            "EnCours" => "En cours",
            "Traitee" => "Traitée",
            "Annulee" => "Annulée",
            _ => "Non traitée"
        };

        public string? ResponsableSAVId { get; set; }
        public string? ResponsableSAVNom { get; set; }
        public DateTime DateCreation { get; set; }
        public DateTime? DateModification { get; set; }
        public DateTime? DateCloture { get; set; }
        public string? Solution { get; set; }
    }

    public class CreateReclamationModel
    {
        [Required(ErrorMessage = "Le titre est requis")]
        [StringLength(200, ErrorMessage = "Le titre ne doit pas dépasser 200 caractères")]
        public string Titre { get; set; } = string.Empty;

        [Required(ErrorMessage = "La description est requise")]
        public string Description { get; set; } = string.Empty;

        [Required(ErrorMessage = "La référence de l'article est requise")]
        public string ArticleReference { get; set; } = string.Empty;

        [Required(ErrorMessage = "La date d'achat est requise")]
        public DateTime DateAchat { get; set; }

        [Required(ErrorMessage = "Le prix d'achat est requis")]
        [Range(0.01, 1000000, ErrorMessage = "Le prix doit être compris entre 0.01 et 1 000 000")]
        public decimal PrixAchat { get; set; }

        // Ces champs seront remplis automatiquement
        public string? ClientId { get; set; }
        public string? ClientEmail { get; set; }
        public string? ClientNom { get; set; }
    }

    public class UpdateReclamationModel
    {
        [StringLength(200, ErrorMessage = "Le titre ne doit pas dépasser 200 caractères")]
        public string? Titre { get; set; }

        public string? Description { get; set; }
        public StatutReclamation? Statut { get; set; }
        public string? ResponsableSAVId { get; set; }
        public string? ResponsableSAVNom { get; set; }
        public string? NotesInterne { get; set; }
        public string? Solution { get; set; }
        public decimal? MontantFacture { get; set; }
        public int? DureeGarantieMois { get; set; } = 24;
    }

    public class AssignResponsableModel
    {
        [Required(ErrorMessage = "L'ID du responsable est requis")]
        public string ResponsableId { get; set; } = string.Empty;

        [Required(ErrorMessage = "Le nom du responsable est requis")]
        public string ResponsableNom { get; set; } = string.Empty;
    }

    public class UpdateStatutModel
    {
        [Required(ErrorMessage = "Le nouveau statut est requis")]
        public StatutReclamation NouveauStatut { get; set; }
        public string? Solution { get; set; }
    }

    public class ReclamationStats
    {
        public int TotalReclamations { get; set; }
        public int NonTraitees { get; set; }
        public int EnCours { get; set; }
        public int Traitees { get; set; }
        public int Annulees { get; set; }
        public int AvecResponsable { get; set; }
        public int SousGarantie { get; set; }
        public int HorsGarantie { get; set; }
        public int Aujourdhui { get; set; }
        public int CetteSemaine { get; set; }
        public decimal TotalMontantFacture { get; set; }
        public double MoyenneJours { get; set; }
    }

    public class WarrantyCheckResponse1
    {
        public int ReclamationId { get; set; }
        public bool SousGarantie { get; set; }
        public int DureeGarantieMois { get; set; }
    }

    public class UserDto
    {
        public string Id { get; set; } = string.Empty;
        public string Username { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string PhoneNumber { get; set; } = string.Empty;
        public List<string> Roles { get; set; } = new List<string>();
        public DateTime CreatedAt { get; set; }
    }
}
