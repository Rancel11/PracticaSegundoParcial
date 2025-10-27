namespace SistemaCitasAPI.Services
{
    public interface INotificationService
    {
        Task SendAppointmentReminderAsync(int appointmentId);
        Task SendAppointmentConfirmationAsync(int appointmentId);
        Task SendAppointmentCancellationAsync(int appointmentId);
    }
}
