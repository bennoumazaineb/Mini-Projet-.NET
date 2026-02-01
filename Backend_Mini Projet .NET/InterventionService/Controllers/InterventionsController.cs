using InterventionService.Helpers;
using InterventionService.Models.DTOs;
using InterventionService.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace InterventionService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
 
    public class InterventionsController : ControllerBase
    {
        private readonly IInterventionService _interventionService;
        private readonly ILogger<InterventionsController> _logger;

        public InterventionsController(
            IInterventionService interventionService,
            ILogger<InterventionsController> logger)
        {
            _interventionService = interventionService;
            _logger = logger;
        }

        /// <summary>
        /// Récupère toutes les interventions
        /// </summary>
        [HttpGet]
      
        public async Task<IActionResult> GetAll()
        {
            try
            {
                var interventions = await _interventionService.GetAllInterventionsAsync();
                return Ok(ApiResponse<IEnumerable<InterventionDTO>>.CreateSuccess(interventions));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erreur lors de la récupération de toutes les interventions");
                return StatusCode(500, ApiResponse<string>.CreateError("Erreur interne du serveur"));
            }
        }

        /// <summary>
        /// Récupère une intervention par son ID
        /// </summary>
        [HttpGet("{id}")]
       
        public async Task<IActionResult> GetById(int id)
        {
            try
            {
                var intervention = await _interventionService.GetInterventionByIdAsync(id);
                if (intervention == null)
                    return NotFound(ApiResponse<string>.CreateError($"Intervention avec ID {id} non trouvée"));

                return Ok(ApiResponse<InterventionDTO>.CreateSuccess(intervention));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erreur lors de la récupération de l'intervention {Id}", id);
                return StatusCode(500, ApiResponse<string>.CreateError("Erreur interne du serveur"));
            }
        }

        /// <summary>
        /// Récupère les interventions par ID de réclamation
        /// </summary>
        [HttpGet("reclamation/{reclamationId}")]
      
        public async Task<IActionResult> GetByReclamationId(int reclamationId)
        {
            try
            {
                var interventions = await _interventionService.GetInterventionsByReclamationIdAsync(reclamationId);
                return Ok(ApiResponse<IEnumerable<InterventionDTO>>.CreateSuccess(interventions));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erreur lors de la récupération des interventions pour la réclamation {ReclamationId}", reclamationId);
                return StatusCode(500, ApiResponse<string>.CreateError("Erreur interne du serveur"));
            }
        }

        /// <summary>
        /// Récupère les interventions par ID de technicien
        /// </summary>
        [HttpGet("technician/{technicianId}")]
        
        public async Task<IActionResult> GetByTechnicianId(int technicianId)
        {
            try
            {
                var interventions = await _interventionService.GetInterventionsByTechnicianIdAsync(technicianId);
                return Ok(ApiResponse<IEnumerable<InterventionDTO>>.CreateSuccess(interventions));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erreur lors de la récupération des interventions pour le technicien {TechnicianId}", technicianId);
                return StatusCode(500, ApiResponse<string>.CreateError("Erreur interne du serveur"));
            }
        }

        /// <summary>
        /// Crée une nouvelle intervention
        /// </summary>
        [HttpPost]
       
        public async Task<IActionResult> Create([FromBody] CreateInterventionDTO createDto)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ApiResponse<string>.CreateError("Données invalides", ModelState));

                var intervention = await _interventionService.CreateInterventionAsync(createDto);
                return CreatedAtAction(nameof(GetById), new { id = intervention.Id },
                    ApiResponse<InterventionDTO>.CreateSuccess(intervention, "Intervention créée avec succès"));
            }
            catch (ValidationException ex)
            {
                _logger.LogWarning(ex, "Validation échouée lors de la création d'une intervention");
                return BadRequest(ApiResponse<string>.CreateError(ex.Message));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erreur lors de la création de l'intervention");
                return StatusCode(500, ApiResponse<string>.CreateError("Erreur lors de la création de l'intervention"));
            }
        }

        /// <summary>
        /// Met à jour une intervention existante
        /// </summary>
        [HttpPut("{id}")]
      
        public async Task<IActionResult> Update(int id, [FromBody] UpdateInterventionDTO updateDto)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ApiResponse<string>.CreateError("Données invalides", ModelState));

                var intervention = await _interventionService.UpdateInterventionAsync(id, updateDto);
                if (intervention == null)
                    return NotFound(ApiResponse<string>.CreateError($"Intervention avec ID {id} non trouvée"));

                return Ok(ApiResponse<InterventionDTO>.CreateSuccess(intervention, "Intervention mise à jour avec succès"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erreur lors de la mise à jour de l'intervention {Id}", id);
                return StatusCode(500, ApiResponse<string>.CreateError("Erreur lors de la mise à jour de l'intervention"));
            }
        }

        /// <summary>
        /// Supprime une intervention
        /// </summary>
        [HttpDelete("{id}")]
     
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var result = await _interventionService.DeleteInterventionAsync(id);
                if (!result)
                    return NotFound(ApiResponse<string>.CreateError($"Intervention avec ID {id} non trouvée"));

                return Ok(ApiResponse<string>.CreateSuccess(null, "Intervention supprimée avec succès"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erreur lors de la suppression de l'intervention {Id}", id);
                return StatusCode(500, ApiResponse<string>.CreateError("Erreur lors de la suppression de l'intervention"));
            }
        }

        /// <summary>
        /// Met à jour le statut d'une intervention
        /// </summary>
        [HttpPatch("{id}/status")]
    
        public async Task<IActionResult> UpdateStatus(int id, [FromBody] UpdateStatusRequest request)
        {
            try
            {
                var intervention = await _interventionService.UpdateInterventionStatusAsync(id, request.Status);
                return Ok(ApiResponse<InterventionDTO>.CreateSuccess(intervention, "Statut mis à jour avec succès"));
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ApiResponse<string>.CreateError(ex.Message));
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ApiResponse<string>.CreateError(ex.Message));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erreur lors de la mise à jour du statut de l'intervention {Id}", id);
                return StatusCode(500, ApiResponse<string>.CreateError("Erreur lors de la mise à jour du statut"));
            }
        }

        /// <summary>
        /// Récupère les statistiques du tableau de bord
        /// </summary>
        [HttpGet("dashboard/stats")]
       
        public async Task<IActionResult> GetDashboardStats()
        {
            try
            {
                var stats = await _interventionService.GetDashboardStatsAsync();
                return Ok(ApiResponse<DashboardStatsDTO>.CreateSuccess(stats));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erreur lors de la récupération des statistiques du dashboard");
                return StatusCode(500, ApiResponse<string>.CreateError("Erreur lors de la récupération des statistiques"));
            }
        }

        /// <summary>
        /// Récupère les interventions par plage de dates
        /// </summary>
        [HttpGet("date-range")]
      
        public async Task<IActionResult> GetByDateRange(
            [FromQuery] DateTime startDate,
            [FromQuery] DateTime endDate)
        {
            try
            {
                var interventions = await _interventionService.GetInterventionsByDateRangeAsync(startDate, endDate);
                return Ok(ApiResponse<IEnumerable<InterventionDTO>>.CreateSuccess(interventions));
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ApiResponse<string>.CreateError(ex.Message));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erreur lors de la récupération des interventions par plage de dates");
                return StatusCode(500, ApiResponse<string>.CreateError("Erreur interne du serveur"));
            }
        }

        /// <summary>
        /// Récupère les interventions par statut
        /// </summary>
        [HttpGet("status/{status}")]
       
        public async Task<IActionResult> GetByStatus(string status)
        {
            try
            {
                var interventions = await _interventionService.GetInterventionsByStatusAsync(status);
                return Ok(ApiResponse<IEnumerable<InterventionDTO>>.CreateSuccess(interventions));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erreur lors de la récupération des interventions avec statut {Status}", status);
                return StatusCode(500, ApiResponse<string>.CreateError("Erreur interne du serveur"));
            }
        }
    }

    public class UpdateStatusRequest
    {
        [Required]
        [StringLength(50)]
        public string Status { get; set; } = string.Empty;
    }
}