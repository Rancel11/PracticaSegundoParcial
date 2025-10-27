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
    public class NotificationTypesController : ControllerBase
    {
        private readonly AppDbContext _context;

        public NotificationTypesController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<NotificationTypeDto>>> GetTypes()
        {
            var types = await _context.NotificationTypes
                .Where(t => t.IsActive)
                .Select(t => new NotificationTypeDto
                {
                    TypeId = t.TypeId,
                    TypeName = t.TypeName,
                    Description = t.Description,
                    Template = t.Template,
                    IsActive = t.IsActive
                })
                .ToListAsync();

            return Ok(types);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<NotificationTypeDto>> GetType(int id)
        {
            var type = await _context.NotificationTypes.FindAsync(id);

            if (type == null)
            {
                return NotFound(new { message = "Tipo de notificación no encontrado" });
            }

            var typeDto = new NotificationTypeDto
            {
                TypeId = type.TypeId,
                TypeName = type.TypeName,
                Description = type.Description,
                Template = type.Template,
                IsActive = type.IsActive
            };

            return Ok(typeDto);
        }

        [HttpPost]
        [Authorize(Roles = "Administrador")]
        public async Task<ActionResult<NotificationTypeDto>> CreateType(CreateNotificationTypeDto dto)
        {
            var type = new NotificationType
            {
                TypeName = dto.TypeName,
                Description = dto.Description,
                Template = dto.Template,
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            };

            _context.NotificationTypes.Add(type);
            await _context.SaveChangesAsync();

            var typeDto = new NotificationTypeDto
            {
                TypeId = type.TypeId,
                TypeName = type.TypeName,
                Description = type.Description,
                Template = type.Template,
                IsActive = type.IsActive
            };

            return CreatedAtAction(nameof(GetType), new { id = type.TypeId }, typeDto);
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Administrador")]
        public async Task<IActionResult> UpdateType(int id, UpdateNotificationTypeDto dto)
        {
            var type = await _context.NotificationTypes.FindAsync(id);

            if (type == null)
            {
                return NotFound(new { message = "Tipo de notificación no encontrado" });
            }

            if (!string.IsNullOrEmpty(dto.TypeName))
                type.TypeName = dto.TypeName;

            if (dto.Description != null)
                type.Description = dto.Description;

            if (dto.Template != null)
                type.Template = dto.Template;

            if (dto.IsActive.HasValue)
                type.IsActive = dto.IsActive.Value;

            await _context.SaveChangesAsync();

            return NoContent();
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Administrador")]
        public async Task<IActionResult> DeleteType(int id)
        {
            var type = await _context.NotificationTypes.FindAsync(id);

            if (type == null)
            {
                return NotFound(new { message = "Tipo de notificación no encontrado" });
            }

            type.IsActive = false;
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}
