using InterventionService.Models.Entities;
using Microsoft.EntityFrameworkCore;

namespace InterventionService.Data
{
    public static class SeedData
    {
        public static void Initialize(IServiceProvider serviceProvider)
        {
            using (var context = new InterventionDbContext(
                serviceProvider.GetRequiredService<DbContextOptions<InterventionDbContext>>()))
            {
                if (context.Interventions.Any())
                {
                    return;
                }

                context.Interventions.AddRange(
                    new Intervention
                    {
                        ReclamationId = 1,
                        TechnicianId = 101,
                        TechnicianName = "Jean Dupont",
                        InterventionDate = DateTime.Now.AddDays(2),
                        Description = "Remplacement du thermostat défectueux",
                        Status = "Planifiée",
                        IsUnderWarranty = true,
                        Priority = "Haute",
                        Location = "123 Rue de Paris, 75001",
                        EstimatedHours = 3,
                        ArticleReference = "TH-1234"
                    },
                    new Intervention
                    {
                        ReclamationId = 2,
                        TechnicianId = 102,
                        TechnicianName = "Marie Curie",
                        InterventionDate = DateTime.Now.AddDays(1),
                        Description = "Réparation de la chaudière - Fuite détectée",
                        Status = "EnCours",
                        IsUnderWarranty = false,
                        PartsCost = 150.50m,
                        LaborCost = 200.00m,
                        Priority = "Urgente",
                        Location = "456 Avenue Victor Hugo, 75016",
                        EstimatedHours = 5,
                        ArticleReference = "CH-5678"
                    }
                );

                context.SaveChanges();
            }
        }
    }
}