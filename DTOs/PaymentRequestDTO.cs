using MiChitra.Models;
using System.ComponentModel.DataAnnotations;

namespace MiChitra.DTOs
{
    public class PaymentRequestDTO : IValidatableObject
    {
        [Required]
        public int TicketId { get; set; }

        [Required]
        public PaymentMethod PaymentMethod { get; set; }

        public string? CardNumber { get; set; }

        [Required]
        public decimal Amount { get; set; }

        public string? CardHolderName { get; set; }

        public string? Expiry { get; set; }

        public string? CVV { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            var requiresCard =
                PaymentMethod == PaymentMethod.CreditCard ||
                PaymentMethod == PaymentMethod.DebitCard;

            if (!requiresCard)
                yield break;

            if (string.IsNullOrWhiteSpace(CardNumber))
                yield return new ValidationResult("Card number is required for card payments", new[] { nameof(CardNumber) });

            if (string.IsNullOrWhiteSpace(CardHolderName))
                yield return new ValidationResult("Card holder name is required for card payments", new[] { nameof(CardHolderName) });

            if (string.IsNullOrWhiteSpace(Expiry))
                yield return new ValidationResult("Expiry is required for card payments", new[] { nameof(Expiry) });

            if (string.IsNullOrWhiteSpace(CVV))
                yield return new ValidationResult("CVV is required for card payments", new[] { nameof(CVV) });
        }
    }
}