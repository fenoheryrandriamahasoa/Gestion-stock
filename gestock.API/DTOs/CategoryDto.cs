namespace gestock.API.DTOs
{
    public class CategoryDto
    {
        public int CategoryId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public int ProductCount { get; set; }  // Nombre d'articles
        public List<ProductDto>? Products { get; set; }
    }
}