using BlazorApp1.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BlazorApp1.Services
{
    public interface IInterventionService
    {
        Task<List<InterventionDto>> GetAllInterventionsAsync();
        Task<InterventionDto?> GetInterventionByIdAsync(int id);
        Task<List<InterventionDto>> GetInterventionsByReclamationIdAsync(int reclamationId);
        Task<List<InterventionDto>> GetInterventionsByTechnicianIdAsync(int technicianId);
        Task<List<InterventionDto>> GetInterventionsByDateRangeAsync(DateTime startDate, DateTime endDate);
        Task<List<InterventionDto>> GetInterventionsByStatusAsync(string status);
        Task<InterventionDto?> CreateInterventionAsync(CreateInterventionDto dto);
        Task<InterventionDto?> UpdateInterventionAsync(int id, UpdateInterventionDto dto);
        Task<bool> DeleteInterventionAsync(int id);
        Task<InterventionDto?> UpdateInterventionStatusAsync(int id, string status);
        Task<DashboardStatsDto> GetDashboardStatsAsync();
        Task<List<TechnicianDto>> GetAllTechniciansAsync();
        Task<List<string>> GetAvailableStatusesAsync();
    }
}