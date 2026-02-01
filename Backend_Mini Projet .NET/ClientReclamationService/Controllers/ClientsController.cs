using ClientReclamationService.Data;
using ClientReclamationService.Models;
using ClientReclamationService.Models.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ClientReclamationService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class ClientsController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly ILogger<ClientsController> _logger;

        public ClientsController(AppDbContext context, ILogger<ClientsController> logger)
        {
            _context = context;
            _logger = logger;
        }

        // GET: api/clients
        [HttpGet]
        [Authorize(Roles = "Admin,ResponsableSAV")]
        public async Task<ActionResult<IEnumerable<ClientDTO>>> GetClients()
        {
            var clients = await _context.Clients
                .Include(c => c.Reclamations)
                .OrderByDescending(c => c.DateInscription)
                .ToListAsync();

            var dtos = clients.Select(c => new ClientDTO
            {
                Id = c.Id,
                Nom = c.Nom,
                Prenom = c.Prenom,
                Email = c.Email,
                Telephone = c.Telephone,
                Adresse = c.Adresse,
                Ville = c.Ville,
                CodePostal = c.CodePostal,
                DateInscription = c.DateInscription,
                NombreReclamations = c.Reclamations.Count
            });

            return Ok(dtos);
        }

        // GET: api/clients/5
        [HttpGet("{id}")]
        public async Task<ActionResult<ClientDTO>> GetClient(int id)
        {
            var client = await _context.Clients
                .Include(c => c.Reclamations)
                .FirstOrDefaultAsync(c => c.Id == id);

            if (client == null)
                return NotFound();

            var dto = new ClientDTO
            {
                Id = client.Id,
                Nom = client.Nom,
                Prenom = client.Prenom,
                Email = client.Email,
                Telephone = client.Telephone,
                Adresse = client.Adresse,
                Ville = client.Ville,
                CodePostal = client.CodePostal,
                DateInscription = client.DateInscription,
                NombreReclamations = client.Reclamations.Count
            };

            return Ok(dto);
        }

        // GET: api/clients/by-user/{userId}
        [HttpGet("by-user/{userId}")]
        public async Task<ActionResult<ClientDTO>> GetClientByUserId(string userId)
        {
            var client = await _context.Clients
                .Include(c => c.Reclamations)
                .FirstOrDefaultAsync(c => c.UserId == userId);

            if (client == null)
                return NotFound($"Client avec UserId {userId} non trouvé");

            var dto = new ClientDTO
            {
                Id = client.Id,
                Nom = client.Nom,
                Prenom = client.Prenom,
                Email = client.Email,
                Telephone = client.Telephone,
                Adresse = client.Adresse,
                Ville = client.Ville,
                CodePostal = client.CodePostal,
                DateInscription = client.DateInscription,
                NombreReclamations = client.Reclamations.Count
            };

            return Ok(dto);
        }

        // POST: api/clients
        [HttpPost]
        [Authorize(Roles = "Admin,ResponsableSAV")]
        public async Task<ActionResult<ClientDTO>> CreateClient(CreateClientDTO dto)
        {
            // Vérifier si l'email existe déjà
            var existingClient = await _context.Clients
                .FirstOrDefaultAsync(c => c.Email == dto.Email || c.UserId == dto.UserId);

            if (existingClient != null)
                return Conflict("Un client avec cet email ou UserId existe déjà");

            var client = new Client
            {
                Nom = dto.Nom,
                Prenom = dto.Prenom,
                Email = dto.Email,
                Telephone = dto.Telephone,
                Adresse = dto.Adresse,
                Ville = dto.Ville,
                CodePostal = dto.CodePostal,
                UserId = dto.UserId,
                DateInscription = DateTime.UtcNow
            };

            _context.Clients.Add(client);
            await _context.SaveChangesAsync();

            _logger.LogInformation("Nouveau client créé: {Prenom} {Nom}", client.Prenom, client.Nom);

            var clientDto = new ClientDTO
            {
                Id = client.Id,
                Nom = client.Nom,
                Prenom = client.Prenom,
                Email = client.Email,
                Telephone = client.Telephone,
                Adresse = client.Adresse,
                Ville = client.Ville,
                CodePostal = client.CodePostal,
                DateInscription = client.DateInscription,
                NombreReclamations = 0
            };

            return CreatedAtAction(nameof(GetClient), new { id = client.Id }, clientDto);
        }

        // PUT: api/clients/5
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateClient(int id, CreateClientDTO dto)
        {
            var client = await _context.Clients.FindAsync(id);
            if (client == null)
                return NotFound();

            client.Nom = dto.Nom;
            client.Prenom = dto.Prenom;
            client.Email = dto.Email;
            client.Telephone = dto.Telephone;
            client.Adresse = dto.Adresse;
            client.Ville = dto.Ville;
            client.CodePostal = dto.CodePostal;
            client.UserId = dto.UserId;

            await _context.SaveChangesAsync();

            _logger.LogInformation("Client mis à jour: ID {Id}", id);
            return NoContent();
        }

        // DELETE: api/clients/5
        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteClient(int id)
        {
            var client = await _context.Clients.FindAsync(id);
            if (client == null)
                return NotFound();

            _context.Clients.Remove(client);
            await _context.SaveChangesAsync();

            _logger.LogInformation("Client supprimé: ID {Id}", id);
            return NoContent();
        }
    }
}