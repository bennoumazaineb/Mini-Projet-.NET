using System.Net.Http.Json;
using BlazorApp1.Models;
using Microsoft.AspNetCore.Components;
using static BlazorApp_projet.Pages.GestionReclamations;

namespace BlazorApp1.Services
{
    public interface IReclamationService
    {
        Task<List<ReclamationResponse>> GetAllReclamationsAsync();
        Task<List<ReclamationResponse>> GetMesReclamationsAsync(string clientId);
        Task<ReclamationResponse?> GetReclamationByIdAsync(int id);
        Task<ReclamationResponse> CreateReclamationAsync(CreateReclamationModel model);
        Task<ReclamationResponse?> UpdateReclamationAsync(int id, UpdateReclamationModel model);
        Task<bool> DeleteReclamationAsync(int id);
        Task<bool> AssignerResponsableAsync(int id, string responsableId, string responsableNom);
        Task<bool> UpdateStatutAsync(int id, StatutReclamation nouveauStatut, string? solution);
        Task<WarrantyCheckResponse1> VerifierGarantieAsync(int id, int dureeMois = 24); // CHANGÉ ICI
        Task<List<ReclamationResponse>> GetReclamationsByStatutAsync(string statut);
        Task<List<ReclamationResponse>> SearchReclamationsAsync(string term);
        Task<ReclamationStats> GetReclamationStatsAsync();
    }

    public class ReclamationService : IReclamationService
    {
        private readonly HttpClient _httpClient;
        private readonly NavigationManager _navigationManager;

        public ReclamationService(HttpClient httpClient, NavigationManager navigationManager)
        {
            _httpClient = httpClient;
            _navigationManager = navigationManager;
        }

        public async Task<List<ReclamationResponse>> GetAllReclamationsAsync()
        {
            try
            {
                var response = await _httpClient.GetAsync("/gateway/reclamations");
                response.EnsureSuccessStatusCode();
                return await response.Content.ReadFromJsonAsync<List<ReclamationResponse>>() ?? new List<ReclamationResponse>();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erreur lors de la récupération des réclamations: {ex.Message}");
                throw;
            }
        }

        public async Task<List<ReclamationResponse>> GetMesReclamationsAsync(string clientId)
        {
            try
            {
                var response = await _httpClient.GetAsync($"/gateway/reclamations/mes-reclamations?clientId={Uri.EscapeDataString(clientId)}");
                response.EnsureSuccessStatusCode();
                return await response.Content.ReadFromJsonAsync<List<ReclamationResponse>>() ?? new List<ReclamationResponse>();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erreur lors de la récupération des réclamations client: {ex.Message}");
                throw;
            }
        }

        public async Task<ReclamationResponse?> GetReclamationByIdAsync(int id)
        {
            try
            {
                var response = await _httpClient.GetAsync($"/gateway/reclamations/{id}");
                if (response.IsSuccessStatusCode)
                {
                    return await response.Content.ReadFromJsonAsync<ReclamationResponse>();
                }
                return null;
            }
            catch
            {
                return null;
            }
        }

        public async Task<ReclamationResponse> CreateReclamationAsync(CreateReclamationModel model)
        {
            try
            {
                var response = await _httpClient.PostAsJsonAsync("/gateway/reclamations", model);
                response.EnsureSuccessStatusCode();
                return await response.Content.ReadFromJsonAsync<ReclamationResponse>() ?? throw new Exception("Réponse invalide");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erreur lors de la création de la réclamation: {ex.Message}");
                throw;
            }
        }

        public async Task<ReclamationResponse?> UpdateReclamationAsync(int id, UpdateReclamationModel model)
        {
            try
            {
                var response = await _httpClient.PutAsJsonAsync($"/gateway/reclamations/{id}", model);
                if (response.IsSuccessStatusCode)
                {
                    return await response.Content.ReadFromJsonAsync<ReclamationResponse>();
                }
                return null;
            }
            catch
            {
                return null;
            }
        }

        public async Task<bool> DeleteReclamationAsync(int id)
        {
            try
            {
                var response = await _httpClient.DeleteAsync($"/gateway/reclamations/{id}");
                return response.IsSuccessStatusCode;
            }
            catch
            {
                return false;
            }
        }

        public async Task<bool> AssignerResponsableAsync(int id, string responsableId, string responsableNom)
        {
            try
            {
                var model = new { ResponsableId = responsableId, ResponsableNom = responsableNom };
                var response = await _httpClient.PostAsJsonAsync($"/gateway/reclamations/{id}/assigner", model);
                return response.IsSuccessStatusCode;
            }
            catch
            {
                return false;
            }
        }

        public async Task<bool> UpdateStatutAsync(int id, StatutReclamation nouveauStatut, string? solution)
        {
            try
            {
                var model = new { NouveauStatut = nouveauStatut, Solution = solution };
                var response = await _httpClient.PatchAsJsonAsync($"/gateway/reclamations/{id}/statut", model);
                return response.IsSuccessStatusCode;
            }
            catch
            {
                return false;
            }
        }

        public async Task<WarrantyCheckResponse1> VerifierGarantieAsync(int id, int dureeMois = 24) // CHANGÉ ICI
        {
            try
            {
                var response = await _httpClient.GetAsync($"/gateway/reclamations/{id}/garantie?dureeMois={dureeMois}");
                response.EnsureSuccessStatusCode();

                // Si votre API retourne WarrantyCheckResponse, convertissez-le
                var result = await response.Content.ReadFromJsonAsync<WarrantyCheckResponse1>();
                if (result != null)
                {
                    return result;
                }

                // Fallback si l'API retourne un type différent
                return new WarrantyCheckResponse1
                {
                    ReclamationId = id,
                    SousGarantie = false,
                    DureeGarantieMois = dureeMois
                };
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erreur lors de la vérification de garantie: {ex.Message}");
                return new WarrantyCheckResponse1
                {
                    ReclamationId = id,
                    SousGarantie = false,
                    DureeGarantieMois = dureeMois
                };
            }
        }

        public async Task<List<ReclamationResponse>> GetReclamationsByStatutAsync(string statut)
        {
            try
            {
                var response = await _httpClient.GetAsync($"/gateway/reclamations/statut/{statut}");
                response.EnsureSuccessStatusCode();
                return await response.Content.ReadFromJsonAsync<List<ReclamationResponse>>() ?? new List<ReclamationResponse>();
            }
            catch
            {
                return new List<ReclamationResponse>();
            }
        }

        public async Task<List<ReclamationResponse>> SearchReclamationsAsync(string term)
        {
            try
            {
                var response = await _httpClient.GetAsync($"/gateway/reclamations/search?term={Uri.EscapeDataString(term)}");
                response.EnsureSuccessStatusCode();
                return await response.Content.ReadFromJsonAsync<List<ReclamationResponse>>() ?? new List<ReclamationResponse>();
            }
            catch
            {
                return new List<ReclamationResponse>();
            }
        }

        public async Task<ReclamationStats> GetReclamationStatsAsync()
        {
            try
            {
                // Calculer les statistiques côté client pour l'instant
                var reclamations = await GetAllReclamationsAsync();

                var aujourdhui = DateTime.Today;
                var debutSemaine = aujourdhui.AddDays(-(int)aujourdhui.DayOfWeek);

                return new ReclamationStats
                {
                    TotalReclamations = reclamations.Count,
                    NonTraitees = reclamations.Count(r => r.Statut == "NonTraitee"),
                    EnCours = reclamations.Count(r => r.Statut == "EnCours"),
                    Traitees = reclamations.Count(r => r.Statut == "Traitee"),
                    Annulees = reclamations.Count(r => r.Statut == "Annulee"),
                    AvecResponsable = reclamations.Count(r => !string.IsNullOrEmpty(r.ResponsableSAVId)),
                    SousGarantie = reclamations.Count(r => r.SousGarantie),
                    HorsGarantie = reclamations.Count(r => !r.SousGarantie),
                    Aujourdhui = reclamations.Count(r => r.DateCreation.Date == aujourdhui),
                    CetteSemaine = reclamations.Count(r => r.DateCreation >= debutSemaine),
                    TotalMontantFacture = reclamations.Where(r => r.MontantFacture.HasValue).Sum(r => r.MontantFacture ?? 0),
                    MoyenneJours = reclamations.Any() ?
                        reclamations.Average(r => (DateTime.Now - r.DateCreation).TotalDays) : 0
                };
            }
            catch
            {
                return new ReclamationStats();
            }
        }
    }
}