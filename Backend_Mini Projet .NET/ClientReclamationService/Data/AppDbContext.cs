// Data/AppDbContext.cs
using ClientReclamationService.Models;
using Microsoft.EntityFrameworkCore;

namespace ClientReclamationService.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        public DbSet<Client> Clients { get; set; }
        public DbSet<Reclamation> Reclamations { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configuration Client
            modelBuilder.Entity<Client>(entity =>
            {
                entity.HasIndex(c => c.Email).IsUnique();
                entity.HasIndex(c => c.UserId).IsUnique();

                entity.HasMany(c => c.Reclamations)
                      .WithOne(r => r.Client)
                      .HasForeignKey(r => r.ClientId)
                      .OnDelete(DeleteBehavior.Cascade);
            });

            // Configuration Reclamation
            modelBuilder.Entity<Reclamation>(entity =>
            {
                entity.HasIndex(r => r.Reference).IsUnique();

                entity.Property(r => r.Statut)
                      .HasConversion<string>();

                entity.Property(r => r.Priorite)
                      .HasConversion<string>();

                // Calculer la date de fin de garantie
                entity.Property(r => r.DateFinGarantie)
                      .HasComputedColumnSql("DATEADD(MONTH, 24, [DateAchat])", stored: true);
            });
        }
    }
}