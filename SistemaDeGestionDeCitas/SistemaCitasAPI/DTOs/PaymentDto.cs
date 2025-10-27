namespace SistemaCitasAPI.DTOs
{
    public class PaymentDto
    {
        public int PaymentId { get; set; }
        public int AppointmentId { get; set; }
        public decimal Amount { get; set; }
        public string PaymentMethod { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public string? TransactionId { get; set; }
        public DateTime PaymentDate { get; set; }
    }

    public class CreatePaymentDto
    {
        public int AppointmentId { get; set; }
        public decimal Amount { get; set; }
        public string PaymentMethod { get; set; } = string.Empty;
        public string? CardNumber { get; set; }
        public string? CardHolderName { get; set; }
        public string? ExpiryDate { get; set; }
        public string? CVV { get; set; }
    }

    public class ProcessPaymentResponseDto
    {
        public bool Success { get; set; }
        public string Message { get; set; } = string.Empty;
        public PaymentDto? Payment { get; set; }
        public string? TransactionId { get; set; }
    }

    public class RefundPaymentDto
    {
        public int PaymentId { get; set; }
        public string Reason { get; set; } = string.Empty;
    }

    public class PaymentHistoryDto
    {
        public int PaymentId { get; set; }
        public int AppointmentId { get; set; }
        public string ServiceName { get; set; } = string.Empty;
        public decimal Amount { get; set; }
        public string PaymentMethod { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public string? TransactionId { get; set; }
        public DateTime PaymentDate { get; set; }
        public DateTime AppointmentDate { get; set; }
    }
}
