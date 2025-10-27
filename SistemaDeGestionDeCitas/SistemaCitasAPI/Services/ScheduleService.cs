using Microsoft.EntityFrameworkCore;
using SistemaCitasAPI.Data;
using SistemaCitasAPI.DTOs;
using SistemaCitasAPI.Models;

namespace SistemaCitasAPI.Services
{
    public class ScheduleService : IScheduleService
    {
        private readonly AppDbContext _context;

        public ScheduleService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<ScheduleDTO>> GetServiceSchedulesAsync(int serviceId)
        {
            return await _context.Schedules
                .Include(s => s.Service)
                .Where(s => s.ServiceId == serviceId && s.IsActive)
                .Select(s => new ScheduleDTO
                {
                    ScheduleId = s.ScheduleId,
                    ServiceId = s.ServiceId,
                    ServiceName = s.Service.ServiceName,
                    DayOfWeek = s.DayOfWeek,
                    StartTime = s.StartTime,
                    EndTime = s.EndTime,
                    IsActive = s.IsActive
                })
                .ToListAsync();
        }

        public async Task<IEnumerable<AvailableSlotDTO>> GetAvailableSlotsAsync(int serviceId, DateTime date)
        {
            var dayOfWeek = date.DayOfWeek.ToString();
            var service = await _context.Services.FindAsync(serviceId);
            
            if (service == null)
                return new List<AvailableSlotDTO>();

            var schedules = await _context.Schedules
                .Where(s => s.ServiceId == serviceId && s.DayOfWeek == dayOfWeek && s.IsActive)
                .ToListAsync();

            var slots = new List<AvailableSlotDTO>();

            foreach (var schedule in schedules)
            {
                var currentTime = schedule.StartTime;
                while (currentTime < schedule.EndTime)
                {
                    var isBooked = await _context.Appointments
                        .AnyAsync(a => a.ServiceId == serviceId
                            && a.AppointmentDate.Date == date.Date
                            && a.AppointmentTime == currentTime
                            && a.Status != "Cancelada");

                    slots.Add(new AvailableSlotDTO
                    {
                        Date = date,
                        Time = currentTime,
                        IsAvailable = !isBooked
                    });

                    currentTime = currentTime.Add(TimeSpan.FromMinutes(service.DurationMinutes));
                }
            }

            return slots.OrderBy(s => s.Time);
        }

        public async Task<ScheduleDTO?> CreateScheduleAsync(CreateScheduleDTO dto)
        {
            var service = await _context.Services.FindAsync(dto.ServiceId);
            if (service == null)
                return null;

            var schedule = new Schedule
            {
                ServiceId = dto.ServiceId,
                DayOfWeek = dto.DayOfWeek,
                StartTime = dto.StartTime,
                EndTime = dto.EndTime,
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            };

            _context.Schedules.Add(schedule);
            await _context.SaveChangesAsync();

            return new ScheduleDTO
            {
                ScheduleId = schedule.ScheduleId,
                ServiceId = schedule.ServiceId,
                ServiceName = service.ServiceName,
                DayOfWeek = schedule.DayOfWeek,
                StartTime = schedule.StartTime,
                EndTime = schedule.EndTime,
                IsActive = schedule.IsActive
            };
        }

        public async Task<bool> DeleteScheduleAsync(int scheduleId)
        {
            var schedule = await _context.Schedules.FindAsync(scheduleId);
            if (schedule == null)
                return false;

            schedule.IsActive = false;
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
