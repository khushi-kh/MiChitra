using System.ComponentModel.DataAnnotations;
using MiChitra.Attributes;
using MiChitra.Models;

namespace MiChitra.DTOs
{
    public class RegisterDTO
    {
        [Required]
        public string Username { get; set; } = string.Empty;
        [Required, EmailAddress]
        public string Email { get; set; } = string.Empty;
        [Required, PasswordStrength]
        public string Password { get; set; } = string.Empty;

        // Added: registration details expected by UserService
        public string? FName { get; set; }
        public string? LName { get; set; }
        public UserRole Role { get; set; } = UserRole.User;
        public string? ContactNumber { get; set; }
    }
}