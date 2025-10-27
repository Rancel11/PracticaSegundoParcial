using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SistemaCitasAPI.DTOs;
using SistemaCitasAPI.Services;
using System.Security.Claims;

namespace SistemaCitasAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class PaymentsController : ControllerBase
    {
        private readonly IPaymentService _paymentService;
        private readonly ILogger<PaymentsController> _logger;

        public PaymentsController(IPaymentService paymentService, ILogger<PaymentsController> logger)
        {
            _paymentService = paymentService;
            _logger = logger;
        }

        /// <summary>
        /// Procesar un nuevo pago (simulado)
        /// </summary>
        [HttpPost("process")]
        public async Task<ActionResult<ProcessPaymentResponseDto>> ProcessPayment([FromBody] CreatePaymentDto paymentDto)
        {
            try
            {
                var result = await _paymentService.ProcessPaymentAsync(paymentDto);

                if (!result.Success)
                {
                    return BadRequest(result);
                }

                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al procesar el pago");
                return StatusCode(500, new { message = "Error interno del servidor" });
            }
        }

        /// <summary>
        /// Procesar un reembolso (simulado)
        /// </summary>
        [HttpPost("refund")]
        [Authorize(Roles = "Administrador")]
        public async Task<ActionResult<ProcessPaymentResponseDto>> RefundPayment([FromBody] RefundPaymentDto refundDto)
        {
            try
            {
                var result = await _paymentService.RefundPaymentAsync(refundDto);

                if (!result.Success)
                {
                    return BadRequest(result);
                }

                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al procesar el reembolso");
                return StatusCode(500, new { message = "Error interno del servidor" });
            }
        }

        /// <summary>
        /// Obtener un pago por ID
        /// </summary>
        [HttpGet("{id}")]
        public async Task<ActionResult<PaymentDto>> GetPaymentById(int id)
        {
            try
            {
                var payment = await _paymentService.GetPaymentByIdAsync(id);

                if (payment == null)
                {
                    return NotFound(new { message = "Pago no encontrado" });
                }

                return Ok(payment);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener el pago");
                return StatusCode(500, new { message = "Error interno del servidor" });
            }
        }

        /// <summary>
        /// Obtener historial de pagos del usuario autenticado
        /// </summary>
        [HttpGet("my-payments")]
        public async Task<ActionResult<List<PaymentHistoryDto>>> GetMyPayments()
        {
            try
            {
                var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

                if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out int userId))
                {
                    return Unauthorized(new { message = "Usuario no autenticado" });
                }

                var payments = await _paymentService.GetPaymentsByUserIdAsync(userId);
                return Ok(payments);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener los pagos del usuario");
                return StatusCode(500, new { message = "Error interno del servidor" });
            }
        }

        /// <summary>
        /// Obtener pagos por usuario (solo administradores)
        /// </summary>
        [HttpGet("user/{userId}")]
        [Authorize(Roles = "Administrador")]
        public async Task<ActionResult<List<PaymentHistoryDto>>> GetPaymentsByUserId(int userId)
        {
            try
            {
                var payments = await _paymentService.GetPaymentsByUserIdAsync(userId);
                return Ok(payments);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener los pagos del usuario");
                return StatusCode(500, new { message = "Error interno del servidor" });
            }
        }

        /// <summary>
        /// Obtener pagos por cita
        /// </summary>
        [HttpGet("appointment/{appointmentId}")]
        public async Task<ActionResult<List<PaymentHistoryDto>>> GetPaymentsByAppointmentId(int appointmentId)
        {
            try
            {
                var payments = await _paymentService.GetPaymentsByAppointmentIdAsync(appointmentId);
                return Ok(payments);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener los pagos de la cita");
                return StatusCode(500, new { message = "Error interno del servidor" });
            }
        }

        /// <summary>
        /// Obtener pago por ID de transacción
        /// </summary>
        [HttpGet("transaction/{transactionId}")]
        public async Task<ActionResult<PaymentDto>> GetPaymentByTransactionId(string transactionId)
        {
            try
            {
                var payment = await _paymentService.GetPaymentByTransactionIdAsync(transactionId);

                if (payment == null)
                {
                    return NotFound(new { message = "Pago no encontrado" });
                }

                return Ok(payment);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener el pago");
                return StatusCode(500, new { message = "Error interno del servidor" });
            }
        }

        /// <summary>
        /// Verificar estado de un pago
        /// </summary>
        [HttpGet("{id}/verify")]
        public async Task<ActionResult<object>> VerifyPaymentStatus(int id)
        {
            try
            {
                var isCompleted = await _paymentService.VerifyPaymentStatusAsync(id);

                return Ok(new
                {
                    paymentId = id,
                    isCompleted = isCompleted,
                    message = isCompleted ? "Pago completado" : "Pago no completado"
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al verificar el estado del pago");
                return StatusCode(500, new { message = "Error interno del servidor" });
            }
        }
    }
}
