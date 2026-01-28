using MiChitra.Models;
using System.ComponentModel.DataAnnotations;

namespace MiChitra.DTOs
{
    public class PaymentRequestDTO
    {
        [Required]
        public int TicketId { get; set; }

        [Required]
        public PaymentMethod PaymentMethod { get; set; }

        [Required]
        public string CardNumber { get; set; } = string.Empty;

        [Required]
        public decimal Amount { get; set; }

        [Required]
        public string CardHolderName { get; set; } = string.Empty;

        [Required]
        public string ExpiryMonth { get; set; } = string.Empty;

        [Required]
        public string ExpiryYear { get; set; } = string.Empty;

        [Required]
        public string CVV { get; set; } = string.Empty;
    }
}