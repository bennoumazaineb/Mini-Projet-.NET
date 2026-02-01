using InterventionService.Data;
using InterventionService.Models.Entities;
using InterventionService.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace InterventionService.Repositories
{
    public class InterventionRepository : IInterventionRepository
    {
        private readonly InterventionDbContext _context;
        private readonly ILogger<InterventionRepository> _logger;

        public InterventionRepository(InterventionDbContext context, ILogger<InterventionRepository> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<IEnumerable<Intervention>> GetAllAsync()
        {
            try
            {
                return await _context.Interventions
                    .OrderByDescending(i => i.InterventionDate)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erreur lors de la récupération de toutes les interventions");
                throw;
            }
        }

        public async Task<Intervention?> GetByIdAsync(int id)
        {
            try
            {
                return await _context.Interventions.FindAsync(id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erreur lors de la récupération de l'intervention {Id}", id);
                throw;
            }
        }

        public async Task<IEnumerable<Intervention>> GetByReclamationIdAsync(int reclamationId)
        {
            try
            {
                return await _context.Interventions
                    .Where(i => i.ReclamationId == reclamationId)
                    .OrderByDescending(i => i.CreatedAt)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erreur lors de la récupération des interventions pour la réclamation {ReclamationId}", reclamationId);
                throw;
            }
        }

        public async Task<IEnumerable<Intervention>> GetByTechnicianIdAsync(int technicianId)
        {
            try
            {
                return await _context.Interventions
                    .Where(i => i.TechnicianId == technicianId)
                    .OrderByDescending(i => i.InterventionDate)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erreur lors de la récupération des interventions pour le technicien {TechnicianId}", technicianId);
                throw;
            }
        }

        public async Task<IEnumerable<Intervention>> GetByStatusAsync(string status)
        {
            try
            {
                return await _context.Interventions
                    .Where(i => i.Status == status)
                    .OrderBy(i => i.InterventionDate)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erreur lors de la récupération des interventions avec le statut {Status}", status);
                throw;
            }
        }

        public async Task<IEnumerable<Intervention>> GetByDateRangeAsync(DateTime startDate, DateTime endDate)
        {
            try
            {
                return await _context.Interventions
                    .Where(i => i.InterventionDate >= startDate && i.InterventionDate <= endDate)
                    .OrderBy(i => i.InterventionDate)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erreur lors de la récupération des interventions entre {StartDate} et {EndDate}", startDate, endDate);
                throw;
            }
        }

        public async Task<IEnumerable<Intervention>> GetUnderWarrantyAsync(bool isUnderWarranty)
        {
            try
            {
                return await _context.Interventions
                    .Where(i => i.IsUnderWarranty == isUnderWarranty)
                    .OrderByDescending(i => i.CreatedAt)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erreur lors de la récupération des interventions sous garantie: {IsUnderWarranty}", isUnderWarranty);
                throw;
            }
        }

        public async Task<Intervention> CreateAsync(Intervention intervention)
        {
            try
            {
                _context.Interventions.Add(intervention);
                await _context.SaveChangesAsync();
                _logger.LogInformation("Intervention créée avec ID: {Id}", intervention.Id);
                return intervention;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erreur lors de la création de l'intervention");
                throw;
            }
        }

        public async Task<Intervention?> UpdateAsync(int id, Intervention intervention)
        {
            try
            {
                var existingIntervention = await _context.Interventions.FindAsync(id);
                if (existingIntervention == null)
                    return null;

                // Mise à jour des propriétés
                _context.Entry(existingIntervention).CurrentValues.SetValues(intervention);
                existingIntervention.UpdatedAt = DateTime.UtcNow;

                // Si statut = Terminée, mettre à jour CompletedAt
                if (intervention.Status == "Terminée" && existingIntervention.CompletedAt == null)
                {
                    existingIntervention.CompletedAt = DateTime.UtcNow;
                }

                await _context.SaveChangesAsync();
                _logger.LogInformation("Intervention {Id} mise à jour", id);
                return existingIntervention;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erreur lors de la mise à jour de l'intervention {Id}", id);
                throw;
            }
        }

        public async Task<bool> DeleteAsync(int id)
        {
            try
            {
                var intervention = await _context.Interventions.FindAsync(id);
                if (intervention == null)
                    return false;

                _context.Interventions.Remove(intervention);
                await _context.SaveChangesAsync();
                _logger.LogInformation("Intervention {Id} supprimée", id);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erreur lors de la suppression de l'intervention {Id}", id);
                throw;
            }
        }

        public async Task<bool> ExistsAsync(int id)
        {
            try
            {
                return await _context.Interventions.AnyAsync(i => i.Id == id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erreur lors de la vérification de l'existence de l'intervention {Id}", id);
                throw;
            }
        }

        public async Task<int> GetCountByStatusAsync(string status)
        {
            try
            {
                return await _context.Interventions.CountAsync(i => i.Status == status);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erreur lors du comptage des interventions avec statut {Status}", status);
                throw;
            }
        }

        public async Task<decimal> GetTotalCostByMonthAsync(int year, int month)
        {
            try
            {
                var interventions = await _context.Interventions
                    .Where(i => i.CreatedAt.Year == year && i.CreatedAt.Month == month && !i.IsUnderWarranty)
                    .ToListAsync();

                return interventions.Sum(i => (i.PartsCost ?? 0) + (i.LaborCost ?? 0));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erreur lors du calcul du coût total pour {Year}-{Month}", year, month);
                throw;
            }
        }
    }
}