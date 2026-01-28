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

            // Simulate processing payment delay
            await Task.Delay(1000);

            // Simulate payment success rate
            bool isSuccess = true;

            var payment = new Payment
            {
                TicketId = request.TicketId,
                Amount = request.Amount,
                PaymentMethod = request.PaymentMethod,
                PaymentStatus = isSuccess ? PaymentStatus.Completed : PaymentStatus.Failed,
                TransactionId = GenerateTransactionIdAsync(),
                PaymentDate = DateTime.UtcNow
            };

            _context.Payments.Add(payment);
            await _context.SaveChangesAsync();

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

            var payment = await _context.Payments.FindAsync(ticketId);

            if (payment == null || payment.PaymentStatus != PaymentStatus.Completed) {
                return new PaymentResponseDTO
                {
                    TicketId = ticketId,
                    Status = PaymentStatus.Failed,
                    Message = payment == null ? "Payment not found" : "Payment not completed"
                };
            }

            // Simulate refund processing delay
            await Task.Delay(500);

            // Simulate refund success rate as 95%
            var isSuccess = Random.Shared.NextDouble() > 0.05;

            if (isSuccess)
            {
                payment.PaymentStatus = PaymentStatus.Refunded;
                payment.UpdatedAt = DateTime.UtcNow;
                await _context.SaveChangesAsync();
            }

            _logger.LogInformation("Refund processed for payment ID: {PaymentId}, Status: {Status}", payment.TransactionId, PaymentStatus.Refunded);

            return new PaymentResponseDTO
            {
                TicketId = ticketId,
                Amount = payment.Amount,
                Status = isSuccess ? PaymentStatus.Refunded : PaymentStatus.RefundFailed,
                Message = isSuccess ? "Refund processed successfully" : "Refund failed",
                TransactionId = payment.TransactionId
            };
        }
    

        private static string GenerateTransactionIdAsync()
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            var random = new Random();
            return new string(Enumerable.Repeat(chars, 12)
                .Select(s => s[random.Next(s.Length)]).ToArray());
        }

    }
}