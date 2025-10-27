using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SistemaCitasAPI.Data;
using SistemaCitasAPI.DTOs;
using SistemaCitasAPI.Models;
using System.Security.Claims;

namespace SistemaCitasAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = "Administrador")]
    public class ScheduleExceptionsController : ControllerBase
    {
        private readonly AppDbContext _context;

        public ScheduleExceptionsController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<ScheduleExceptionDto>>> GetExceptions(
            [FromQuery] int? serviceId = null,
            [FromQuery] DateTime? startDate = null,
            [FromQuery] DateTime? endDate = null)
        {
            var query = _context.ScheduleExceptions
                .Include(e => e.Service)
                .AsQueryable();

            if (serviceId.HasValue)
                query = query.Where(e => e.ServiceId == serviceId.Value);

            if (startDate.HasValue)
                query = query.Where(e => e.ExceptionDate >= startDate.Value);

            if (endDate.HasValue)
                query = query.Where(e => e.ExceptionDate <= endDate.Value);

            var exceptions = await query
                .Select(e => new ScheduleExceptionDto
                {
                    ExceptionId = e.ExceptionId,
                    ServiceId = e.ServiceId,
                    ServiceName = e.Service != null ? e.Service.ServiceName : null,
                    ExceptionDate = e.ExceptionDate,
                    StartTime = e.StartTime,
                    EndTime = e.EndTime,
                    Reason = e.Reason,
                    ExceptionType = e.ExceptionType,
                    IsRecurring = e.IsRecurring,
                    CreatedAt = e.CreatedAt
                })
                .OrderBy(e => e.ExceptionDate)
                .ToListAsync();

            return Ok(exceptions);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ScheduleExceptionDto>> GetException(int id)
        {
            var exception = await _context.ScheduleExceptions
                .Include(e => e.Service)
                .FirstOrDefaultAsync(e => e.ExceptionId == id);

            if (exception == null)
            {
                return NotFound(new { message = "Excepción no encontrada" });
            }

            var exceptionDto = new ScheduleExceptionDto
            {
                ExceptionId = exception.ExceptionId,
                ServiceId = exception.ServiceId,
                ServiceName = exception.Service?.ServiceName,
                ExceptionDate = exception.ExceptionDate,
                StartTime = exception.StartTime,
                EndTime = exception.EndTime,
                Reason = exception.Reason,
                ExceptionType = exception.ExceptionType,
                IsRecurring = exception.IsRecurring,
                CreatedAt = exception.CreatedAt
            };

            return Ok(exceptionDto);
        }

        [HttpPost]
        public async Task<ActionResult<ScheduleExceptionDto>> CreateException(CreateScheduleExceptionDto dto)
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out int userId))
            {
                return Unauthorized(new { message = "Usuario no autenticado" });
            }

            var exception = new ScheduleException
            {
                ServiceId = dto.ServiceId,
                ExceptionDate = dto.ExceptionDate,
                StartTime = dto.StartTime,
                EndTime = dto.EndTime,
                Reason = dto.Reason,
                ExceptionType = dto.ExceptionType,
                IsRecurring = dto.IsRecurring,
                CreatedAt = DateTime.UtcNow,
                CreatedBy = userId
            };

            _context.ScheduleExceptions.Add(exception);
            await _context.SaveChangesAsync();

            var exceptionDto = new ScheduleExceptionDto
            {
                ExceptionId = exception.ExceptionId,
                ServiceId = exception.ServiceId,
                ExceptionDate = exception.ExceptionDate,
                StartTime = exception.StartTime,
                EndTime = exception.EndTime,
                Reason = exception.Reason,
                ExceptionType = exception.ExceptionType,
                IsRecurring = exception.IsRecurring,
                CreatedAt = exception.CreatedAt
            };

            return CreatedAtAction(nameof(GetException), new { id = exception.ExceptionId }, exceptionDto);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateException(int id, UpdateScheduleExceptionDto dto)
        {
            var exception = await _context.ScheduleExceptions.FindAsync(id);

            if (exception == null)
            {
                return NotFound(new { message = "Excepción no encontrada" });
            }

            if (dto.ExceptionDate.HasValue)
                exception.ExceptionDate = dto.ExceptionDate.Value;

            if (dto.StartTime.HasValue)
                exception.StartTime = dto.StartTime;

            if (dto.EndTime.HasValue)
                exception.EndTime = dto.EndTime;

            if (!string.IsNullOrEmpty(dto.Reason))
                exception.Reason = dto.Reason;

            if (!string.IsNullOrEmpty(dto.ExceptionType))
                exception.ExceptionType = dto.ExceptionType;

            if (dto.IsRecurring.HasValue)
                exception.IsRecurring = dto.IsRecurring.Value;

            await _context.SaveChangesAsync();

            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteException(int id)
        {
            var exception = await _context.ScheduleExceptions.FindAsync(id);

            if (exception == null)
            {
                return NotFound(new { message = "Excepción no encontrada" });
            }

            _context.ScheduleExceptions.Remove(exception);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}
