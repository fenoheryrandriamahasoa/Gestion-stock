using Microsoft.EntityFrameworkCore;
using gestock.API.Models;

namespace gestock.API.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        // ══════════════════════════════════════════
        //  Tables de la base de données
        // ══════════════════════════════════════════
        public DbSet<Product> Products { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<StockMovement> StockMovements { get; set; }
        public DbSet<Sale> Sales { get; set; }
        public DbSet<SaleDetail> SaleDetails { get; set; }

        // ══════════════════════════════════════════
        //  Configuration des relations + Seed Data
        // ══════════════════════════════════════════
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // ── Relations Product → Category ──
            modelBuilder.Entity<Product>()
                .HasOne(p => p.Category)
                .WithMany(c => c.Products)
                .HasForeignKey(p => p.CategoryId)
                .OnDelete(DeleteBehavior.Restrict);  // Empêche suppression cascade

            // ── Relations SaleDetail → Sale ──
            modelBuilder.Entity<SaleDetail>()
                .HasOne(sd => sd.Sale)
                .WithMany(s => s.SaleDetails)
                .HasForeignKey(sd => sd.SaleId)
                .OnDelete(DeleteBehavior.Cascade);   // Supprimer une vente → supprime ses détails

            // ── Relations SaleDetail → Product ──
            modelBuilder.Entity<SaleDetail>()
                .HasOne(sd => sd.Product)
                .WithMany()
                .HasForeignKey(sd => sd.ProductId)
                .OnDelete(DeleteBehavior.Restrict); 

            // ── Relations Sale → User ──
            modelBuilder.Entity<Sale>()
                .HasOne(s => s.User)
                .WithMany()
                .HasForeignKey(s => s.UserId)
                .OnDelete(DeleteBehavior.Restrict); 

            // ── Relations StockMovement → Product ──
            modelBuilder.Entity<StockMovement>()
                .HasOne(sm => sm.Product)
                .WithMany()
                .HasForeignKey(sm => sm.ProductId)
                .OnDelete(DeleteBehavior.Restrict);

            // ── Index unique sur Username ──
            modelBuilder.Entity<User>()
                .HasIndex(u => u.Username)
                .IsUnique();

            // ══════════════════════════════════════════
            //  SEED DATA : Données initiales
            // ══════════════════════════════════════════

            // Admin par défaut (mot de passe: "admin123")
            // Hash généré avec BCrypt.Net.BCrypt.HashPassword("admin123")
            modelBuilder.Entity<User>().HasData(new User
            {
                UserId = 1,
                Username = "admin",
                PasswordHash = "$2a$11$v8pwCwQ4kmg2Ro7vl/w7se2vT/RJDp/cyj0fwBhH1kuojm6ZkYWtS",
                Role = "Admin",
                IsActive = true
            });

            // ✅ Catégories par défaut
            modelBuilder.Entity<Category>().HasData(
                new Category { CategoryId = 1, Name = "Boissons", Description = "Sodas, jus, eau" },
                new Category { CategoryId = 2, Name = "Produits laitiers", Description = "Lait, fromage, yaourt" },
                new Category { CategoryId = 3, Name = "Fruits et légumes", Description = "Produits frais" },
                new Category { CategoryId = 4, Name = "Épicerie", Description = "Riz, pâtes, conserves" },
                new Category { CategoryId = 5, Name = "Hygiène", Description = "Savon, dentifrice, etc." }
            );
        }
    }
}