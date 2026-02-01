using Microsoft.EntityFrameworkCore;
using SAV.InterventionsAPI.Models;

namespace SAV.InterventionsAPI.Services
{
    public class InterventionService : IInterventionService
    {
        private readonly List<Intervention> _interventions = new();
        private readonly ILogger<InterventionService> _logger;

        // Techniciens disponibles (en mémoire)
        private readonly List<(string Nom, string Specialite, decimal TauxHoraire)> _techniciens = new()
        {
            ("Ahmed Ben Ali", SpecialiteTechnicien.Chauffage, 200),
            ("Samia Khaled", SpecialiteTechnicien.Sanitaire, 180),
            ("Mohamed Trabelsi", SpecialiteTechnicien.Generaliste, 150),
            ("Fatma Jlassi", SpecialiteTechnicien.Generaliste, 160)
        };

        public InterventionService(ILogger<InterventionService> logger)
        {
            _logger = logger;
            SeedTestInterventions();
        }

        private void SeedTestInterventions()
        {
            // Intervention sous garantie
            _interventions.Add(new Intervention
            {
                Id = Guid.NewGuid(),
                Numero = "INT-20240115-001",
                ReclamationId = Guid.NewGuid(),
                TechnicienNom = "Ahmed Ben Ali",
                TechnicienSpecialite = SpecialiteTechnicien.Chauffage,
                DatePlanification = DateTime.Now.AddDays(2),
                Description = "Problème de chauffage dans la salle de bain",
                SousGarantie = true,
                Statut = InterventionStatut.Planifiee,
                CreatedBy = "sav@test.com",
                CreatedAt = DateTime.UtcNow.AddDays(-1)
            });

            // Intervention hors garantie (facturée)
            _interventions.Add(new Intervention
            {
                Id = Guid.NewGuid(),
                Numero = "INT-20240114-001",
                ReclamationId = Guid.NewGuid(),
                TechnicienNom = "Samia Khaled",
                TechnicienSpecialite = SpecialiteTechnicien.Sanitaire,
                DatePlanification = DateTime.Now.AddDays(-3),
                DateDebut = DateTime.Now.AddDays(-2),
                DateFin = DateTime.Now.AddDays(-1),
                Description = "Réparation robinet cuisine",
                Rapport = "Robinet remplacé, joint changé",
                SousGarantie = false,
                Statut = InterventionStatut.Terminee,
                CoutMainOeuvre = 360, // 2 heures × 180€/h
                CoutPieces = 45,
                MontantFacture = 405,
                DateFacturation = DateTime.Now.AddDays(-1),
                FacturePayee = false,
                CreatedBy = "sav@test.com",
                CreatedAt = DateTime.UtcNow.AddDays(-5)
            });

            // Intervention en cours
            _interventions.Add(new Intervention
            {
                Id = Guid.NewGuid(),
                Numero = "INT-20240116-001",
                ReclamationId = Guid.NewGuid(),
                TechnicienNom = "Mohamed Trabelsi",
                TechnicienSpecialite = SpecialiteTechnicien.Generaliste,
                DatePlanification = DateTime.Now.AddDays(-1),
                DateDebut = DateTime.Now,
                Description = "Installation radiateur salle de séjour",
                SousGarantie = true,
                Statut = InterventionStatut.EnCours,
                CreatedBy = "sav@test.com",
                CreatedAt = DateTime.UtcNow.AddDays(-2)
            });

            _logger.LogInformation("Test interventions seeded: {Count} interventions", _interventions.Count);
        }

        public Task<List<Intervention>> GetAllInterventionsAsync()
        {
            return Task.FromResult(_interventions.OrderByDescending(i => i.DatePlanification).ToList());
        }

        public Task<Intervention?> GetInterventionByIdAsync(Guid id)
        {
            var intervention = _interventions.FirstOrDefault(i => i.Id == id);
            return Task.FromResult(intervention);
        }

        public Task<Intervention?> GetInterventionByNumeroAsync(string numero)
        {
            var intervention = _interventions.FirstOrDefault(i => i.Numero == numero);
            return Task.FromResult(intervention);
        }

        public Task<Intervention> CreateInterventionAsync(CreateInterventionRequest request, string createdBy)
        {
            // Vérifier si le technicien existe dans notre liste
            var technicien = _techniciens.FirstOrDefault(t =>
                t.Nom.Equals(request.TechnicienNom, StringComparison.OrdinalIgnoreCase));

            if (technicien.Nom == null)
            {
                throw new Exception($"Technicien '{request.TechnicienNom}' non trouvé");
            }

            // Générer un numéro unique
            var numero = $"INT-{DateTime.Now:yyyyMMdd}-{(_interventions.Count + 1):D3}";

            var intervention = new Intervention
            {
                Id = Guid.NewGuid(),
                Numero = numero,
                ReclamationId = request.ReclamationId,
                TechnicienNom = request.TechnicienNom,
                TechnicienSpecialite = technicien.Specialite,
                DatePlanification = request.DatePlanification,
                Description = request.Description,
                SousGarantie = request.SousGarantie,
                Statut = InterventionStatut.Planifiee,
                CreatedBy = createdBy,
                CreatedAt = DateTime.UtcNow
            };

            _interventions.Add(intervention);
            _logger.LogInformation("Nouvelle intervention créée: {Numero} par {CreatedBy}",
                intervention.Numero, createdBy);

            return Task.FromResult(intervention);
        }

        public Task<bool> UpdateInterventionAsync(Guid id, UpdateInterventionRequest request)
        {
            var intervention = _interventions.FirstOrDefault(i => i.Id == id);
            if (intervention == null) return Task.FromResult(false);

            if (!string.IsNullOrWhiteSpace(request.TechnicienNom))
                intervention.TechnicienNom = request.TechnicienNom;

            if (!string.IsNullOrWhiteSpace(request.TechnicienSpecialite))
                intervention.TechnicienSpecialite = request.TechnicienSpecialite;

            if (request.DatePlanification.HasValue)
                intervention.DatePlanification = request.DatePlanification.Value;

            if (!string.IsNullOrWhiteSpace(request.Description))
                intervention.Description = request.Description;

            if (!string.IsNullOrWhiteSpace(request.Statut))
                intervention.Statut = request.Statut;

            if (!string.IsNullOrWhiteSpace(request.Rapport))
                intervention.Rapport = request.Rapport;

            if (request.SousGarantie.HasValue)
                intervention.SousGarantie = request.SousGarantie.Value;

            intervention.UpdatedAt = DateTime.UtcNow;

            _logger.LogInformation("Intervention {Numero} mise à jour", intervention.Numero);
            return Task.FromResult(true);
        }

        public Task<bool> DemarrerInterventionAsync(Guid id)
        {
            var intervention = _interventions.FirstOrDefault(i => i.Id == id);
            if (intervention == null) return Task.FromResult(false);

            if (intervention.Statut != InterventionStatut.Planifiee)
                throw new Exception("Seules les interventions planifiées peuvent être démarrées");

            intervention.Statut = InterventionStatut.EnCours;
            intervention.DateDebut = DateTime.UtcNow;
            intervention.UpdatedAt = DateTime.UtcNow;

            _logger.LogInformation("Intervention {Numero} démarrée", intervention.Numero);
            return Task.FromResult(true);
        }

        public Task<bool> TerminerInterventionAsync(Guid id, string rapport)
        {
            var intervention = _interventions.FirstOrDefault(i => i.Id == id);
            if (intervention == null) return Task.FromResult(false);

            if (intervention.Statut != InterventionStatut.EnCours)
                throw new Exception("Seules les interventions en cours peuvent être terminées");

            intervention.Statut = InterventionStatut.Terminee;
            intervention.DateFin = DateTime.UtcNow;
            intervention.Rapport = rapport;
            intervention.UpdatedAt = DateTime.UtcNow;

            _logger.LogInformation("Intervention {Numero} terminée", intervention.Numero);
            return Task.FromResult(true);
        }

        public Task<bool> AnnulerInterventionAsync(Guid id)
        {
            var intervention = _interventions.FirstOrDefault(i => i.Id == id);
            if (intervention == null) return Task.FromResult(false);

            intervention.Statut = InterventionStatut.Annulee;
            intervention.UpdatedAt = DateTime.UtcNow;

            _logger.LogInformation("Intervention {Numero} annulée", intervention.Numero);
            return Task.FromResult(true);
        }

        public Task<FactureResponse> CalculerFactureAsync(Guid id, CalculFactureRequest request)
        {
            var intervention = _interventions.FirstOrDefault(i => i.Id == id);
            if (intervention == null)
                throw new Exception("Intervention non trouvée");

            // Vérifier si l'intervention est terminée
            if (intervention.Statut != InterventionStatut.Terminee)
                throw new Exception("L'intervention doit être terminée pour calculer une facture");

            // Vérifier si sous garantie
            if (intervention.SousGarantie)
            {
                return Task.FromResult(new FactureResponse
                {
                    InterventionId = intervention.Id,
                    NumeroIntervention = intervention.Numero,
                    SousGarantie = true,
                    Message = "Cette intervention est sous garantie, aucune facturation nécessaire"
                });
            }

            // Trouver le taux horaire du technicien
            var technicien = _techniciens.FirstOrDefault(t =>
                t.Nom.Equals(intervention.TechnicienNom, StringComparison.OrdinalIgnoreCase));

            var tauxHoraire = technicien.TauxHoraire > 0 ? technicien.TauxHoraire : 150;

            // Calculer les coûts
            var coutMainOeuvre = tauxHoraire * request.DureeHeures;
            var coutPieces = request.Pieces.Sum(p => p.Quantite * p.PrixUnitaire);
            var totalHT = coutMainOeuvre + coutPieces;
            var tva = totalHT * 0.20m;
            var totalTTC = totalHT + tva;

            return Task.FromResult(new FactureResponse
            {
                InterventionId = intervention.Id,
                NumeroIntervention = intervention.Numero,
                DateFacturation = DateTime.UtcNow,
                TechnicienNom = intervention.TechnicienNom,
                CoutMainOeuvre = coutMainOeuvre,
                CoutPieces = coutPieces,
                MontantTotalHT = totalHT,
                TVA = tva,
                MontantTotalTTC = totalTTC,
                EstPayee = false,
                SousGarantie = false,
                Message = "Facture calculée avec succès"
            });
        }

        public Task<bool> GenererFactureAsync(Guid id, CalculFactureRequest request)
        {
            var intervention = _interventions.FirstOrDefault(i => i.Id == id);
            if (intervention == null) return Task.FromResult(false);

            if (intervention.SousGarantie)
                throw new Exception("Pas de facturation pour les interventions sous garantie");

            var facture = CalculerFactureAsync(id, request).Result;

            // Sauvegarder les informations de facture dans l'intervention
            intervention.CoutMainOeuvre = facture.CoutMainOeuvre;
            intervention.CoutPieces = facture.CoutPieces;
            intervention.MontantFacture = facture.MontantTotalTTC;
            intervention.DateFacturation = facture.DateFacturation;
            intervention.FacturePayee = false;
            intervention.UpdatedAt = DateTime.UtcNow;

            _logger.LogInformation("Facture générée pour l'intervention {Numero}: {Montant}€",
                intervention.Numero, intervention.MontantFacture);

            return Task.FromResult(true);
        }

        public Task<bool> MarquerFacturePayeeAsync(Guid id)
        {
            var intervention = _interventions.FirstOrDefault(i => i.Id == id);
            if (intervention == null) return Task.FromResult(false);

            if (!intervention.MontantFacture.HasValue)
                throw new Exception("Aucune facture existante pour cette intervention");

            intervention.FacturePayee = true;
            intervention.UpdatedAt = DateTime.UtcNow;

            _logger.LogInformation("Facture marquée comme payée pour l'intervention {Numero}",
                intervention.Numero);

            return Task.FromResult(true);
        }

        public Task<List<Intervention>> GetInterventionsByReclamationAsync(Guid reclamationId)
        {
            var interventions = _interventions
                .Where(i => i.ReclamationId == reclamationId)
                .OrderByDescending(i => i.DatePlanification)
                .ToList();

            return Task.FromResult(interventions);
        }

        public Task<List<Intervention>> GetInterventionsByStatutAsync(string statut)
        {
            var interventions = _interventions
                .Where(i => i.Statut == statut)
                .OrderByDescending(i => i.DatePlanification)
                .ToList();

            return Task.FromResult(interventions);
        }

        public List<(string Nom, string Specialite, decimal TauxHoraire)> GetTechniciensDisponibles()
        {
            return _techniciens;
        }
    }
}