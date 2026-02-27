using System.ComponentModel.DataAnnotations;

namespace MiChitra.Models
{
    public class Ticket
    {
        [Key]
        public int TicketId { get; set; }

        // Foreign Keys
        public int UserId { get; set; }
        public int MovieShowId { get; set; }

        // Navigation Properties
        public User? User { get; set; }
        public MovieShow? MovieShow { get; set; }

        public DateTime BookingDate { get; set; } = DateTime.UtcNow;
        public int NumberOfSeats { get; set; }
        public string? SeatNumbers { get; set; }
        public decimal TotalPrice { get; set; }
        public TicketStatus Status { get; set; } = TicketStatus.Reserved;
        public DateTime? ReservationExpiry { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    }

    public enum TicketStatus
    {
        Reserved,
        Booked,
        Completed,
        Cancelled,
        Expired
    }
}
