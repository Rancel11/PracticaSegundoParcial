using Microsoft.EntityFrameworkCore;
using SistemaCitasAPI.Data;
using SistemaCitasAPI.Models;

namespace SistemaCitasAPI.Services
{
    public class NotificationService : INotificationService
    {
        private readonly AppDbContext _context;
        private readonly ILogger<NotificationService> _logger;

        public NotificationService(AppDbContext context, ILogger<NotificationService> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task SendAppointmentReminderAsync(int appointmentId)
        {
            var appointment = await _context.Appointments
                .Include(a => a.User)
                .Include(a => a.Service)
                .FirstOrDefaultAsync(a => a.AppointmentId == appointmentId);

            if (appointment == null)
                return;

            var message = $"Recordatorio: Tienes una cita de {appointment.Service.ServiceName} " +
                         $"el {appointment.AppointmentDate:dd/MM/yyyy} a las {appointment.AppointmentTime}.";

            var notification = new Notification
            {
                UserId = appointment.UserId,
                AppointmentId = appointmentId,
                NotificationType = "Recordatorio",
                Message = message,
                Status = "Pendiente",
                CreatedAt = DateTime.UtcNow
            };

            _context.Notifications.Add(notification);
            await _context.SaveChangesAsync();

            // Aquí se implementaría el envío real del email
            _logger.LogInformation($"Recordatorio enviado para cita {appointmentId}");
        }

        public async Task SendAppointmentConfirmationAsync(int appointmentId)
        {
            var appointment = await _context.Appointments
                .Include(a => a.User)
                .Include(a => a.Service)
                .FirstOrDefaultAsync(a => a.AppointmentId == appointmentId);

            if (appointment == null)
                return;

            var message = $"Tu cita de {appointment.Service.ServiceName} ha sido confirmada " +
                         $"para el {appointment.AppointmentDate:dd/MM/yyyy} a las {appointment.AppointmentTime}.";

            var notification = new Notification
            {
                UserId = appointment.UserId,
                AppointmentId = appointmentId,
                NotificationType = "Confirmación",
                Message = message,
                Status = "Enviado",
                SentAt = DateTime.UtcNow,
                CreatedAt = DateTime.UtcNow
            };

            _context.Notifications.Add(notification);
            await _context.SaveChangesAsync();

            _logger.LogInformation($"Confirmación enviada para cita {appointmentId}");
        }

        public async Task SendAppointmentCancellationAsync(int appointmentId)
        {
            var appointment = await _context.Appointments
                .Include(a => a.User)
                .Include(a => a.Service)
                .FirstOrDefaultAsync(a => a.AppointmentId == appointmentId);

            if (appointment == null)
                return;

            var message = $"Tu cita de {appointment.Service.ServiceName} " +
                         $"del {appointment.AppointmentDate:dd/MM/yyyy} a las {appointment.AppointmentTime} ha sido cancelada.";

            var notification = new Notification
            {
                UserId = appointment.UserId,
                AppointmentId = appointmentId,
                NotificationType = "Cancelación",
                Message = message,
                Status = "Enviado",
                SentAt = DateTime.UtcNow,
                CreatedAt = DateTime.UtcNow
            };

            _context.Notifications.Add(notification);
            await _context.SaveChangesAsync();

            _logger.LogInformation($"Notificación de cancelación enviada para cita {appointmentId}");
        }
    }
}
