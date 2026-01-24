using System.ComponentModel.DataAnnotations;

namespace MiChitra.Models
{
    public class User
    {
        [Key]
        public int UserId { get; set; }
        [Required]
        public string FName { get; set; } = string.Empty;
        [Required]
        public string LName { get; set; }  = string.Empty;
        [Required]
        public string Username { get; set; } = string.Empty;
        [EmailAddress, Required]
        public string Email { get; set; } = string.Empty;
        [Required]
        public string PasswordHash { get; set; } = string.Empty;
        [Required]
        public UserRole Role { get; set; }
        public string? ContactNumber { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? LastLoginAt { get; set; } = DateTime.UtcNow;
        public bool IsActive { get; set; } = true;

        // Navigation Properties
        public ICollection<Ticket> Tickets { get; set; } = new List<Ticket>();
    }
    public enum UserRole
    {
        Admin,
        User,
        Guest
    }

}


