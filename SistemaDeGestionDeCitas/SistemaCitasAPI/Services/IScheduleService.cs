using SistemaCitasAPI.DTOs;

namespace SistemaCitasAPI.Services
{
    public interface IScheduleService
    {
        Task<IEnumerable<ScheduleDTO>> GetServiceSchedulesAsync(int serviceId);
        Task<IEnumerable<AvailableSlotDTO>> GetAvailableSlotsAsync(int serviceId, DateTime date);
        Task<ScheduleDTO?> CreateScheduleAsync(CreateScheduleDTO dto);
        Task<bool> DeleteScheduleAsync(int scheduleId);
    }
}
