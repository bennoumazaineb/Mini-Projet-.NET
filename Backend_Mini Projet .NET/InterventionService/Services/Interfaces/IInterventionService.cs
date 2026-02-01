using InterventionService.Models.DTOs;

namespace InterventionService.Services.Interfaces
{
    public interface IInterventionService
    {
        Task<IEnumerable<InterventionDTO>> GetAllInterventionsAsync();
        Task<InterventionDTO?> GetInterventionByIdAsync(int id);
        Task<IEnumerable<InterventionDTO>> GetInterventionsByReclamationIdAsync(int reclamationId);
        Task<IEnumerable<InterventionDTO>> GetInterventionsByTechnicianIdAsync(int technicianId);
        Task<IEnumerable<InterventionDTO>> GetInterventionsByStatusAsync(string status);
        Task<InterventionDTO> CreateInterventionAsync(CreateInterventionDTO createDto);
        Task<InterventionDTO?> UpdateInterventionAsync(int id, UpdateInterventionDTO updateDto);
        Task<bool> DeleteInterventionAsync(int id);
        Task<InterventionDTO> UpdateInterventionStatusAsync(int id, string status);
        Task<IEnumerable<InterventionDTO>> GetInterventionsByDateRangeAsync(DateTime startDate, DateTime endDate);
        Task<DashboardStatsDTO> GetDashboardStatsAsync();
    }

    public class DashboardStatsDTO
    {
        public int TotalInterventions { get; set; }
        public int PlannedCount { get; set; }
        public int InProgressCount { get; set; }
        public int CompletedCount { get; set; }
        public int CancelledCount { get; set; }
        public int UnderWarrantyCount { get; set; }
        public decimal TotalRevenue { get; set; }
        public Dictionary<string, int> InterventionsByMonth { get; set; } = new();
    }
}