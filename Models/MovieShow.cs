using System.ComponentModel.DataAnnotations;

namespace MiChitra.Models
{
    public class MovieShow
    {
        [Key]
        public int Id { get; set; }
        public int MovieId { get; set; }
        public Movie? Movie { get; set; }
        public int TheatreId { get; set; }
        public Theatre? Theatre { get; set; }
        public DateTime ShowTime { get; set; }
        public int TotalSeats { get; set; }
        public int AvailableSeats { get; set; }
        public MovieShowStatus Status { get; set; } = MovieShowStatus.Available;

        // Navigation Properties
        public ICollection<Ticket> Tickets { get; set; } = new List<Ticket>();
    }
    public enum MovieShowStatus
    {
        Available,
        AlmostFull,
        SoldOut,
    }
}


