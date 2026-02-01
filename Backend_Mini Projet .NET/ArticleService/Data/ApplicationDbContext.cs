using Microsoft.EntityFrameworkCore;
using ArticleService.Models;

namespace ArticleService.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Article> Articles { get; set; }
        public DbSet<Category> Categories { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Configuration pour Article
            modelBuilder.Entity<Article>(entity =>
            {
                entity.HasIndex(a => a.Reference).IsUnique();

                entity.Property(a => a.PrixAchat)
                    .HasColumnType("decimal(18,2)");

                entity.Property(a => a.PrixVente)
                    .HasColumnType("decimal(18,2)");

                // Relation avec Category
                entity.HasOne(a => a.Category)
                    .WithMany(c => c.Articles)
                    .HasForeignKey(a => a.CategoryId)
                    .OnDelete(DeleteBehavior.SetNull);
            });

            // Configuration pour Category
            modelBuilder.Entity<Category>(entity =>
            {
                entity.HasIndex(c => c.Name).IsUnique();

                entity.Property(c => c.Name)
                    .IsRequired()
                    .HasMaxLength(100);

                entity.Property(c => c.Description)
                    .HasMaxLength(500);
            });
        }
    }
}