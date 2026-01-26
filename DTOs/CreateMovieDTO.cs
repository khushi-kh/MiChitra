using System.ComponentModel.DataAnnotations;

namespace MiChitra.DTOs
{
    public class CreateMovieDto
    {
        [Required]
        public string MovieName { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        [Required]
        public string Language { get; set; } = string.Empty;
        [Range(0, 10)]
        public decimal Rating { get; set; }
    }
}
