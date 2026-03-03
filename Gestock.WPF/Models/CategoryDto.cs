using System.Collections.Generic;

namespace SuperMarcheApp.Models
{
    public class CategoryDto
    {
        public int CategoryId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public int ProductCount { get; set; }
        public override string ToString() => Name;
    }

    // Pour POST/PUT
    public class CategoryRequest
    {
        public int CategoryId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
    }
}