using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SistemaCitasAPI.Services;

namespace SistemaCitasAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = "Administrador")]
    public class ReportsController : ControllerBase
    {
        private readonly IReportService _reportService;

        public ReportsController(IReportService reportService)
        {
            _reportService = reportService;
        }

        [HttpGet("dashboard")]
        public async Task<IActionResult> GetDashboardStats()
        {
            var stats = await _reportService.GetDashboardStatsAsync();
            return Ok(stats);
        }

        [HttpGet("appointments-by-status")]
        public async Task<IActionResult> GetAppointmentsByStatus()
        {
            var stats = await _reportService.GetAppointmentsByStatusAsync();
            return Ok(stats);
        }

        [HttpGet("revenue")]
        public async Task<IActionResult> GetRevenueReport([FromQuery] DateTime startDate, [FromQuery] DateTime endDate)
        {
            var report = await _reportService.GetRevenueReportAsync(startDate, endDate);
            return Ok(report);
        }

        [HttpGet("popular-services")]
        public async Task<IActionResult> GetPopularServices()
        {
            var services = await _reportService.GetPopularServicesAsync();
            return Ok(services);
        }
    }
}
