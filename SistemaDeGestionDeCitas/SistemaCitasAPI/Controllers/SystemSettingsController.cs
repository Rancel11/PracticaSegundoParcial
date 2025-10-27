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
    public class SystemSettingsController : ControllerBase
    {
        private readonly AppDbContext _context;

        public SystemSettingsController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<SystemSettingDto>>> GetSettings([FromQuery] string? category = null)
        {
            var query = _context.SystemSettings.AsQueryable();

            if (!string.IsNullOrEmpty(category))
                query = query.Where(s => s.Category == category);

            var settings = await query
                .Select(s => new SystemSettingDto
                {
                    SettingId = s.SettingId,
                    SettingKey = s.SettingKey,
                    SettingValue = s.SettingValue,
                    Description = s.Description,
                    DataType = s.DataType,
                    Category = s.Category,
                    IsEditable = s.IsEditable,
                    UpdatedAt = s.UpdatedAt
                })
                .OrderBy(s => s.Category)
                .ThenBy(s => s.SettingKey)
                .ToListAsync();

            return Ok(settings);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<SystemSettingDto>> GetSetting(int id)
        {
            var setting = await _context.SystemSettings.FindAsync(id);

            if (setting == null)
            {
                return NotFound(new { message = "Configuración no encontrada" });
            }

            var settingDto = new SystemSettingDto
            {
                SettingId = setting.SettingId,
                SettingKey = setting.SettingKey,
                SettingValue = setting.SettingValue,
                Description = setting.Description,
                DataType = setting.DataType,
                Category = setting.Category,
                IsEditable = setting.IsEditable,
                UpdatedAt = setting.UpdatedAt
            };

            return Ok(settingDto);
        }

        [HttpGet("by-key/{key}")]
        [AllowAnonymous]
        public async Task<ActionResult<SystemSettingDto>> GetSettingByKey(string key)
        {
            var setting = await _context.SystemSettings
                .FirstOrDefaultAsync(s => s.SettingKey == key);

            if (setting == null)
            {
                return NotFound(new { message = "Configuración no encontrada" });
            }

            var settingDto = new SystemSettingDto
            {
                SettingId = setting.SettingId,
                SettingKey = setting.SettingKey,
                SettingValue = setting.SettingValue,
                Description = setting.Description,
                DataType = setting.DataType,
                Category = setting.Category,
                IsEditable = setting.IsEditable,
                UpdatedAt = setting.UpdatedAt
            };

            return Ok(settingDto);
        }

        [HttpPost]
        public async Task<ActionResult<SystemSettingDto>> CreateSetting(CreateSystemSettingDto dto)
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out int userId))
            {
                return Unauthorized(new { message = "Usuario no autenticado" });
            }

            var existingSetting = await _context.SystemSettings
                .FirstOrDefaultAsync(s => s.SettingKey == dto.SettingKey);

            if (existingSetting != null)
            {
                return BadRequest(new { message = "Ya existe una configuración con esta clave" });
            }

            var setting = new SystemSetting
            {
                SettingKey = dto.SettingKey,
                SettingValue = dto.SettingValue,
                Description = dto.Description,
                DataType = dto.DataType,
                Category = dto.Category,
                IsEditable = true,
                UpdatedAt = DateTime.UtcNow,
                UpdatedBy = userId
            };

            _context.SystemSettings.Add(setting);
            await _context.SaveChangesAsync();

            var settingDto = new SystemSettingDto
            {
                SettingId = setting.SettingId,
                SettingKey = setting.SettingKey,
                SettingValue = setting.SettingValue,
                Description = setting.Description,
                DataType = setting.DataType,
                Category = setting.Category,
                IsEditable = setting.IsEditable,
                UpdatedAt = setting.UpdatedAt
            };

            return CreatedAtAction(nameof(GetSetting), new { id = setting.SettingId }, settingDto);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateSetting(int id, UpdateSystemSettingDto dto)
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out int userId))
            {
                return Unauthorized(new { message = "Usuario no autenticado" });
            }

            var setting = await _context.SystemSettings.FindAsync(id);

            if (setting == null)
            {
                return NotFound(new { message = "Configuración no encontrada" });
            }

            if (!setting.IsEditable)
            {
                return BadRequest(new { message = "Esta configuración no es editable" });
            }

            setting.SettingValue = dto.SettingValue;
            setting.UpdatedAt = DateTime.UtcNow;
            setting.UpdatedBy = userId;

            await _context.SaveChangesAsync();

            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteSetting(int id)
        {
            var setting = await _context.SystemSettings.FindAsync(id);

            if (setting == null)
            {
                return NotFound(new { message = "Configuración no encontrada" });
            }

            if (!setting.IsEditable)
            {
                return BadRequest(new { message = "Esta configuración no puede ser eliminada" });
            }

            _context.SystemSettings.Remove(setting);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}
