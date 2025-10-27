using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SistemaCitasAPI.DTOs;
using SistemaCitasAPI.Services;

namespace SistemaCitasAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class SchedulesController : ControllerBase
    {
        private readonly IScheduleService _scheduleService;

        public SchedulesController(IScheduleService scheduleService)
        {
            _scheduleService = scheduleService;
        }

        [HttpGet("service/{serviceId}")]
        public async Task<IActionResult> GetServiceSchedules(int serviceId)
        {
            var schedules = await _scheduleService.GetServiceSchedulesAsync(serviceId);
            return Ok(schedules);
        }

        [HttpGet("available-slots")]
        public async Task<IActionResult> GetAvailableSlots([FromQuery] int serviceId, [FromQuery] DateTime date)
        {
            var slots = await _scheduleService.GetAvailableSlotsAsync(serviceId, date);
            return Ok(slots);
        }

        [HttpPost]
        [Authorize(Roles = "Administrador")]
        public async Task<IActionResult> CreateSchedule([FromBody] CreateScheduleDTO dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var schedule = await _scheduleService.CreateScheduleAsync(dto);
            if (schedule == null)
                return BadRequest(new { message = "Error al crear el horario" });

            return Ok(schedule);
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Administrador")]
        public async Task<IActionResult> DeleteSchedule(int id)
        {
            var result = await _scheduleService.DeleteScheduleAsync(id);
            if (!result)
                return NotFound(new { message = "Horario no encontrado" });

            return Ok(new { message = "Horario eliminado exitosamente" });
        }
    }
}
