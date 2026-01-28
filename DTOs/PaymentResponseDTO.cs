using MiChitra.Models;

namespace MiChitra.DTOs
{
    public class PaymentResponseDTO
    {
        public string TransactionId { get; set; } = string.Empty;
        public int TicketId { get; set; }
        public decimal Amount { get; set; }
        public PaymentStatus Status { get; set; }
        public DateTime PaymentDate { get; set; }
        public string Message { get; set; } = string.Empty;
    }
}