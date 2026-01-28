using MiChitra.Models;

namespace MiChitra.DTOs
{
    public class UserResponseDTO
    {
        public int UserId { get; set; }
        public string FName { get; set; } = string.Empty;
        public string LName { get; set; } = string.Empty;
        public string Username { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string? ContactNumber { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? LastLoginAt { get; set; }
        public bool IsActive { get; set; }

        // Added role so AuthService/UserService can populate it
        public UserRole Role { get; set; }
    }
}