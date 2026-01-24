namespace MiChitra.DTOs
{
    public class UpdateMovieShowDto
    {
        public DateTime ShowTime { get; set; }
        public int TotalSeats { get; set; }
        public int AvailableSeats { get; set; }
        public decimal PricePerSeat { get; set; }
    }
}
