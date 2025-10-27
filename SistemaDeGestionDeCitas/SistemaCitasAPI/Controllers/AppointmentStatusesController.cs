using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SistemaCitasAPI.Data;
using SistemaCitasAPI.DTOs;
using SistemaCitasAPI.Models;

namespace SistemaCitasAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class AppointmentStatusesController : ControllerBase
    {
        private readonly AppDbContext _context;

        public AppointmentStatusesController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<ActionResult<IEnumerable<AppointmentStatusDto>>> GetStatuses()
        {
            var statuses = await _context.AppointmentStatuses
                .Where(s => s.IsActive)
                .Select(s => new AppointmentStatusDto
                {
                    StatusId = s.StatusId,
                    StatusName = s.StatusName,
                    Description = s.Description,
                    ColorCode = s.ColorCode,
                    IsActive = s.IsActive
                })
                .ToListAsync();

            return Ok(statuses);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<AppointmentStatusDto>> GetStatus(int id)
        {
            var status = await _context.AppointmentStatuses.FindAsync(id);

            if (status == null)
            {
                return NotFound(new { message = "Estado no encontrado" });
            }

            var statusDto = new AppointmentStatusDto
            {
                StatusId = status.StatusId,
                StatusName = status.StatusName,
                Description = status.Description,
                ColorCode = status.ColorCode,
                IsActive = status.IsActive
            };

            return Ok(statusDto);
        }

        [HttpPost]
        [Authorize(Roles = "Administrador")]
        public async Task<ActionResult<AppointmentStatusDto>> CreateStatus(CreateAppointmentStatusDto dto)
        {
            var status = new AppointmentStatus
            {
                StatusName = dto.StatusName,
                Description = dto.Description,
                ColorCode = dto.ColorCode,
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            };

            _context.AppointmentStatuses.Add(status);
            await _context.SaveChangesAsync();

            var statusDto = new AppointmentStatusDto
            {
                StatusId = status.StatusId,
                StatusName = status.StatusName,
                Description = status.Description,
                ColorCode = status.ColorCode,
                IsActive = status.IsActive
            };

            return CreatedAtAction(nameof(GetStatus), new { id = status.StatusId }, statusDto);
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Administrador")]
        public async Task<IActionResult> UpdateStatus(int id, UpdateAppointmentStatusDto dto)
        {
            var status = await _context.AppointmentStatuses.FindAsync(id);

            if (status == null)
            {
                return NotFound(new { message = "Estado no encontrado" });
            }

            if (!string.IsNullOrEmpty(dto.StatusName))
                status.StatusName = dto.StatusName;

            if (dto.Description != null)
                status.Description = dto.Description;

            if (dto.ColorCode != null)
                status.ColorCode = dto.ColorCode;

            if (dto.IsActive.HasValue)
                status.IsActive = dto.IsActive.Value;

            await _context.SaveChangesAsync();

            return NoContent();
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Administrador")]
        public async Task<IActionResult> DeleteStatus(int id)
        {
            var status = await _context.AppointmentStatuses.FindAsync(id);

            if (status == null)
            {
                return NotFound(new { message = "Estado no encontrado" });
            }

            // Soft delete
            status.IsActive = false;
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}
