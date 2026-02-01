using Microservice2_Reclamations.Models;

public interface IReclamationService
{
    Task<ReclamationResponse> CreateReclamationAsync(CreateReclamationModel model, string authToken);
    Task<List<ReclamationResponse>> GetReclamationsByClientAsync(string clientId, string authToken);
    Task<ReclamationResponse?> GetReclamationByIdForClientAsync(int id, string clientId, string authToken);
    Task<List<ReclamationResponse>> GetAllReclamationsAsync();
    Task<List<ReclamationResponse>> GetReclamationsByStatutAsync(StatutReclamation statut);
    Task<ReclamationResponse?> GetReclamationByIdAsync(int id);
    Task<ReclamationResponse> UpdateReclamationAsync(int id, UpdateReclamationModel model);
    Task<bool> DeleteReclamationAsync(int id);
    Task<List<ReclamationResponse>> SearchReclamationsAsync(string searchTerm);
    Task<bool> AssignerResponsableAsync(int reclamationId, string responsableId, string responsableNom);
    Task<Dictionary<StatutReclamation, int>> GetStatistiquesAsync();
    Task<bool> VerifierGarantieAsync(int reclamationId, int dureeGarantieMois = 24);
    Task<bool> UpdateStatutAsync(int reclamationId, StatutReclamation nouveauStatut, string? solution = null);
}
