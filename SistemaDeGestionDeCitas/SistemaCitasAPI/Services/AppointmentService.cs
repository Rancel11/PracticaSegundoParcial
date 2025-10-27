using Microsoft.EntityFrameworkCore;
using SistemaCitasAPI.Data;
using SistemaCitasAPI.DTOs;
using SistemaCitasAPI.Models;

namespace SistemaCitasAPI.Services
{
    public class AppointmentService : IAppointmentService
    {
        private readonly AppDbContext _context;

        public AppointmentService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<AppointmentDTO>> GetAllAppointmentsAsync()
        {
            return await _context.Appointments
                .Include(a => a.User)
                .Include(a => a.Service)
                .OrderByDescending(a => a.AppointmentDate)
                .Select(a => new AppointmentDTO
                {
                    AppointmentId = a.AppointmentId,
                    UserId = a.UserId,
                    UserName = $"{a.User.FirstName} {a.User.LastName}",
                    UserEmail = a.User.Email,
                    ServiceId = a.ServiceId,
                    ServiceName = a.Service.ServiceName,
                    AppointmentDate = a.AppointmentDate,
                    AppointmentTime = a.AppointmentTime,
                    Status = a.Status,
                    Notes = a.Notes,
                    ServicePrice = a.Service.Price,
                    CreatedAt = a.CreatedAt
                })
                .ToListAsync();
        }

        public async Task<IEnumerable<AppointmentDTO>> GetUserAppointmentsAsync(int userId)
        {
            return await _context.Appointments
                .Include(a => a.User)
                .Include(a => a.Service)
                .Where(a => a.UserId == userId)
                .OrderByDescending(a => a.AppointmentDate)
                .Select(a => new AppointmentDTO
                {
                    AppointmentId = a.AppointmentId,
                    UserId = a.UserId,
                    UserName = $"{a.User.FirstName} {a.User.LastName}",
                    UserEmail = a.User.Email,
                    ServiceId = a.ServiceId,
                    ServiceName = a.Service.ServiceName,
                    AppointmentDate = a.AppointmentDate,
                    AppointmentTime = a.AppointmentTime,
                    Status = a.Status,
                    Notes = a.Notes,
                    ServicePrice = a.Service.Price,
                    CreatedAt = a.CreatedAt
                })
                .ToListAsync();
        }

        public async Task<AppointmentDTO?> GetAppointmentByIdAsync(int appointmentId)
        {
            var appointment = await _context.Appointments
                .Include(a => a.User)
                .Include(a => a.Service)
                .FirstOrDefaultAsync(a => a.AppointmentId == appointmentId);

            if (appointment == null)
                return null;

            return new AppointmentDTO
            {
                AppointmentId = appointment.AppointmentId,
                UserId = appointment.UserId,
                UserName = $"{appointment.User.FirstName} {appointment.User.LastName}",
                UserEmail = appointment.User.Email,
                ServiceId = appointment.ServiceId,
                ServiceName = appointment.Service.ServiceName,
                AppointmentDate = appointment.AppointmentDate,
                AppointmentTime = appointment.AppointmentTime,
                Status = appointment.Status,
                Notes = appointment.Notes,
                ServicePrice = appointment.Service.Price,
                CreatedAt = appointment.CreatedAt
            };
        }

        public async Task<AppointmentDTO?> CreateAppointmentAsync(int userId, CreateAppointmentDTO dto)
        {
            // Verificar que el servicio existe
            var service = await _context.Services.FindAsync(dto.ServiceId);
            if (service == null || !service.IsActive)
                return null;

            // Verificar disponibilidad
            var isAvailable = await CheckAvailabilityAsync(dto.ServiceId, dto.AppointmentDate, dto.AppointmentTime);
            if (!isAvailable)
                return null;

            var appointment = new Appointment
            {
                UserId = userId,
                ServiceId = dto.ServiceId,
                AppointmentDate = dto.AppointmentDate,
                AppointmentTime = dto.AppointmentTime,
                Status = "Pendiente",
                Notes = dto.Notes,
                CreatedAt = DateTime.UtcNow
            };

            _context.Appointments.Add(appointment);
            await _context.SaveChangesAsync();

            // Recargar con includes
            var createdAppointment = await GetAppointmentByIdAsync(appointment.AppointmentId);
            return createdAppointment;
        }

        public async Task<AppointmentDTO?> UpdateAppointmentStatusAsync(int appointmentId, string status)
        {
            var appointment = await _context.Appointments
                .Include(a => a.User)
                .Include(a => a.Service)
                .FirstOrDefaultAsync(a => a.AppointmentId == appointmentId);

            if (appointment == null)
                return null;

            appointment.Status = status;
            appointment.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            return new AppointmentDTO
            {
                AppointmentId = appointment.AppointmentId,
                UserId = appointment.UserId,
                UserName = $"{appointment.User.FirstName} {appointment.User.LastName}",
                UserEmail = appointment.User.Email,
                ServiceId = appointment.ServiceId,
                ServiceName = appointment.Service.ServiceName,
                AppointmentDate = appointment.AppointmentDate,
                AppointmentTime = appointment.AppointmentTime,
                Status = appointment.Status,
                Notes = appointment.Notes,
                ServicePrice = appointment.Service.Price,
                CreatedAt = appointment.CreatedAt
            };
        }

        public async Task<bool> CancelAppointmentAsync(int appointmentId)
        {
            var appointment = await _context.Appointments.FindAsync(appointmentId);
            if (appointment == null)
                return false;

            appointment.Status = "Cancelada";
            appointment.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();
            return true;
        }

        private async Task<bool> CheckAvailabilityAsync(int serviceId, DateTime date, TimeSpan time)
        {
            var dayOfWeek = date.DayOfWeek.ToString();

            // Verificar que existe un horario para ese día
            var schedule = await _context.Schedules
                .FirstOrDefaultAsync(s => s.ServiceId == serviceId 
                    && s.DayOfWeek == dayOfWeek 
                    && s.IsActive
                    && time >= s.StartTime 
                    && time < s.EndTime);

            if (schedule == null)
                return false;

            // Verificar que no hay otra cita en ese horario
            var existingAppointment = await _context.Appointments
                .AnyAsync(a => a.ServiceId == serviceId
                    && a.AppointmentDate.Date == date.Date
                    && a.AppointmentTime == time
                    && a.Status != "Cancelada");

            return !existingAppointment;
        }
    }
}
