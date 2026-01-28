using System.ComponentModel.DataAnnotations;

namespace MiChitra.Models
{
    public class Movie
    {
        [Key]
        public int MovieId { get; set; }
        [Required]
        public string MovieName { get; set; } = string.Empty;

        public string Description { get; set; } = string.Empty;
        public string Language { get; set; } = string.Empty;
        public decimal Rating { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        // Navigation Properties
        public ICollection<MovieShow> MovieShows { get; set; } = new List<MovieShow>();

    }

}