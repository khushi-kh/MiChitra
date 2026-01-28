using MiChitra.DTOs;

namespace MiChitra.Interfaces
{
    public interface IPaymentService
    {
        Task<PaymentResponseDTO> ProcessPaymentAsync(PaymentRequestDTO request);
        Task<PaymentResponseDTO?> GetPaymentByIdAsync(string transactionId);
        Task<PaymentResponseDTO> ProcessRefundAsync(int ticketId);
    }
}