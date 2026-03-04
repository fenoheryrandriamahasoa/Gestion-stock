namespace SuperMarcheApp.Models
{
    public class UserDto
    {
        public int UserId { get; set; }
        public string Username { get; set; } = string.Empty;
        public string Role { get; set; } = string.Empty;
        public bool IsActive { get; set; }
        public override string ToString() => Username;

        // Affichage du rôle en français
        public string RoleDisplay => Role switch
        {
            "Seller" => "Vendeur",
            "Admin" => "Admin",
            _ => Role
        };
    }

    // Pour POST/PUT
    public class UserRequest
    {
        public int UserId { get; set; }
        public string Username { get; set; } = string.Empty;
        public string PasswordHash { get; set; } = string.Empty;
        public string Role { get; set; } = "Seller";
        public bool IsActive { get; set; } = true;
    }
}