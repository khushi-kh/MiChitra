using System.ComponentModel.DataAnnotations;

namespace MiChitra.DTOs
{
    public class UpdateUserDTO
    {
        [StringLength(50)]
        public string? FName { get; set; }
        
        [StringLength(50)]
        public string? LName { get; set; }
        
        [EmailAddress]
        public string? Email { get; set; }
        
        [Phone]
        public string? ContactNumber { get; set; }
    }
}

