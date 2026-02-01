using Microsoft.EntityFrameworkCore;
using ReclamationService.Models;

namespace ReclamationService.Data
{
    public class ReclamationDbContext : DbContext
    {
        public ReclamationDbContext(DbContextOptions<ReclamationDbContext> options) : base(options)
        {
        }

        public DbSet<Reclamation> Reclamations { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Reclamation>(entity =>
            {
                entity.HasIndex(r => r.Reference).IsUnique();
                entity.HasIndex(r => r.ClientUserId);
                entity.HasIndex(r => r.Statut);
                entity.HasIndex(r => r.DateCreation);

                entity.Property(r => r.Statut)
                      .HasConversion<string>();

                entity.Property(r => r.Priorite)
                      .HasConversion<string>();
            });
        }
    }
}