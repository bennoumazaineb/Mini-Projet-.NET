using InterventionService.Models.Entities;
using Microsoft.EntityFrameworkCore;

namespace InterventionService.Data
{
    public class InterventionDbContext : DbContext
    {
        public InterventionDbContext(DbContextOptions<InterventionDbContext> options) : base(options) { }

        public DbSet<Intervention> Interventions { get; set; }
        public DbSet<Technician> Technicians { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Intervention>(entity =>
            {
                entity.HasIndex(i => i.ReclamationId);
                entity.HasIndex(i => i.TechnicianId);
                entity.HasIndex(i => i.Status);
                entity.HasIndex(i => i.InterventionDate);

                entity.Property(i => i.Status)
                    .HasConversion<string>();

                entity.Property(i => i.Priority)
                    .HasConversion<string>();

                entity.Property(i => i.PartsCost)
                    .HasPrecision(10, 2);

                entity.Property(i => i.LaborCost)
                    .HasPrecision(10, 2);
            });

            modelBuilder.Entity<Technician>(entity =>
            {
                entity.HasIndex(t => t.UserId).IsUnique();
                entity.HasIndex(t => t.IsActive);
            });
        }
    }
}