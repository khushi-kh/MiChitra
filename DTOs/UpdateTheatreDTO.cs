using System.ComponentModel.DataAnnotations;

namespace MiChitra.DTOs
{
    public class UpdateTheatreDTO
    {
        [Required]
        [StringLength(100, MinimumLength = 1)]
        public string Name { get; set; } = string.Empty;
        
        [Required]
        [StringLength(50, MinimumLength = 1)]
        public string City { get; set; } = string.Empty;
    }
}
