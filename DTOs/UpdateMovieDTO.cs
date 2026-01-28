using System.ComponentModel.DataAnnotations;

namespace MiChitra.DTOs
{
    public class UpdateMovieDto
    {
        [Required]
        [StringLength(100, MinimumLength = 1)]
        public string MovieName { get; set; } = string.Empty;
        
        [StringLength(1000)]
        public string Description { get; set; } = string.Empty;
        
        [Required]
        [StringLength(50, MinimumLength = 1)]
        public string Language { get; set; } = string.Empty;
        
        [Range(0, 10)]
        public decimal Rating { get; set; }
    }
}
