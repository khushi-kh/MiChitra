using MiChitra.Models;

namespace MiChitra.DTOs
{
    public class MovieShowResponseDTO
    {
        public int Id { get; set; }
        public int MovieId { get; set; }
        public string MovieName { get; set; } = string.Empty;
        public int TheatreId { get; set; }
        public string TheatreName { get; set; } = string.Empty;
        public string City { get; set; } = string.Empty;
        public DateTime ShowTime { get; set; }
        public int TotalSeats { get; set; }
        public int AvailableSeats { get; set; }
        public decimal PricePerSeat { get; set; }
        public MovieShowStatus Status { get; set; }
    }
}