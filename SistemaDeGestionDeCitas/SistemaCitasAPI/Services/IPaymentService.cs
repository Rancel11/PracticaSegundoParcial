using SistemaCitasAPI.DTOs;

namespace SistemaCitasAPI.Services
{
    public interface IPaymentService
    {
        Task<ProcessPaymentResponseDto> ProcessPaymentAsync(CreatePaymentDto paymentDto);
        Task<ProcessPaymentResponseDto> RefundPaymentAsync(RefundPaymentDto refundDto);
        Task<PaymentDto?> GetPaymentByIdAsync(int paymentId);
        Task<List<PaymentHistoryDto>> GetPaymentsByUserIdAsync(int userId);
        Task<List<PaymentHistoryDto>> GetPaymentsByAppointmentIdAsync(int appointmentId);
        Task<PaymentDto?> GetPaymentByTransactionIdAsync(string transactionId);
        Task<bool> VerifyPaymentStatusAsync(int paymentId);
    }
}
