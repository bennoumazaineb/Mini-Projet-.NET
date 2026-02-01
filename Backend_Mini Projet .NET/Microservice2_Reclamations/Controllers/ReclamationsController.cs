using Microservice2_Reclamations.Models;
using Microservice2_Reclamations.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;

namespace Microservice2_Reclamations.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
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

        // === ENDPOINTS CLIENT ===

        // POST: api/reclamations
        [HttpPost]
        public async Task<IActionResult> CreateReclamation([FromBody] CreateReclamationModel model)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                var reclamation = await _reclamationService.CreateReclamationAsync(model, null);
                return CreatedAtAction(nameof(GetReclamationById), new { id = reclamation.Id }, reclamation);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erreur création réclamation");
                return StatusCode(500, "Erreur interne");
            }
        }

        // GET: api/reclamations/mes-reclamations
        [HttpGet("mes-reclamations")]
        public async Task<IActionResult> GetMesReclamations([FromQuery] string clientId)
        {
            try
            {
                var reclamations = await _reclamationService.GetReclamationsByClientAsync(clientId, null);
                return Ok(reclamations);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erreur récupération réclamations client");
                return StatusCode(500, "Erreur interne");
            }
        }

        // GET: api/reclamations/mes-reclamations/{id}
        [HttpGet("mes-reclamations/{id}")]
        public async Task<IActionResult> GetMaReclamation(int id, [FromQuery] string clientId)
        {
            try
            {
                var reclamation = await _reclamationService.GetReclamationByIdForClientAsync(id, clientId, null);

                if (reclamation == null)
                    return NotFound("Réclamation non trouvée");

                return Ok(reclamation);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erreur récupération réclamation");
                return StatusCode(500, "Erreur interne");
            }
        }

        // === ENDPOINTS RESPONSABLE SAV ===

        // GET: api/reclamations
        [HttpGet]
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

        // GET: api/reclamations/{id}
        [HttpGet("{id}")]
        public async Task<IActionResult> GetReclamationById(int id)
        {
            try
            {
                var reclamation = await _reclamationService.GetReclamationByIdAsync(id);

                if (reclamation == null)
                    return NotFound("Réclamation non trouvée");

                return Ok(reclamation);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erreur récupération réclamation");
                return StatusCode(500, "Erreur interne");
            }
        }

        // GET: api/reclamations/statut/{statut}
        [HttpGet("statut/{statut}")]
        public async Task<IActionResult> GetReclamationsByStatut(StatutReclamation statut)
        {
            try
            {
                var reclamations = await _reclamationService.GetReclamationsByStatutAsync(statut);
                return Ok(reclamations);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erreur récupération par statut");
                return StatusCode(500, "Erreur interne");
            }
        }

        // PUT: api/reclamations/{id}
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateReclamation(int id, [FromBody] UpdateReclamationModel model)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                var reclamation = await _reclamationService.UpdateReclamationAsync(id, model);
                return Ok(reclamation);
            }
            catch (KeyNotFoundException)
            {
                return NotFound("Réclamation non trouvée");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erreur mise à jour");
                return StatusCode(500, "Erreur interne");
            }
        }

        // DELETE: api/reclamations/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteReclamation(int id)
        {
            try
            {
                var result = await _reclamationService.DeleteReclamationAsync(id);

                if (!result)
                    return NotFound("Réclamation non trouvée");

                return Ok("Réclamation supprimée");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erreur suppression");
                return StatusCode(500, "Erreur interne");
            }
        }

        // POST: api/reclamations/{id}/assigner
        [HttpPost("{id}/assigner")]
        public async Task<IActionResult> AssignerResponsable(int id, [FromBody] AssignResponsableModel model)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                var success = await _reclamationService.AssignerResponsableAsync(id, model.ResponsableId, model.ResponsableNom);

                if (!success)
                    return NotFound("Réclamation non trouvée");

                return Ok("Responsable assigné");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erreur assignation responsable");
                return StatusCode(500, "Erreur interne");
            }
        }

        // PATCH: api/reclamations/{id}/statut
        [HttpPatch("{id}/statut")]
        public async Task<IActionResult> UpdateStatut(int id, [FromBody] UpdateStatutModel model)
        {
            try
            {
                var success = await _reclamationService.UpdateStatutAsync(id, model.NouveauStatut, model.Solution);

                if (!success)
                    return NotFound("Réclamation non trouvée");

                return Ok("Statut mis à jour");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erreur mise à jour statut");
                return StatusCode(500, "Erreur interne");
            }
        }

        // GET: api/reclamations/{id}/garantie
        [HttpGet("{id}/garantie")]
        public async Task<IActionResult> VerifierGarantie(int id, [FromQuery] int dureeMois = 24)
        {
            try
            {
                var resultat = await _reclamationService.VerifierGarantieAsync(id, dureeMois);
                return Ok(new
                {
                    ReclamationId = id,
                    SousGarantie = resultat,
                    DureeGarantieMois = dureeMois
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erreur garantie");
                return StatusCode(500, "Erreur interne");
            }
        }

        // GET: api/reclamations/search
        [HttpGet("search")]
        public async Task<IActionResult> SearchReclamations([FromQuery] string term)
        {
            try
            {
                var reclamations = await _reclamationService.SearchReclamationsAsync(term);
                return Ok(reclamations);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erreur recherche");
                return StatusCode(500, "Erreur interne");
            }
        }

        // GET: api/reclamations/clients/{clientId}
        [HttpGet("clients/{clientId}")]
        public async Task<IActionResult> GetReclamationsByClientId(string clientId)
        {
            try
            {
                var reclamations = await _reclamationService.GetReclamationsByClientAsync(clientId, null);
                return Ok(reclamations);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erreur récupération client");
                return StatusCode(500, "Erreur interne");
            }
        }
    }

    public class AssignResponsableModel
    {
        [Required]
        public string ResponsableId { get; set; } = string.Empty;

        [Required]
        public string ResponsableNom { get; set; } = string.Empty;
    }

    public class UpdateStatutModel
    {
        [Required]
        public StatutReclamation NouveauStatut { get; set; }
        public string? Solution { get; set; }
    }
}
