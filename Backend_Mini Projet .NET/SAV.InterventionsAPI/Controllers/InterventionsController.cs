using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SAV.InterventionsAPI.Models;
using SAV.InterventionsAPI.Services;
using System.Security.Claims;

namespace SAV.InterventionsAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Produces("application/json")]
    [Authorize]
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
        /// Récupérer toutes les interventions
        /// </summary>
        [HttpGet]
        [ProducesResponseType(typeof(ApiResponse<List<Intervention>>), 200)]
        [Authorize(Roles = "ResponsableSAV,Admin")]
        public async Task<IActionResult> GetAll()
        {
            try
            {
                var interventions = await _interventionService.GetAllInterventionsAsync();
                return Ok(ApiResponse<List<Intervention>>.SuccessResponse(interventions));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erreur lors de la récupération des interventions");
                return StatusCode(500, ApiResponse<object>.ErrorResponse("Erreur interne"));
            }
        }

        /// <summary>
        /// Récupérer une intervention par son ID
        /// </summary>
        [HttpGet("{id:guid}")]
        [ProducesResponseType(typeof(ApiResponse<Intervention>), 200)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> GetById(Guid id)
        {
            try
            {
                var intervention = await _interventionService.GetInterventionByIdAsync(id);
                if (intervention == null)
                    return NotFound(ApiResponse<object>.ErrorResponse("Intervention non trouvée"));

                return Ok(ApiResponse<Intervention>.SuccessResponse(intervention));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erreur lors de la récupération de l'intervention {Id}", id);
                return StatusCode(500, ApiResponse<object>.ErrorResponse("Erreur interne"));
            }
        }

        /// <summary>
        /// Récupérer une intervention par son numéro
        /// </summary>
        [HttpGet("numero/{numero}")]
        [ProducesResponseType(typeof(ApiResponse<Intervention>), 200)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> GetByNumero(string numero)
        {
            try
            {
                var intervention = await _interventionService.GetInterventionByNumeroAsync(numero);
                if (intervention == null)
                    return NotFound(ApiResponse<object>.ErrorResponse("Intervention non trouvée"));

                return Ok(ApiResponse<Intervention>.SuccessResponse(intervention));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erreur lors de la récupération de l'intervention {Numero}", numero);
                return StatusCode(500, ApiResponse<object>.ErrorResponse("Erreur interne"));
            }
        }

        /// <summary>
        /// Créer une nouvelle intervention
        /// </summary>
        [HttpPost]
        [ProducesResponseType(typeof(ApiResponse<Intervention>), 201)]
        [ProducesResponseType(400)]
        [Authorize(Roles = "ResponsableSAV,Admin")]
        public async Task<IActionResult> Create([FromBody] CreateInterventionRequest request)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ApiResponse<object>.ErrorResponse("Données invalides"));

                var createdBy = User.FindFirst(ClaimTypes.Email)?.Value ?? "system";
                var intervention = await _interventionService.CreateInterventionAsync(request, createdBy);

                return CreatedAtAction(nameof(GetById), new { id = intervention.Id },
                    ApiResponse<Intervention>.SuccessResponse(intervention, "Intervention créée"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erreur lors de la création de l'intervention");
                return BadRequest(ApiResponse<object>.ErrorResponse(ex.Message));
            }
        }

        /// <summary>
        /// Mettre à jour une intervention
        /// </summary>
        [HttpPut("{id:guid}")]
        [ProducesResponseType(typeof(ApiResponse<bool>), 200)]
        [ProducesResponseType(404)]
        [Authorize(Roles = "ResponsableSAV,Admin")]
        public async Task<IActionResult> Update(Guid id, [FromBody] UpdateInterventionRequest request)
        {
            try
            {
                var success = await _interventionService.UpdateInterventionAsync(id, request);
                if (!success)
                    return NotFound(ApiResponse<object>.ErrorResponse("Intervention non trouvée"));

                return Ok(ApiResponse<bool>.SuccessResponse(true, "Intervention mise à jour"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erreur lors de la mise à jour de l'intervention {Id}", id);
                return BadRequest(ApiResponse<object>.ErrorResponse(ex.Message));
            }
        }

        /// <summary>
        /// Démarrer une intervention
        /// </summary>
        [HttpPost("{id:guid}/demarrer")]
        [ProducesResponseType(typeof(ApiResponse<bool>), 200)]
        [ProducesResponseType(400)]
        [Authorize(Roles = "ResponsableSAV,Admin")]
        public async Task<IActionResult> Demarrer(Guid id)
        {
            try
            {
                var success = await _interventionService.DemarrerInterventionAsync(id);
                if (!success)
                    return NotFound(ApiResponse<object>.ErrorResponse("Intervention non trouvée"));

                return Ok(ApiResponse<bool>.SuccessResponse(true, "Intervention démarrée"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erreur lors du démarrage de l'intervention {Id}", id);
                return BadRequest(ApiResponse<object>.ErrorResponse(ex.Message));
            }
        }

        /// <summary>
        /// Terminer une intervention
        /// </summary>
        [HttpPost("{id:guid}/terminer")]
        [ProducesResponseType(typeof(ApiResponse<bool>), 200)]
        [ProducesResponseType(400)]
        [Authorize(Roles = "ResponsableSAV,Admin")]
        public async Task<IActionResult> Terminer(Guid id, [FromBody] string rapport)
        {
            try
            {
                var success = await _interventionService.TerminerInterventionAsync(id, rapport);
                if (!success)
                    return NotFound(ApiResponse<object>.ErrorResponse("Intervention non trouvée"));

                return Ok(ApiResponse<bool>.SuccessResponse(true, "Intervention terminée"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erreur lors de la fin de l'intervention {Id}", id);
                return BadRequest(ApiResponse<object>.ErrorResponse(ex.Message));
            }
        }

        /// <summary>
        /// Annuler une intervention
        /// </summary>
        [HttpPost("{id:guid}/annuler")]
        [ProducesResponseType(typeof(ApiResponse<bool>), 200)]
        [ProducesResponseType(404)]
        [Authorize(Roles = "ResponsableSAV,Admin")]
        public async Task<IActionResult> Annuler(Guid id)
        {
            try
            {
                var success = await _interventionService.AnnulerInterventionAsync(id);
                if (!success)
                    return NotFound(ApiResponse<object>.ErrorResponse("Intervention non trouvée"));

                return Ok(ApiResponse<bool>.SuccessResponse(true, "Intervention annulée"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erreur lors de l'annulation de l'intervention {Id}", id);
                return BadRequest(ApiResponse<object>.ErrorResponse(ex.Message));
            }
        }

        /// <summary>
        /// Calculer une facture pour une intervention
        /// </summary>
        [HttpPost("{id:guid}/calculer-facture")]
        [ProducesResponseType(typeof(ApiResponse<FactureResponse>), 200)]
        [ProducesResponseType(400)]
        [Authorize(Roles = "ResponsableSAV,Admin")]
        public async Task<IActionResult> CalculerFacture(Guid id, [FromBody] CalculFactureRequest request)
        {
            try
            {
                var facture = await _interventionService.CalculerFactureAsync(id, request);
                return Ok(ApiResponse<FactureResponse>.SuccessResponse(facture));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erreur lors du calcul de la facture pour l'intervention {Id}", id);
                return BadRequest(ApiResponse<object>.ErrorResponse(ex.Message));
            }
        }

        /// <summary>
        /// Générer une facture pour une intervention
        /// </summary>
        [HttpPost("{id:guid}/generer-facture")]
        [ProducesResponseType(typeof(ApiResponse<bool>), 200)]
        [ProducesResponseType(400)]
        [Authorize(Roles = "ResponsableSAV,Admin")]
        public async Task<IActionResult> GenererFacture(Guid id, [FromBody] CalculFactureRequest request)
        {
            try
            {
                var success = await _interventionService.GenererFactureAsync(id, request);
                if (!success)
                    return NotFound(ApiResponse<object>.ErrorResponse("Intervention non trouvée"));

                return Ok(ApiResponse<bool>.SuccessResponse(true, "Facture générée"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erreur lors de la génération de la facture pour l'intervention {Id}", id);
                return BadRequest(ApiResponse<object>.ErrorResponse(ex.Message));
            }
        }

        /// <summary>
        /// Marquer une facture comme payée
        /// </summary>
        [HttpPost("{id:guid}/payer-facture")]
        [ProducesResponseType(typeof(ApiResponse<bool>), 200)]
        [ProducesResponseType(404)]
        [Authorize(Roles = "ResponsableSAV,Admin")]
        public async Task<IActionResult> PayerFacture(Guid id)
        {
            try
            {
                var success = await _interventionService.MarquerFacturePayeeAsync(id);
                if (!success)
                    return NotFound(ApiResponse<object>.ErrorResponse("Intervention non trouvée"));

                return Ok(ApiResponse<bool>.SuccessResponse(true, "Facture marquée comme payée"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erreur lors du paiement de la facture pour l'intervention {Id}", id);
                return BadRequest(ApiResponse<object>.ErrorResponse(ex.Message));
            }
        }

        /// <summary>
        /// Récupérer les interventions par réclamation
        /// </summary>
        [HttpGet("reclamation/{reclamationId:guid}")]
        [ProducesResponseType(typeof(ApiResponse<List<Intervention>>), 200)]
        public async Task<IActionResult> GetByReclamation(Guid reclamationId)
        {
            try
            {
                var interventions = await _interventionService.GetInterventionsByReclamationAsync(reclamationId);
                return Ok(ApiResponse<List<Intervention>>.SuccessResponse(interventions));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erreur lors de la récupération des interventions pour la réclamation {ReclamationId}",
                    reclamationId);
                return StatusCode(500, ApiResponse<object>.ErrorResponse("Erreur interne"));
            }
        }

        /// <summary>
        /// Récupérer les interventions par statut
        /// </summary>
        [HttpGet("statut/{statut}")]
        [ProducesResponseType(typeof(ApiResponse<List<Intervention>>), 200)]
        [Authorize(Roles = "ResponsableSAV,Admin")]
        public async Task<IActionResult> GetByStatut(string statut)
        {
            try
            {
                var interventions = await _interventionService.GetInterventionsByStatutAsync(statut);
                return Ok(ApiResponse<List<Intervention>>.SuccessResponse(interventions));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erreur lors de la récupération des interventions avec le statut {Statut}", statut);
                return StatusCode(500, ApiResponse<object>.ErrorResponse("Erreur interne"));
            }
        }

        /// <summary>
        /// Récupérer la liste des techniciens disponibles
        /// </summary>
        [HttpGet("techniciens")]
        [ProducesResponseType(typeof(ApiResponse<List<object>>), 200)]
        [AllowAnonymous]
        public IActionResult GetTechniciens()
        {
            try
            {
                var service = _interventionService as InterventionService;
                var techniciens = service?.GetTechniciensDisponibles() ?? new List<(string, string, decimal)>();

                var result = techniciens.Select(t => new
                {
                    Nom = t.Item1,
                    Specialite = t.Item2,
                    TauxHoraire = t.Item3
                }).ToList();

                return Ok(ApiResponse<List<object>>.SuccessResponse(result.Cast<object>().ToList()));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erreur lors de la récupération des techniciens");
                return StatusCode(500, ApiResponse<object>.ErrorResponse("Erreur interne"));
            }
        }

        /// <summary>
        /// Vérifier si une intervention nécessite une facture
        /// </summary>
        [HttpGet("{id:guid}/check-facturation")]
        [ProducesResponseType(typeof(ApiResponse<object>), 200)]
        public async Task<IActionResult> CheckFacturation(Guid id)
        {
            try
            {
                var intervention = await _interventionService.GetInterventionByIdAsync(id);
                if (intervention == null)
                    return NotFound(ApiResponse<object>.ErrorResponse("Intervention non trouvée"));

                var result = new
                {
                    SousGarantie = intervention.SousGarantie,
                    DoitEtreFacturee = !intervention.SousGarantie && intervention.Statut == InterventionStatut.Terminee,
                    FactureExistante = intervention.MontantFacture.HasValue,
                    MontantFacture = intervention.MontantFacture,
                    FacturePayee = intervention.FacturePayee
                };

                return Ok(ApiResponse<object>.SuccessResponse(result));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erreur lors de la vérification de facturation pour l'intervention {Id}", id);
                return StatusCode(500, ApiResponse<object>.ErrorResponse("Erreur interne"));
            }
        }
    }
}