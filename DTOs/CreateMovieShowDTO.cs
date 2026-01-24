namespace MiChitra.DTOs
{
    public class CreateMovieShowDto
    {
        public int MovieId { get; set; }
        public int TheatreId { get; set; }
        public DateTime ShowTime { get; set; }
        public int TotalSeats { get; set; }
        public decimal PricePerSeat { get; set; }
    }
}
