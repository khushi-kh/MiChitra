using System.ComponentModel.DataAnnotations;

namespace MiChitra.Models
{
    public class Theatre
    {
        [Key]
        public int TheatreId { get; set; }
        [Required]
        public string Name { get; set; }
        [Required]
        public string City { get; set; }
        public ICollection<MovieShow> MovieShows { get; set; } = new List<MovieShow>();
    }
}
