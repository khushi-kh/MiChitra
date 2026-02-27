using MiChitra.Models;

namespace MiChitra.DTOs
{
    public class TicketResponseDTO
    {
        public int UserId { get; set; }
        public int TicketId { get; set; }
        public DateTime BookingDate { get; set; }
        public int NumberOfSeats { get; set; }
        public decimal TotalPrice { get; set; }
        public TicketStatus Status { get; set; }

        public int MovieShowId { get; set; }
        public DateTime ShowTime { get; set; }
        public decimal PricePerSeat { get; set; }

        public string? MovieName { get; set; }
        public string? TheatreName { get; set; }
        public string? TransactionId { get; set; }
    }
}
