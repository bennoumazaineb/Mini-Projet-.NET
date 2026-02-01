using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microservice2_Reclamations.Data;
using Microservice2_Reclamations.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Microservice2_Reclamations.Services
{
    public class ReclamationService : IReclamationService
    {
        private readonly ReclamationDbContext _context;
        private readonly ILogger<ReclamationService> _logger;

        public ReclamationService(
            ReclamationDbContext context,
            ILogger<ReclamationService> logger)
        {
            _context = context;
            _logger = logger;
        }

        // === CLIENT ===
        public async Task<ReclamationResponse> CreateReclamationAsync(CreateReclamationModel model, string authToken)
        {
            try
            {
                var dureeGarantieMois = 24;
                var estSousGarantie = DateTime.UtcNow <= model.DateAchat.AddMonths(dureeGarantieMois);

                var reclamation = new Reclamation
                {
                    Titre = model.Titre,
                    Description = model.Description,
                    ClientId = model.ClientId,
                    ClientEmail = model.ClientEmail,
                    ClientNom = model.ClientNom,
                    ArticleReference = model.ArticleReference,
                    DateAchat = model.DateAchat,
                    SousGarantie = estSousGarantie,
                    Statut = StatutReclamation.NonTraitee,
                    DateCreation = DateTime.UtcNow
                };

                if (!estSousGarantie)
                    reclamation.MontantFacture = model.PrixAchat * 0.1m;

                _context.Reclamations.Add(reclamation);
                await _context.SaveChangesAsync();

                _logger.LogInformation(
                    "Réclamation créée: ID={Id}, Client={Client} ({Email})",
                    reclamation.Id, reclamation.ClientNom, reclamation.ClientEmail);

                return MapToResponse(reclamation);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erreur création réclamation");
                throw;
            }
        }

        public async Task<List<ReclamationResponse>> GetReclamationsByClientAsync(string clientId, string authToken)
        {
            var reclamations = await _context.Reclamations
                .Where(r => r.ClientId == clientId)
                .OrderByDescending(r => r.DateCreation)
                .ToListAsync();

            return reclamations.Select(MapToResponse).ToList();
        }

        public async Task<ReclamationResponse?> GetReclamationByIdForClientAsync(int id, string clientId, string authToken)
        {
            var reclamation = await _context.Reclamations
                .FirstOrDefaultAsync(r => r.Id == id && r.ClientId == clientId);

            if (reclamation == null) return null;

            return MapToResponse(reclamation);
        }

        // === RESPONSABLE SAV ===
        public async Task<List<ReclamationResponse>> GetAllReclamationsAsync()
        {
            var reclamations = await _context.Reclamations
                .OrderByDescending(r => r.DateCreation)
                .ToListAsync();

            return reclamations.Select(MapToResponse).ToList();
        }

        public async Task<List<ReclamationResponse>> GetReclamationsByStatutAsync(StatutReclamation statut)
        {
            var reclamations = await _context.Reclamations
                .Where(r => r.Statut == statut)
                .OrderByDescending(r => r.DateCreation)
                .ToListAsync();

            return reclamations.Select(MapToResponse).ToList();
        }

        public async Task<ReclamationResponse?> GetReclamationByIdAsync(int id)
        {
            var reclamation = await _context.Reclamations
                .FirstOrDefaultAsync(r => r.Id == id);

            if (reclamation == null) return null;

            return MapToResponse(reclamation);
        }

        public async Task<ReclamationResponse> UpdateReclamationAsync(int id, UpdateReclamationModel model)
        {
            var reclamation = await _context.Reclamations.FirstOrDefaultAsync(r => r.Id == id);
            if (reclamation == null)
                throw new KeyNotFoundException($"Réclamation non trouvée avec l'ID {id}");

            if (!string.IsNullOrEmpty(model.Titre)) reclamation.Titre = model.Titre;
            if (!string.IsNullOrEmpty(model.Description)) reclamation.Description = model.Description;
            if (model.Statut.HasValue) reclamation.Statut = model.Statut.Value;
            if (!string.IsNullOrEmpty(model.Solution)) reclamation.Solution = model.Solution;
            if (!string.IsNullOrEmpty(model.NotesInterne)) reclamation.NotesInterne = model.NotesInterne;
            if (!string.IsNullOrEmpty(model.ResponsableSAVId)) reclamation.ResponsableSAVId = model.ResponsableSAVId;
            if (!string.IsNullOrEmpty(model.ResponsableSAVNom)) reclamation.ResponsableSAVNom = model.ResponsableSAVNom;
            if (model.MontantFacture.HasValue) reclamation.MontantFacture = model.MontantFacture.Value;
            if (model.DureeGarantieMois.HasValue)
                reclamation.SousGarantie = DateTime.UtcNow <= reclamation.DateAchat.AddMonths(model.DureeGarantieMois.Value);

            reclamation.DateModification = DateTime.UtcNow;

            _context.Reclamations.Update(reclamation);
            await _context.SaveChangesAsync();

            return MapToResponse(reclamation);
        }

        public async Task<bool> DeleteReclamationAsync(int id)
        {
            var reclamation = await _context.Reclamations.FirstOrDefaultAsync(r => r.Id == id);
            if (reclamation == null) return false;

            _context.Reclamations.Remove(reclamation);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<List<ReclamationResponse>> SearchReclamationsAsync(string searchTerm)
        {
            var query = _context.Reclamations.AsQueryable();

            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                searchTerm = searchTerm.ToLower();
                query = query.Where(r =>
                    r.Titre.ToLower().Contains(searchTerm) ||
                    r.Description.ToLower().Contains(searchTerm) ||
                    r.ClientNom.ToLower().Contains(searchTerm) ||
                    r.ClientEmail.ToLower().Contains(searchTerm) ||
                    r.ArticleReference.ToLower().Contains(searchTerm) ||
                    (r.Solution != null && r.Solution.ToLower().Contains(searchTerm)) ||
                    (r.NotesInterne != null && r.NotesInterne.ToLower().Contains(searchTerm))
                );
            }

            var reclamations = await query.OrderByDescending(r => r.DateCreation).ToListAsync();
            return reclamations.Select(MapToResponse).ToList();
        }

        public async Task<bool> AssignerResponsableAsync(int reclamationId, string responsableId, string responsableNom)
        {
            var reclamation = await _context.Reclamations.FirstOrDefaultAsync(r => r.Id == reclamationId);
            if (reclamation == null) return false;

            reclamation.ResponsableSAVId = responsableId;
            reclamation.ResponsableSAVNom = responsableNom;
            reclamation.DateModification = DateTime.UtcNow;

            if (reclamation.Statut == StatutReclamation.NonTraitee)
                reclamation.Statut = StatutReclamation.EnCours;

            _context.Reclamations.Update(reclamation);
            await _context.SaveChangesAsync();

            return true;
        }

        public async Task<bool> VerifierGarantieAsync(int reclamationId, int dureeGarantieMois = 24)
        {
            var reclamation = await _context.Reclamations.FirstOrDefaultAsync(r => r.Id == reclamationId);
            if (reclamation == null) throw new KeyNotFoundException($"Réclamation non trouvée avec l'ID {reclamationId}");

            var estSousGarantie = DateTime.UtcNow <= reclamation.DateAchat.AddMonths(dureeGarantieMois);
            if (reclamation.SousGarantie != estSousGarantie)
            {
                reclamation.SousGarantie = estSousGarantie;
                reclamation.DateModification = DateTime.UtcNow;
                await _context.SaveChangesAsync();
            }

            return estSousGarantie;
        }

        public async Task<bool> UpdateStatutAsync(int reclamationId, StatutReclamation nouveauStatut, string? solution = null)
        {
            var reclamation = await _context.Reclamations.FirstOrDefaultAsync(r => r.Id == reclamationId);
            if (reclamation == null) return false;

            reclamation.Statut = nouveauStatut;
            if (!string.IsNullOrEmpty(solution)) reclamation.Solution = solution;

            if (nouveauStatut == StatutReclamation.Traitee && reclamation.DateCloture == null)
                reclamation.DateCloture = DateTime.UtcNow;

            reclamation.DateModification = DateTime.UtcNow;

            _context.Reclamations.Update(reclamation);
            await _context.SaveChangesAsync();

            return true;
        }

        public async Task<Dictionary<StatutReclamation, int>> GetStatistiquesAsync()
        {
            var statistiques = await _context.Reclamations
                .GroupBy(r => r.Statut)
                .Select(g => new { Statut = g.Key, Count = g.Count() })
                .ToDictionaryAsync(x => x.Statut, x => x.Count);

            foreach (var statut in Enum.GetValues(typeof(StatutReclamation)).Cast<StatutReclamation>())
            {
                if (!statistiques.ContainsKey(statut)) statistiques[statut] = 0;
            }

            return statistiques;
        }

        private ReclamationResponse MapToResponse(Reclamation reclamation)
        {
            return new ReclamationResponse
            {
                Id = reclamation.Id,
                Titre = reclamation.Titre,
                Description = reclamation.Description,
                ClientId = reclamation.ClientId,
                ClientEmail = reclamation.ClientEmail,
                ClientNom = reclamation.ClientNom,
                ArticleReference = reclamation.ArticleReference,
                DateAchat = reclamation.DateAchat,
                SousGarantie = reclamation.SousGarantie,
                MontantFacture = reclamation.MontantFacture,
                Statut = reclamation.Statut,
                ResponsableSAVId = reclamation.ResponsableSAVId,
                ResponsableSAVNom = reclamation.ResponsableSAVNom,
                DateCreation = reclamation.DateCreation,
                DateModification = reclamation.DateModification,
                DateCloture = reclamation.DateCloture,
                Solution = reclamation.Solution
            };
        }
    }
}
