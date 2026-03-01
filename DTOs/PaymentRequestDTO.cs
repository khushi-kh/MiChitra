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

            if (!IsValidLuhn(CardNumber))
                yield return new ValidationResult("Invalid card number", new[] { nameof(CardNumber) });
        }

        private static bool IsValidLuhn(string? cardNumber)
        {
            if (string.IsNullOrWhiteSpace(cardNumber))
                return false;

            var digits = cardNumber.Replace(" ", "").Replace("-", "");
            if (!digits.All(char.IsDigit))
                return false;

            int sum = 0;
            bool alternate = false;
            for (int i = digits.Length - 1; i >= 0; i--)
            {
                int n = digits[i] - '0';
                if (alternate)
                {
                    n *= 2;
                    if (n > 9)
                        n -= 9;
                }
                sum += n;
                alternate = !alternate;
            }
            return sum % 10 == 0;
        }
    }
}