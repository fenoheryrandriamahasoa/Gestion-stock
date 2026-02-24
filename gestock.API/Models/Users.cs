using System.ComponentModel.DataAnnotations;

namespace gestock.API.Models
{
    public class User
    {
        public int UserId { get; set; }

        [Required]
        public string Username { get; set; } = string.Empty;

        [Required]
        public string PasswordHash { get; set; } = string.Empty;

        public string Role { get; set; } = "Seller"; 
        public bool IsActive { get; set; } = true;
    }
}