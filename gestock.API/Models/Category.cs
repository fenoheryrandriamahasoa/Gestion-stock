using System.ComponentModel.DataAnnotations;

namespace gestock.API.Models
{
    public class Category
    {
        public int CategoryId { get; set; }

        [Required]
        public string Name { get; set; } = string.Empty;

        public string Description { get; set; } = string.Empty;

        // Relation : Une catégorie contient plusieurs produits
        // (Optionnel mais pratique pour plus tard)
        public List<Product>? Products { get; set; }
    }
}