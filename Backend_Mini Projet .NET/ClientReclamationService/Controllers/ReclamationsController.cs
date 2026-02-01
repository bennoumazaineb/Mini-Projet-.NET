using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ReclamationService.Models.DTOs;
using ReclamationService.Services;
using System.Security.Claims;

namespace ReclamationService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class ReclamationsController : ControllerBase
    {
        private readonly IReclamationService _reclamationService;
        private readonly ILogger<ReclamationsController> _logger;

        public ReclamationsController(
            IReclamationService reclamationService,
            ILogger<ReclamationsController> logger)
        {
            _reclamationService = reclamationService;
            _logger = logger;
        }

        // === ENDPOINTS CLIENTS ===

        [HttpPost("my-reclamations")]
        [Authorize(Roles = "Client")]
        public async Task<IActionResult> CreateMyReclamation([FromBody] CreateReclamationDTO dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                var userId = GetCurrentUserId();
                var reclamation = await _reclamationService.CreateReclamationAsync(dto, userId);

                return CreatedAtAction(nameof(GetMyReclamationById), new { id = reclamation.Id }, reclamation);
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erreur création réclamation");
                return StatusCode(500, "Erreur interne");
            }
        }

        [HttpGet("my-reclamations")]
        [Authorize(Roles = "Client")]
        public async Task<IActionResult> GetMyReclamations()
        {
            try
            {
                var userId = GetCurrentUserId();
                var reclamations = await _reclamationService.GetMyReclamationsAsync(userId);
                return Ok(reclamations);
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erreur récupération réclamations");
                return StatusCode(500, "Erreur interne");
            }
        }

        [HttpGet("my-reclamations/{id}")]
        [Authorize(Roles = "Client")]
        public async Task<IActionResult> GetMyReclamationById(int id)
        {
            try
            {
                var userId = GetCurrentUserId();
                var reclamation = await _reclamationService.GetMyReclamationByIdAsync(id, userId);
                return Ok(reclamation);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(ex.Message);
            }
        }

        [HttpPut("my-reclamations/{id}")]
        [Authorize(Roles = "Client")]
        public async Task<IActionResult> UpdateMyReclamation(int id, [FromBody] UpdateReclamationDTO dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                var userId = GetCurrentUserId();
                var reclamation = await _reclamationService.UpdateMyReclamationAsync(id, dto, userId);
                return Ok(reclamation);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(ex.Message);
            }
        }

        // === ENDPOINTS RESPONSABLES SAV ===

        [HttpGet]
        [Authorize(Roles = "ResponsableSAV")]
        public async Task<IActionResult> GetAllReclamations()
        {
            try
            {
                var reclamations = await _reclamationService.GetAllReclamationsAsync();
                return Ok(reclamations);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erreur récupération réclamations");
                return StatusCode(500, "Erreur interne");
            }
        }

        [HttpGet("{id}")]
        [Authorize(Roles = "ResponsableSAV")]
        public async Task<IActionResult> GetReclamationById(int id)
        {
            try
            {
                var reclamation = await _reclamationService.GetReclamationByIdAsync(id);
                return Ok(reclamation);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
        }

        [HttpGet("by-client/{clientUserId}")]
        [Authorize(Roles = "ResponsableSAV")]
        public async Task<IActionResult> GetReclamationsByClient(string clientUserId)
        {
            try
            {
                var reclamations = await _reclamationService.GetReclamationsByClientAsync(clientUserId);
                return Ok(reclamations);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "ResponsableSAV")]
        public async Task<IActionResult> UpdateReclamation(int id, [FromBody] UpdateReclamationDTO dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                var userId = GetCurrentUserId();
                var reclamation = await _reclamationService.UpdateReclamationAsync(id, dto, userId);
                return Ok(reclamation);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(ex.Message);
            }
        }

        [HttpPatch("{id}/statut")]
        [Authorize(Roles = "ResponsableSAV")]
        public async Task<IActionResult> UpdateStatutReclamation(int id, [FromBody] string statut)
        {
            try
            {
                var userId = GetCurrentUserId();
                var reclamation = await _reclamationService.UpdateStatutReclamationAsync(id, statut, userId);
                return Ok(reclamation);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(ex.Message);
            }
        }

        [HttpGet("statistiques")]
        [Authorize(Roles = "ResponsableSAV")]
        public async Task<IActionResult> GetStatistiques()
        {
            try
            {
                var stats = await _reclamationService.GetStatistiquesAsync();
                return Ok(stats);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erreur récupération statistiques");
                return StatusCode(500, "Erreur interne");
            }
        }

        // === MÉTHODE UTILITAIRE ===

        private string GetCurrentUserId()
        {
            return User.FindFirst("uid")?.Value ?? User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        }
    }
}