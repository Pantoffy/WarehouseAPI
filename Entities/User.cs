using System.ComponentModel.DataAnnotations.Schema;

namespace WarehouseAPI.Entities
{
    public class User
    {
        public int Id { get; set; }
        public string Username { get; set; } = string.Empty;

        [Column("password")]
        public string PasswordHash { get; set; } = string.Empty;

        public string Role { get; set; } = string.Empty;
        public string? RefreshToken { get; set; }
        public DateTime? RefreshTokenExpiryTime { get; set; }
    }
}
