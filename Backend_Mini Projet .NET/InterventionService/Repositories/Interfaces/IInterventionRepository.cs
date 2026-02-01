using InterventionService.Models.Entities;
using System.Linq.Expressions;

namespace InterventionService.Repositories.Interfaces
{
    public interface IInterventionRepository
    {
        Task<IEnumerable<Intervention>> GetAllAsync();
        Task<Intervention?> GetByIdAsync(int id);
        Task<IEnumerable<Intervention>> GetByReclamationIdAsync(int reclamationId);
        Task<IEnumerable<Intervention>> GetByTechnicianIdAsync(int technicianId);
        Task<IEnumerable<Intervention>> GetByStatusAsync(string status);
        Task<IEnumerable<Intervention>> GetByDateRangeAsync(DateTime startDate, DateTime endDate);
        Task<IEnumerable<Intervention>> GetUnderWarrantyAsync(bool isUnderWarranty);
        Task<Intervention> CreateAsync(Intervention intervention);
        Task<Intervention?> UpdateAsync(int id, Intervention intervention);
        Task<bool> DeleteAsync(int id);
        Task<bool> ExistsAsync(int id);
        Task<int> GetCountByStatusAsync(string status);
        Task<decimal> GetTotalCostByMonthAsync(int year, int month);
    }
}