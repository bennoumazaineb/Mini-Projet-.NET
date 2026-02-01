using AutoMapper;
using InterventionService.Models.DTOs;
using InterventionService.Models.Entities;
using InterventionService.Repositories.Interfaces;
using InterventionService.Services.Interfaces;
using Microsoft.Extensions.Logging;

namespace InterventionService.Services
{
    public class InterventionService : IInterventionService
    {
        private readonly IInterventionRepository _repository;
        private readonly IMapper _mapper;
        private readonly ILogger<InterventionService> _logger;

        public InterventionService(
            IInterventionRepository repository,
            IMapper mapper,
            ILogger<InterventionService> logger)
        {
            _repository = repository;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<IEnumerable<InterventionDTO>> GetAllInterventionsAsync()
        {
            try
            {
                var interventions = await _repository.GetAllAsync();
                return _mapper.Map<IEnumerable<InterventionDTO>>(interventions);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erreur dans GetAllInterventionsAsync");
                throw;
            }
        }

        public async Task<InterventionDTO?> GetInterventionByIdAsync(int id)
        {
            try
            {
                var intervention = await _repository.GetByIdAsync(id);
                return intervention == null ? null : _mapper.Map<InterventionDTO>(intervention);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erreur dans GetInterventionByIdAsync pour l'ID {Id}", id);
                throw;
            }
        }

        public async Task<IEnumerable<InterventionDTO>> GetInterventionsByReclamationIdAsync(int reclamationId)
        {
            try
            {
                var interventions = await _repository.GetByReclamationIdAsync(reclamationId);
                return _mapper.Map<IEnumerable<InterventionDTO>>(interventions);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erreur dans GetInterventionsByReclamationIdAsync pour {ReclamationId}", reclamationId);
                throw;
            }
        }

        public async Task<IEnumerable<InterventionDTO>> GetInterventionsByTechnicianIdAsync(int technicianId)
        {
            try
            {
                var interventions = await _repository.GetByTechnicianIdAsync(technicianId);
                return _mapper.Map<IEnumerable<InterventionDTO>>(interventions);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erreur dans GetInterventionsByTechnicianIdAsync pour {TechnicianId}", technicianId);
                throw;
            }
        }

        public async Task<IEnumerable<InterventionDTO>> GetInterventionsByStatusAsync(string status)
        {
            try
            {
                var interventions = await _repository.GetByStatusAsync(status);
                return _mapper.Map<IEnumerable<InterventionDTO>>(interventions);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erreur dans GetInterventionsByStatusAsync pour le statut {Status}", status);
                throw;
            }
        }

        public async Task<InterventionDTO> CreateInterventionAsync(CreateInterventionDTO createDto)
        {
            try
            {
                // Validation métier supplémentaire
                if (!createDto.IsUnderWarranty && (!createDto.PartsCost.HasValue || !createDto.LaborCost.HasValue))
                {
                    throw new InvalidOperationException("Pour une intervention hors garantie, les coûts (pièces et main d'œuvre) sont requis.");
                }

                if (createDto.IsUnderWarranty && (createDto.PartsCost.HasValue || createDto.LaborCost.HasValue))
                {
                    createDto.PartsCost = null;
                    createDto.LaborCost = null;
                }

                var intervention = _mapper.Map<Intervention>(createDto);
                intervention.Status = "Planifiée";
                intervention.CreatedAt = DateTime.UtcNow;

                var created = await _repository.CreateAsync(intervention);
                return _mapper.Map<InterventionDTO>(created);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erreur dans CreateInterventionAsync");
                throw;
            }
        }

        public async Task<InterventionDTO?> UpdateInterventionAsync(int id, UpdateInterventionDTO updateDto)
        {
            try
            {
                var existing = await _repository.GetByIdAsync(id);
                if (existing == null)
                    return null;

                // Mise à jour des propriétés
                _mapper.Map(updateDto, existing);
                existing.UpdatedAt = DateTime.UtcNow;

                // Si changement de statut à "Terminée"
                if (updateDto.Status == "Terminée" && existing.Status != "Terminée")
                {
                    existing.CompletedAt = DateTime.UtcNow;
                }

                var updated = await _repository.UpdateAsync(id, existing);
                return updated == null ? null : _mapper.Map<InterventionDTO>(updated);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erreur dans UpdateInterventionAsync pour l'ID {Id}", id);
                throw;
            }
        }

        public async Task<bool> DeleteInterventionAsync(int id)
        {
            try
            {
                return await _repository.DeleteAsync(id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erreur dans DeleteInterventionAsync pour l'ID {Id}", id);
                throw;
            }
        }

        public async Task<InterventionDTO> UpdateInterventionStatusAsync(int id, string status)
        {
            try
            {
                var allowedStatuses = new[] { "Planifiée", "EnCours", "Terminée", "Annulée" };
                if (!allowedStatuses.Contains(status))
                {
                    throw new ArgumentException($"Statut invalide. Les statuts autorisés sont: {string.Join(", ", allowedStatuses)}");
                }

                var existing = await _repository.GetByIdAsync(id);
                if (existing == null)
                    throw new KeyNotFoundException($"Intervention avec ID {id} non trouvée");

                existing.Status = status;
                existing.UpdatedAt = DateTime.UtcNow;

                if (status == "Terminée" && existing.CompletedAt == null)
                {
                    existing.CompletedAt = DateTime.UtcNow;
                }

                var updated = await _repository.UpdateAsync(id, existing);
                if (updated == null)
                    throw new InvalidOperationException("Échec de la mise à jour du statut");

                return _mapper.Map<InterventionDTO>(updated);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erreur dans UpdateInterventionStatusAsync pour l'ID {Id}, Statut {Status}", id, status);
                throw;
            }
        }

        public async Task<IEnumerable<InterventionDTO>> GetInterventionsByDateRangeAsync(DateTime startDate, DateTime endDate)
        {
            try
            {
                if (startDate > endDate)
                    throw new ArgumentException("La date de début doit être antérieure à la date de fin");

                var interventions = await _repository.GetByDateRangeAsync(startDate, endDate);
                return _mapper.Map<IEnumerable<InterventionDTO>>(interventions);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erreur dans GetInterventionsByDateRangeAsync");
                throw;
            }
        }

        public async Task<DashboardStatsDTO> GetDashboardStatsAsync()
        {
            try
            {
                var allInterventions = await _repository.GetAllAsync();
                var interventionsList = allInterventions.ToList();

                var stats = new DashboardStatsDTO
                {
                    TotalInterventions = interventionsList.Count,
                    PlannedCount = interventionsList.Count(i => i.Status == "Planifiée"),
                    InProgressCount = interventionsList.Count(i => i.Status == "EnCours"),
                    CompletedCount = interventionsList.Count(i => i.Status == "Terminée"),
                    CancelledCount = interventionsList.Count(i => i.Status == "Annulée"),
                    UnderWarrantyCount = interventionsList.Count(i => i.IsUnderWarranty),
                    TotalRevenue = interventionsList
                        .Where(i => !i.IsUnderWarranty)
                        .Sum(i => (i.PartsCost ?? 0) + (i.LaborCost ?? 0))
                };

                // Statistiques par mois (6 derniers mois)
                var sixMonthsAgo = DateTime.UtcNow.AddMonths(-6);
                var recentInterventions = interventionsList
                    .Where(i => i.CreatedAt >= sixMonthsAgo)
                    .GroupBy(i => new { i.CreatedAt.Year, i.CreatedAt.Month })
                    .OrderBy(g => g.Key.Year)
                    .ThenBy(g => g.Key.Month);

                foreach (var group in recentInterventions)
                {
                    var monthKey = $"{group.Key.Year}-{group.Key.Month:D2}";
                    stats.InterventionsByMonth[monthKey] = group.Count();
                }

                return stats;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erreur dans GetDashboardStatsAsync");
                throw;
            }
        }
    }
}