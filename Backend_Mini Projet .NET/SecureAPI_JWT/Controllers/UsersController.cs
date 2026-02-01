using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SecureAPI_JMT.Models;
using SecureAPI_JWT.Models;
using SecureAPI_JWT.Services;
using System;
using System.Threading.Tasks;

namespace Microservice1_Auth.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly IAuthService _authService;
        private readonly ILogger<UsersController> _logger;

        public UsersController(IAuthService authService, ILogger<UsersController> logger)
        {
            _authService = authService;
            _logger = logger;
        }

        // GET: api/users
        [HttpGet]
        public async Task<IActionResult> GetAllUsers()
        {
            try
            {
                var users = await _authService.GetAllUsersAsync();
                return Ok(users);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erreur lors de la récupération des utilisateurs");
                return StatusCode(500, "Erreur interne");
            }
        }

        // GET: api/users/{id}
        [HttpGet("{id}")]
        public async Task<IActionResult> GetUserById(string id)
        {
            try
            {
                // Utilisez la nouvelle méthode optimisée
                var user = await _authService.GetUserByIdAsync(id);
                if (user == null)
                    return NotFound("Utilisateur non trouvé");

                return Ok(user);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erreur lors de la récupération de l'utilisateur");
                return StatusCode(500, "Erreur interne");
            }
        }

 
        // PUT: api/users/update/{id}
        [HttpPut("update/{id}")]
        public async Task<IActionResult> UpdateUser(string id, [FromBody] UpdateUserModel model)
        {
            try
            {
                if (string.IsNullOrEmpty(id))
                    return BadRequest("ID utilisateur requis");

                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                var error = await _authService.UpdateUserAsync(id, model);

                if (!string.IsNullOrEmpty(error))
                    return BadRequest(error);

                return Ok("Utilisateur mis à jour avec succès");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erreur mise à jour utilisateur");
                return StatusCode(500, "Erreur interne");
            }
        }

        // DELETE: api/users/delete/{id}
        [HttpDelete("delete/{id}")]
        public async Task<IActionResult> DeleteUser(string id)
        {
            try
            {
                if (string.IsNullOrEmpty(id))
                    return BadRequest("ID utilisateur requis");

                var error = await _authService.DeleteUserAsync(id);

                if (!string.IsNullOrEmpty(error))
                    return BadRequest(error);

                return Ok("Utilisateur supprimé avec succès");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erreur suppression utilisateur");
                return StatusCode(500, "Erreur interne");
            }
        }

        // POST: api/users/affect-role
        [HttpPost("affect-role")]
        public async Task<IActionResult> AddRoleToUser([FromBody] AddRoleModel model)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                var error = await _authService.AddRoleAsync(model);

                if (!string.IsNullOrEmpty(error))
                    return BadRequest(error);

                return Ok("Rôle affecté avec succès");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erreur affectation rôle");
                return StatusCode(500, "Erreur interne");
            }
        }

        // GET: api/users/search?email=xxx&name=xxx
        [HttpGet("search")]
        public async Task<IActionResult> SearchUsers([FromQuery] string email = "", [FromQuery] string name = "")
        {
            try
            {
                var allUsers = await _authService.GetAllUsersAsync();
                var filteredUsers = allUsers;

                // Filtrage par email
                if (!string.IsNullOrEmpty(email))
                {
                    filteredUsers = filteredUsers.Where(u =>
                        u.Email.Contains(email, StringComparison.OrdinalIgnoreCase)).ToList();
                }

                // Filtrage par nom ou prénom
                if (!string.IsNullOrEmpty(name))
                {
                    filteredUsers = filteredUsers.Where(u =>
                        (u.FirstName?.Contains(name, StringComparison.OrdinalIgnoreCase) ?? false) ||
                        (u.LastName?.Contains(name, StringComparison.OrdinalIgnoreCase) ?? false)).ToList();
                }

                return Ok(filteredUsers);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erreur recherche utilisateurs");
                return StatusCode(500, "Erreur interne");
            }
        }

        // GET: api/users/clients
        [HttpGet("clients")]
        public async Task<IActionResult> GetAllClients()
        {
            try
            {
                var allUsers = await _authService.GetAllUsersAsync();
                var clients = allUsers.Where(u => u.Roles.Contains("Client")).ToList();

                return Ok(clients);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erreur récupération clients");
                return StatusCode(500, "Erreur interne");
            }
        }

        // GET: api/users/responsables
        [HttpGet("responsables")]
        public async Task<IActionResult> GetAllResponsables()
        {
            try
            {
                var allUsers = await _authService.GetAllUsersAsync();
                var responsables = allUsers.Where(u => u.Roles.Contains("ResponsableSAV")).ToList();

                return Ok(responsables);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erreur récupération responsables");
                return StatusCode(500, "Erreur interne");
            }
        }
    }
}