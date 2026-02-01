using ReclamationService.Models.DTOs;

namespace ReclamationService.Services
{
    public interface IReclamationService
    {
        // === Clients ===
        Task<ReclamationDTO> CreateReclamationAsync(CreateReclamationDTO dto, string currentUserId);
        Task<IEnumerable<ReclamationDTO>> GetMyReclamationsAsync(string clientUserId);
        Task<ReclamationDTO> GetMyReclamationByIdAsync(int id, string clientUserId);
        Task<ReclamationDTO> UpdateMyReclamationAsync(int id, UpdateReclamationDTO dto, string clientUserId);

        // === Responsables SAV ===
        Task<IEnumerable<ReclamationDTO>> GetAllReclamationsAsync();
        Task<ReclamationDTO> GetReclamationByIdAsync(int id);
        Task<IEnumerable<ReclamationDTO>> GetReclamationsByClientAsync(string clientUserId);
        Task<ReclamationDTO> UpdateReclamationAsync(int id, UpdateReclamationDTO dto, string updatedByUserId);
        Task<ReclamationDTO> UpdateStatutReclamationAsync(int id, string statut, string updatedByUserId);
        Task<IEnumerable<ReclamationDTO>> GetReclamationsByStatutAsync(string statut);
        Task<bool> DeleteReclamationAsync(int id);
        Task<object> GetStatistiquesAsync();
    }
}
