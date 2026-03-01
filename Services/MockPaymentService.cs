using MiChitra.Interfaces;
using MiChitra.Models;
using MiChitra.Data;
using MiChitra.DTOs;
using Microsoft.EntityFrameworkCore;

namespace MiChitra.Services
{
    public class MockPaymentService : IPaymentService
    {
        private readonly MiChitraDbContext _context;
        private readonly ILogger<MockPaymentService> _logger;

        public MockPaymentService(MiChitraDbContext context, ILogger<MockPaymentService> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<PaymentResponseDTO> ProcessPaymentAsync(PaymentRequestDTO request)
        {
            _logger.LogInformation("Processing payment for booking {TicketId} with amount {Amount}", request.TicketId, request.Amount);

            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                // Validate card expiry for card payments
                if ((request.PaymentMethod == PaymentMethod.CreditCard || request.PaymentMethod == PaymentMethod.DebitCard) && !string.IsNullOrEmpty(request.Expiry))
                {
                    if (!IsValidExpiry(request.Expiry))
                        throw new InvalidOperationException("Card has expired or invalid expiry date");
                }

                // Simulate processing payment delay
                await Task.Delay(1000);

                var ticket = await _context.Tickets.FirstOrDefaultAsync(t => t.TicketId == request.TicketId);
                if (ticket == null)
                    throw new InvalidOperationException("Ticket not found");

                if (ticket.Status == TicketStatus.Cancelled || ticket.Status == TicketStatus.Expired)
                    throw new InvalidOperationException($"Cannot pay for a ticket with status: {ticket.Status}");

                if (ticket.Status == TicketStatus.Booked)
                    throw new InvalidOperationException("Ticket is already paid");

                if (ticket.ReservationExpiry.HasValue && ticket.ReservationExpiry.Value <= DateTime.UtcNow)
                {
                    ticket.Status = TicketStatus.Expired;
                    ticket.UpdatedAt = DateTime.UtcNow;
                    await _context.SaveChangesAsync();
                    await transaction.CommitAsync();
                    throw new InvalidOperationException("Reservation expired. Please book again.");
                }

                if (request.Amount != ticket.TotalPrice)
                    throw new InvalidOperationException("Payment amount does not match ticket total");

                // Simulate payment success rate
                bool isSuccess = true;

                var payment = new Payment
                {
                    TicketId = request.TicketId,
                    Amount = request.Amount,
                    PaymentMethod = request.PaymentMethod,
                    PaymentStatus = isSuccess ? PaymentStatus.Completed : PaymentStatus.Failed,
                    TransactionId = GenerateTransactionId(),
                    PaymentDate = DateTime.UtcNow
                };

                _context.Payments.Add(payment);

                // Update ticket status to Booked if payment successful
                if (isSuccess)
                {
                    ticket.Status = TicketStatus.Booked;
                    ticket.ReservationExpiry = null;
                    ticket.UpdatedAt = DateTime.UtcNow;
                }

                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                var result = new PaymentResponseDTO
                {
                    TransactionId = payment.TransactionId,
                    TicketId = payment.TicketId,
                    Amount = payment.Amount,
                    PaymentDate = payment.PaymentDate,
                    Status = payment.PaymentStatus,
                    Message = isSuccess ? "Payment processed successfully" : "Payment failed"
                };

                _logger.LogInformation("Payment processed with ID: {PaymentId}, Status: {Status}", result.TransactionId, result.Status);
                return result;
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }

        public async Task<PaymentResponseDTO?> GetPaymentByIdAsync(string transactionId)
        {
            _logger.LogInformation("Retrieving payment information for ID: {PaymentId}", transactionId);

            return await _context.Payments
                .Where(p => p.TransactionId == transactionId)
                .Select(p => new PaymentResponseDTO
                {
                    TransactionId = p.TransactionId,
                    TicketId = p.TicketId,
                    Amount = p.Amount,
                    PaymentDate = p.PaymentDate,
                    Status = p.PaymentStatus,
                    Message = p.PaymentStatus == PaymentStatus.Completed
                                ? "Payment processed successfully"
                                : "Payment failed"
                })
                .FirstOrDefaultAsync();
        }

        public async Task<PaymentResponseDTO> ProcessRefundAsync(int ticketId) {
            _logger.LogInformation("Processing refund for ticket {TicketId}", ticketId);

            var payment = await _context.Payments.FirstOrDefaultAsync(p => p.TicketId == ticketId);

            if (payment == null || payment.PaymentStatus != PaymentStatus.Completed) {
                return new PaymentResponseDTO
                {
                    TicketId = ticketId,
                    Status = PaymentStatus.Failed,
                    Message = payment == null ? "Payment not found" : "Payment not completed"
                };
            }

            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                // Simulate refund processing delay
                await Task.Delay(500);

                // Simulate refund success rate as 95%
                var isSuccess = Random.Shared.NextDouble() > 0.05;

                if (isSuccess)
                {
                    payment.PaymentStatus = PaymentStatus.Refunded;
                    payment.UpdatedAt = DateTime.UtcNow;

                    // Update ticket status to Cancelled
                    var ticket = await _context.Tickets.FindAsync(ticketId);
                    if (ticket != null)
                    {
                        ticket.Status = TicketStatus.Cancelled;
                        ticket.UpdatedAt = DateTime.UtcNow;
                    }

                    await _context.SaveChangesAsync();
                    await transaction.CommitAsync();
                }
                else
                {
                    await transaction.RollbackAsync();
                }

                _logger.LogInformation("Refund processed for payment ID: {PaymentId}, Status: {Status}", payment.TransactionId, isSuccess ? PaymentStatus.Refunded : PaymentStatus.RefundFailed);

                return new PaymentResponseDTO
                {
                    TicketId = ticketId,
                    Amount = payment.Amount,
                    Status = isSuccess ? PaymentStatus.Refunded : PaymentStatus.RefundFailed,
                    Message = isSuccess ? "Refund processed successfully" : "Refund failed",
                    TransactionId = payment.TransactionId
                };
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }
    

        private static string GenerateTransactionId()
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            var random = new Random();
            return new string(Enumerable.Repeat(chars, 12)
                .Select(s => s[random.Next(s.Length)]).ToArray());
        }

        private static bool IsValidExpiry(string expiry)
        {
            if (string.IsNullOrEmpty(expiry) || !System.Text.RegularExpressions.Regex.IsMatch(expiry, @"^\d{2}/\d{2}$"))
                return false;
            
            var parts = expiry.Split('/');
            if (!int.TryParse(parts[0], out int month) || !int.TryParse(parts[1], out int year))
                return false;
            
            if (month < 1 || month > 12)
                return false;
            
            var currentYear = DateTime.UtcNow.Year % 100;
            var currentMonth = DateTime.UtcNow.Month;
            
            return year > currentYear || (year == currentYear && month >= currentMonth);
        }

    }
}