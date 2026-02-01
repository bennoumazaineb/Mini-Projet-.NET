using SAV.InterventionsAPI.Models;

namespace SAV.InterventionsAPI.Services
{
    public interface IInterventionService
    {
        Task<List<Intervention>> GetAllInterventionsAsync();
        Task<Intervention?> GetInterventionByIdAsync(Guid id);
        Task<Intervention?> GetInterventionByNumeroAsync(string numero);
        Task<Intervention> CreateInterventionAsync(CreateInterventionRequest request, string createdBy);
        Task<bool> UpdateInterventionAsync(Guid id, UpdateInterventionRequest request);
        Task<bool> DemarrerInterventionAsync(Guid id);
        Task<bool> TerminerInterventionAsync(Guid id, string rapport);
        Task<bool> AnnulerInterventionAsync(Guid id);
        Task<FactureResponse> CalculerFactureAsync(Guid id, CalculFactureRequest request);
        Task<bool> GenererFactureAsync(Guid id, CalculFactureRequest request);
        Task<bool> MarquerFacturePayeeAsync(Guid id);
        Task<List<Intervention>> GetInterventionsByReclamationAsync(Guid reclamationId);
        Task<List<Intervention>> GetInterventionsByStatutAsync(string statut);
    }
}