using Microservice2_Reclamations.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Reflection.Emit;

namespace Microservice2_Reclamations.Data
{
    public class ReclamationDbContext : DbContext
    {
        public ReclamationDbContext(DbContextOptions<ReclamationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Reclamation> Reclamations { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Reclamation>()
                .Property(r => r.Statut)
                .HasConversion<int>();

            modelBuilder.Entity<Reclamation>()
                .HasIndex(r => r.ClientId);

            modelBuilder.Entity<Reclamation>()
                .HasIndex(r => r.Statut);

            modelBuilder.Entity<Reclamation>()
                .HasIndex(r => r.DateCreation);
        }
    }
}