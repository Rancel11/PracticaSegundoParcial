using Microsoft.EntityFrameworkCore;
using SistemaCitasAPI.Data;

namespace SistemaCitasAPI.Services
{
    public class ReportService : IReportService
    {
        private readonly AppDbContext _context;

        public ReportService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<object> GetDashboardStatsAsync()
        {
            var totalUsers = await _context.Users.CountAsync(u => u.IsActive);
            var totalAppointments = await _context.Appointments.CountAsync();
            var pendingAppointments = await _context.Appointments.CountAsync(a => a.Status == "Pendiente");
            var totalServices = await _context.Services.CountAsync(s => s.IsActive);

            return new
            {
                TotalUsers = totalUsers,
                TotalAppointments = totalAppointments,
                PendingAppointments = pendingAppointments,
                TotalServices = totalServices
            };
        }

        public async Task<object> GetAppointmentsByStatusAsync()
        {
            var stats = await _context.Appointments
                .GroupBy(a => a.Status)
                .Select(g => new
                {
                    Status = g.Key,
                    Count = g.Count()
                })
                .ToListAsync();

            return stats;
        }

        public async Task<object> GetRevenueReportAsync(DateTime startDate, DateTime endDate)
        {
            var revenue = await _context.Payments
                .Where(p => p.PaymentDate >= startDate && p.PaymentDate <= endDate && p.Status == "Completado")
                .GroupBy(p => p.PaymentDate.Date)
                .Select(g => new
                {
                    Date = g.Key,
                    TotalRevenue = g.Sum(p => p.Amount),
                    TransactionCount = g.Count()
                })
                .OrderBy(r => r.Date)
                .ToListAsync();

            return revenue;
        }

        public async Task<object> GetPopularServicesAsync()
        {
            var popularServices = await _context.Appointments
                .Include(a => a.Service)
                .GroupBy(a => new { a.ServiceId, a.Service.ServiceName })
                .Select(g => new
                {
                    ServiceId = g.Key.ServiceId,
                    ServiceName = g.Key.ServiceName,
                    AppointmentCount = g.Count()
                })
                .OrderByDescending(s => s.AppointmentCount)
                .Take(10)
                .ToListAsync();

            return popularServices;
        }
    }
}
