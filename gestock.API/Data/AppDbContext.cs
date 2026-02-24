using Microsoft.EntityFrameworkCore;
using gestock.API.Models; // Important : pour qu'il reconnaisse la classe Article

namespace gestock.API.Data
{
    // Cette classe hérite de "DbContext", c'est le chef d'orchestre de la base de données
    public class AppDbContext : DbContext
    {
        // Le constructeur : indispensable pour recevoir la configuration (comme le lien SQL)
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        // C'est ici qu'on transforme tes classes C# en Tables SQL
        // "DbSet<Article>" veut dire : "Crée-moi une table qui contiendra des Articles"
        public DbSet<Product> Products{ get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<StockMovement> StockMovements { get; set; }
        public DbSet<Sale> Sales { get; set; }
        public DbSet<SaleDetail> SaleDetails { get; set; }
    }
}