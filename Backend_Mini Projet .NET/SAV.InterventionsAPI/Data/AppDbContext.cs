using Microsoft.EntityFrameworkCore;
using SAV.InterventionsAPI.Models; // si tes modèles sont ici

namespace SAV.InterventionsAPI.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options)
        {
        }

        // Ajoute ici les DbSet
        // Exemple :
        // public DbSet<Intervention> Interventions { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Ajoute ici la configuration si nécessaire
        }
    }
}
