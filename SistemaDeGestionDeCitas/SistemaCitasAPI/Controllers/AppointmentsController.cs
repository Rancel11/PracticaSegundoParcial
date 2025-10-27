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
    public class AppointmentsController : ControllerBase
    {
        private readonly IAppointmentService _appointmentService;
        private readonly INotificationService _notificationService;

        public AppointmentsController(
            IAppointmentService appointmentService,
            INotificationService notificationService)
        {
            _appointmentService = appointmentService;
            _notificationService = notificationService;
        }

        [HttpGet]
        [Authorize(Roles = "Administrador")]
        public async Task<IActionResult> GetAllAppointments()
        {
            var appointments = await _appointmentService.GetAllAppointmentsAsync();
            return Ok(appointments);
        }

        [HttpGet("my-appointments")]
        public async Task<IActionResult> GetMyAppointments()
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
            var appointments = await _appointmentService.GetUserAppointmentsAsync(userId);
            return Ok(appointments);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetAppointmentById(int id)
        {
            var appointment = await _appointmentService.GetAppointmentByIdAsync(id);
            if (appointment == null)
                return NotFound(new { message = "Cita no encontrada" });

            var currentUserId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
            var userRole = User.FindFirst(ClaimTypes.Role)?.Value;

            // Solo admin o el dueño de la cita pueden verla
            if (userRole != "Administrador" && appointment.UserId != currentUserId)
                return Forbid();

            return Ok(appointment);
        }

        [HttpPost]
        public async Task<IActionResult> CreateAppointment([FromBody] CreateAppointmentDTO dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
            var appointment = await _appointmentService.CreateAppointmentAsync(userId, dto);
            
            if (appointment == null)
                return BadRequest(new { message = "No se pudo crear la cita. Verifica la disponibilidad." });

            // Enviar confirmación
            await _notificationService.SendAppointmentConfirmationAsync(appointment.AppointmentId);

            return CreatedAtAction(nameof(GetAppointmentById), new { id = appointment.AppointmentId }, appointment);
        }

        [HttpPut("{id}/status")]
        [Authorize(Roles = "Administrador")]
        public async Task<IActionResult> UpdateAppointmentStatus(int id, [FromBody] UpdateAppointmentStatusDTO dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var appointment = await _appointmentService.UpdateAppointmentStatusAsync(id, dto.Status);
            if (appointment == null)
                return NotFound(new { message = "Cita no encontrada" });

            return Ok(appointment);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> CancelAppointment(int id)
        {
            var appointment = await _appointmentService.GetAppointmentByIdAsync(id);
            if (appointment == null)
                return NotFound(new { message = "Cita no encontrada" });

            var currentUserId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
            var userRole = User.FindFirst(ClaimTypes.Role)?.Value;

            // Solo admin o el dueño pueden cancelar
            if (userRole != "Administrador" && appointment.UserId != currentUserId)
                return Forbid();

            var result = await _appointmentService.CancelAppointmentAsync(id);
            if (!result)
                return BadRequest(new { message = "No se pudo cancelar la cita" });

            // Enviar notificación de cancelación
            await _notificationService.SendAppointmentCancellationAsync(id);

            return Ok(new { message = "Cita cancelada exitosamente" });
        }
    }
}
