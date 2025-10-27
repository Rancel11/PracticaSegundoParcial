using SistemaCitasAPI.DTOs;

namespace SistemaCitasAPI.Services
{
    public interface IAppointmentService
    {
        Task<IEnumerable<AppointmentDTO>> GetAllAppointmentsAsync();
        Task<IEnumerable<AppointmentDTO>> GetUserAppointmentsAsync(int userId);
        Task<AppointmentDTO?> GetAppointmentByIdAsync(int appointmentId);
        Task<AppointmentDTO?> CreateAppointmentAsync(int userId, CreateAppointmentDTO dto);
        Task<AppointmentDTO?> UpdateAppointmentStatusAsync(int appointmentId, string status);
        Task<bool> CancelAppointmentAsync(int appointmentId);
    }
}
