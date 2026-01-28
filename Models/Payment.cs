namespace MiChitra.Models
{
    public class Payment
    {
        public int Id { get; set; }
        public string TransactionId { get; set; } = string.Empty;
        public int TicketId { get; set; }
        public decimal Amount { get; set; }
        public DateTime PaymentDate { get; set; }
        public PaymentMethod PaymentMethod { get; set; }
        public PaymentStatus PaymentStatus { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        // Navigation property
        public Ticket Ticket { get; set; } = null!;
    }

    public enum PaymentMethod
    {
        CreditCard,
        DebitCard,
        NetBanking,
        UPI
    }

    public enum PaymentStatus
    {
        Pending,
        Completed,
        Failed,
        Refunded,
        RefundFailed
    }
}
