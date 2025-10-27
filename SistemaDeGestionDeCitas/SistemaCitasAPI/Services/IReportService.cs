namespace SistemaCitasAPI.Services
{
    public interface IReportService
    {
        Task<object> GetDashboardStatsAsync();
        Task<object> GetAppointmentsByStatusAsync();
        Task<object> GetRevenueReportAsync(DateTime startDate, DateTime endDate);
        Task<object> GetPopularServicesAsync();
    }
}
