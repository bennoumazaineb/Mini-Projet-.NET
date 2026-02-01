using Microsoft.EntityFrameworkCore;
using ReclamationService.Data;
using ReclamationService.Models;
using ReclamationService.Models.DTOs;
using Microsoft.Extensions.Logging;

namespace ReclamationService.Services
{
    public class ReclamationService : IReclamationService
    {
        private readonly ReclamationDbContext _context;
        private readonly IAuthClientService _authClientService;
        private readonly ILogger<ReclamationService> _logger;

        public ReclamationService(
            ReclamationDbContext context,
            IAuthClientService authClientService,
            ILogger<ReclamationService> logger)
        {
            _context = context;
            _authClientService = authClientService;
            _logger = logger;
        }

        // === POUR LES CLIENTS ===

        public async Task<ReclamationDTO> CreateReclamationAsync(CreateReclamationDTO dto, string currentUserId)
        {
            var isClient = await _authClientService.IsClientAsync(currentUserId);
            if (!isClient)
                throw new UnauthorizedAccessException("Seuls les clients peuvent créer des réclamations");

            var clientInfo = await _authClientService.GetClientInfoAsync(currentUserId);
            if (clientInfo == null)
                throw new KeyNotFoundException("Client non trouvé");

            var dateFinGarantie = dto.DateAchat.AddMonths(dto.DureeGarantieMois);

            var reclamation = new Reclamation
            {
                ClientUserId = currentUserId,
                ClientEmail = clientInfo.Email,
                ClientNomComplet = $"{clientInfo.FirstName} {clientInfo.LastName}",
                ClientTelephone = clientInfo.PhoneNumber ?? "Non spécifié",
                Description = dto.Description,
                ArticleNom = dto.ArticleNom,
                ArticleReference = dto.ArticleReference,
                DateAchat = dto.DateAchat,
                DateFinGarantie = dateFinGarantie,
                Statut = StatutReclamation.Nouvelle,
                Priorite = dateFinGarantie < DateTime.UtcNow.AddDays(30) ? PrioriteReclamation.Haute : PrioriteReclamation.Normale
            };

            _context.Reclamations.Add(reclamation);
            await _context.SaveChangesAsync();

            _logger.LogInformation($"Réclamation créée par Client {currentUserId}: {reclamation.Reference}");

            return MapToDTO(reclamation);
        }

        public async Task<IEnumerable<ReclamationDTO>> GetMyReclamationsAsync(string clientUserId)
        {
            var isClient = await _authClientService.IsClientAsync(clientUserId);
            if (!isClient)
                throw new UnauthorizedAccessException("Accès non autorisé");

            var reclamations = await _context.Reclamations
                .Where(r => r.ClientUserId == clientUserId)
                .OrderByDescending(r => r.DateCreation)
                .ToListAsync();

            return reclamations.Select(MapToDTO);
        }

        public async Task<ReclamationDTO> GetMyReclamationByIdAsync(int id, string clientUserId)
        {
            var isClient = await _authClientService.IsClientAsync(clientUserId);
            if (!isClient)
                throw new UnauthorizedAccessException("Accès non autorisé");

            var reclamation = await _context.Reclamations
                .FirstOrDefaultAsync(r => r.Id == id && r.ClientUserId == clientUserId);

            if (reclamation == null)
                throw new KeyNotFoundException("Réclamation non trouvée");

            return MapToDTO(reclamation);
        }

        public async Task<ReclamationDTO> UpdateMyReclamationAsync(int id, UpdateReclamationDTO dto, string clientUserId)
        {
            var isClient = await _authClientService.IsClientAsync(clientUserId);
            if (!isClient)
                throw new UnauthorizedAccessException("Accès non autorisé");

            var reclamation = await _context.Reclamations
                .FirstOrDefaultAsync(r => r.Id == id && r.ClientUserId == clientUserId);

            if (reclamation == null)
                throw new KeyNotFoundException("Réclamation non trouvée");

            if (!string.IsNullOrEmpty(dto.Description))
                reclamation.Description = dto.Description;

            await _context.SaveChangesAsync();
            return MapToDTO(reclamation);
        }

        // === RESPONSABLES SAV ===

        public async Task<IEnumerable<ReclamationDTO>> GetAllReclamationsAsync()
        {
            var reclamations = await _context.Reclamations
                .OrderByDescending(r => r.DateCreation)
                .ToListAsync();
            return reclamations.Select(MapToDTO);
        }

        public async Task<ReclamationDTO> GetReclamationByIdAsync(int id)
        {
            var reclamation = await _context.Reclamations.FindAsync(id);
            if (reclamation == null) throw new KeyNotFoundException("Réclamation non trouvée");
            return MapToDTO(reclamation);
        }

        public async Task<IEnumerable<ReclamationDTO>> GetReclamationsByClientAsync(string clientUserId)
        {
            var clientExists = await _authClientService.ValidateUserExistsAsync(clientUserId);
            if (!clientExists) throw new KeyNotFoundException("Client non trouvé");

            var reclamations = await _context.Reclamations
                .Where(r => r.ClientUserId == clientUserId)
                .OrderByDescending(r => r.DateCreation)
                .ToListAsync();
            return reclamations.Select(MapToDTO);
        }

        public async Task<ReclamationDTO> UpdateReclamationAsync(int id, UpdateReclamationDTO dto, string updatedByUserId)
        {
            var isResponsable = await _authClientService.IsResponsableSAVAsync(updatedByUserId);
            if (!isResponsable) throw new UnauthorizedAccessException("Seuls les responsables SAV peuvent modifier les réclamations");

            var reclamation = await _context.Reclamations.FindAsync(id);
            if (reclamation == null) throw new KeyNotFoundException("Réclamation non trouvée");

            if (!string.IsNullOrEmpty(dto.Description)) reclamation.Description = dto.Description;
            if (!string.IsNullOrEmpty(dto.Statut) && Enum.TryParse(dto.Statut, out StatutReclamation statut)) reclamation.Statut = statut;
            if (!string.IsNullOrEmpty(dto.Priorite) && Enum.TryParse(dto.Priorite, out PrioriteReclamation priorite)) reclamation.Priorite = priorite;

            await _context.SaveChangesAsync();
            return MapToDTO(reclamation);
        }

        public async Task<ReclamationDTO> UpdateStatutReclamationAsync(int id, string statut, string updatedByUserId)
        {
            var isResponsable = await _authClientService.IsResponsableSAVAsync(updatedByUserId);
            if (!isResponsable) throw new UnauthorizedAccessException("Seuls les responsables SAV peuvent modifier le statut");

            if (!Enum.TryParse(statut, out StatutReclamation statutEnum)) throw new ArgumentException("Statut invalide");

            var reclamation = await _context.Reclamations.FindAsync(id);
            if (reclamation == null) throw new KeyNotFoundException("Réclamation non trouvée");

            reclamation.Statut = statutEnum;
            await _context.SaveChangesAsync();
            return MapToDTO(reclamation);
        }

        public async Task<IEnumerable<ReclamationDTO>> GetReclamationsByStatutAsync(string statut)
        {
            if (!Enum.TryParse(statut, out StatutReclamation statutEnum)) throw new ArgumentException("Statut invalide");
            var reclamations = await _context.Reclamations
                .Where(r => r.Statut == statutEnum)
                .OrderByDescending(r => r.DateCreation)
                .ToListAsync();
            return reclamations.Select(MapToDTO);
        }

        public async Task<bool> DeleteReclamationAsync(int id)
        {
            var reclamation = await _context.Reclamations.FindAsync(id);
            if (reclamation == null) return false;

            _context.Reclamations.Remove(reclamation);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<object> GetStatistiquesAsync()
        {
            var total = await _context.Reclamations.CountAsync();
            var parStatut = await _context.Reclamations
                .GroupBy(r => r.Statut)
                .Select(g => new { Statut = g.Key.ToString(), Count = g.Count() })
                .ToListAsync();
            var parMois = await _context.Reclamations
                .Where(r => r.DateCreation.Year == DateTime.UtcNow.Year)
                .GroupBy(r => r.DateCreation.Month)
                .Select(g => new { Mois = g.Key, Count = g.Count() })
                .OrderBy(g => g.Mois)
                .ToListAsync();

            return new
            {
                TotalReclamations = total,
                ParStatut = parStatut,
                ParMois = parMois,
                SousGarantie = await _context.Reclamations.CountAsync(r => r.EstSousGarantie),
                HorsGarantie = await _context.Reclamations.CountAsync(r => !r.EstSousGarantie)
            };
        }

        // === PRIVÉ ===
        private ReclamationDTO MapToDTO(Reclamation reclamation)
        {
            return new ReclamationDTO
            {
                Id = reclamation.Id,
                Reference = reclamation.Reference,
                DateCreation = reclamation.DateCreation,
                Description = reclamation.Description,
                Statut = reclamation.Statut.ToString(),
                Priorite = reclamation.Priorite.ToString(),
                ClientUserId = reclamation.ClientUserId,
                ClientEmail = reclamation.ClientEmail,
                ClientNomComplet = reclamation.ClientNomComplet,
                ClientTelephone = reclamation.ClientTelephone,
                ArticleNom = reclamation.ArticleNom,
                ArticleReference = reclamation.ArticleReference,
                DateAchat = reclamation.DateAchat,
                DateFinGarantie = reclamation.DateFinGarantie,
                EstSousGarantie = reclamation.EstSousGarantie,
                GarantieExpiree = reclamation.GarantieExpiree,
                JoursRestantsGarantie = reclamation.JoursRestantsGarantie,
                InterventionId = reclamation.InterventionId
            };
        }
    }
}
