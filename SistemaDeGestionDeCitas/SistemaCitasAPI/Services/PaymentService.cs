using Microsoft.EntityFrameworkCore;
using SistemaCitasAPI.Data;
using SistemaCitasAPI.DTOs;
using SistemaCitasAPI.Models;

namespace SistemaCitasAPI.Services
{
    public class PaymentService : IPaymentService
    {
        private readonly AppDbContext _context;
        private readonly ILogger<PaymentService> _logger;

        public PaymentService(AppDbContext context, ILogger<PaymentService> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<ProcessPaymentResponseDto> ProcessPaymentAsync(CreatePaymentDto paymentDto)
        {
            try
            {
                // Verificar que la cita existe
                var appointment = await _context.Appointments
                    .Include(a => a.Service)
                    .FirstOrDefaultAsync(a => a.AppointmentId == paymentDto.AppointmentId);

                if (appointment == null)
                {
                    return new ProcessPaymentResponseDto
                    {
                        Success = false,
                        Message = "La cita no existe"
                    };
                }

                // Verificar que no haya un pago exitoso previo
                var existingPayment = await _context.Payments
                    .FirstOrDefaultAsync(p => p.AppointmentId == paymentDto.AppointmentId && p.Status == "Completado");

                if (existingPayment != null)
                {
                    return new ProcessPaymentResponseDto
                    {
                        Success = false,
                        Message = "Esta cita ya tiene un pago completado"
                    };
                }

                // Validar método de pago
                var validPaymentMethods = new[] { "Tarjeta de Crédito", "Tarjeta de Débito", "Efectivo", "Transferencia" };
                if (!validPaymentMethods.Contains(paymentDto.PaymentMethod))
                {
                    return new ProcessPaymentResponseDto
                    {
                        Success = false,
                        Message = "Método de pago no válido"
                    };
                }

                // Simulación de procesamiento de pago
                var paymentSuccess = SimulatePaymentProcessing(paymentDto);

                var payment = new Payment
                {
                    AppointmentId = paymentDto.AppointmentId,
                    Amount = paymentDto.Amount,
                    PaymentMethod = paymentDto.PaymentMethod,
                    Status = paymentSuccess ? "Completado" : "Fallido",
                    TransactionId = paymentSuccess ? GenerateTransactionId() : null,
                    PaymentDate = DateTime.UtcNow
                };

                _context.Payments.Add(payment);
                await _context.SaveChangesAsync();

                _logger.LogInformation($"Pago procesado: {payment.TransactionId} - Estado: {payment.Status}");

                return new ProcessPaymentResponseDto
                {
                    Success = paymentSuccess,
                    Message = paymentSuccess ? "Pago procesado exitosamente" : "El pago fue rechazado",
                    Payment = MapToDto(payment),
                    TransactionId = payment.TransactionId
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al procesar el pago");
                return new ProcessPaymentResponseDto
                {
                    Success = false,
                    Message = "Error al procesar el pago"
                };
            }
        }

        public async Task<ProcessPaymentResponseDto> RefundPaymentAsync(RefundPaymentDto refundDto)
        {
            try
            {
                var payment = await _context.Payments
                    .FirstOrDefaultAsync(p => p.PaymentId == refundDto.PaymentId);

                if (payment == null)
                {
                    return new ProcessPaymentResponseDto
                    {
                        Success = false,
                        Message = "El pago no existe"
                    };
                }

                if (payment.Status != "Completado")
                {
                    return new ProcessPaymentResponseDto
                    {
                        Success = false,
                        Message = "Solo se pueden reembolsar pagos completados"
                    };
                }

                // Simulación de reembolso
                payment.Status = "Reembolsado";
                await _context.SaveChangesAsync();

                _logger.LogInformation($"Reembolso procesado para pago: {payment.TransactionId}");

                return new ProcessPaymentResponseDto
                {
                    Success = true,
                    Message = "Reembolso procesado exitosamente",
                    Payment = MapToDto(payment)
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al procesar el reembolso");
                return new ProcessPaymentResponseDto
                {
                    Success = false,
                    Message = "Error al procesar el reembolso"
                };
            }
        }

        public async Task<PaymentDto?> GetPaymentByIdAsync(int paymentId)
        {
            var payment = await _context.Payments
                .FirstOrDefaultAsync(p => p.PaymentId == paymentId);

            return payment != null ? MapToDto(payment) : null;
        }

        public async Task<List<PaymentHistoryDto>> GetPaymentsByUserIdAsync(int userId)
        {
            var payments = await _context.Payments
                .Include(p => p.Appointment)
                    .ThenInclude(a => a.Service)
                .Where(p => p.Appointment.UserId == userId)
                .OrderByDescending(p => p.PaymentDate)
                .Select(p => new PaymentHistoryDto
                {
                    PaymentId = p.PaymentId,
                    AppointmentId = p.AppointmentId,
                    ServiceName = p.Appointment.Service.ServiceName,
                    Amount = p.Amount,
                    PaymentMethod = p.PaymentMethod,
                    Status = p.Status,
                    TransactionId = p.TransactionId,
                    PaymentDate = p.PaymentDate,
                    AppointmentDate = p.Appointment.AppointmentDate
                })
                .ToListAsync();

            return payments;
        }

        public async Task<List<PaymentHistoryDto>> GetPaymentsByAppointmentIdAsync(int appointmentId)
        {
            var payments = await _context.Payments
                .Include(p => p.Appointment)
                    .ThenInclude(a => a.Service)
                .Where(p => p.AppointmentId == appointmentId)
                .OrderByDescending(p => p.PaymentDate)
                .Select(p => new PaymentHistoryDto
                {
                    PaymentId = p.PaymentId,
                    AppointmentId = p.AppointmentId,
                    ServiceName = p.Appointment.Service.ServiceName,
                    Amount = p.Amount,
                    PaymentMethod = p.PaymentMethod,
                    Status = p.Status,
                    TransactionId = p.TransactionId,
                    PaymentDate = p.PaymentDate,
                    AppointmentDate = p.Appointment.AppointmentDate
                })
                .ToListAsync();

            return payments;
        }

        public async Task<PaymentDto?> GetPaymentByTransactionIdAsync(string transactionId)
        {
            var payment = await _context.Payments
                .FirstOrDefaultAsync(p => p.TransactionId == transactionId);

            return payment != null ? MapToDto(payment) : null;
        }

        public async Task<bool> VerifyPaymentStatusAsync(int paymentId)
        {
            var payment = await _context.Payments
                .FirstOrDefaultAsync(p => p.PaymentId == paymentId);

            return payment?.Status == "Completado";
        }

        // Métodos privados de simulación
        private bool SimulatePaymentProcessing(CreatePaymentDto paymentDto)
        {
            // Simulación: 95% de éxito
            var random = new Random();
            var success = random.Next(100) < 95;

            // Simular validaciones adicionales
            if (paymentDto.PaymentMethod.Contains("Tarjeta"))
            {
                // Validar número de tarjeta (simulado)
                if (!string.IsNullOrEmpty(paymentDto.CardNumber) && paymentDto.CardNumber.Length < 13)
                {
                    return false;
                }
            }

            // Simular delay de procesamiento
            Thread.Sleep(500);

            return success;
        }

        private string GenerateTransactionId()
        {
            return $"TXN-{DateTime.UtcNow:yyyyMMddHHmmss}-{Guid.NewGuid().ToString().Substring(0, 8).ToUpper()}";
        }

        private PaymentDto MapToDto(Payment payment)
        {
            return new PaymentDto
            {
                PaymentId = payment.PaymentId,
                AppointmentId = payment.AppointmentId,
                Amount = payment.Amount,
                PaymentMethod = payment.PaymentMethod,
                Status = payment.Status,
                TransactionId = payment.TransactionId,
                PaymentDate = payment.PaymentDate
            };
        }
    }
}
