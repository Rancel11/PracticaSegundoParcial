using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SistemaCitasAPI.DTOs;
using SistemaCitasAPI.Services;

namespace SistemaCitasAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ServicesController : ControllerBase
    {
        private readonly IServiceService _serviceService;

        public ServicesController(IServiceService serviceService)
        {
            _serviceService = serviceService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllServices()
        {
            var services = await _serviceService.GetAllServicesAsync();
            return Ok(services);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetServiceById(int id)
        {
            var service = await _serviceService.GetServiceByIdAsync(id);
            if (service == null)
                return NotFound(new { message = "Servicio no encontrado" });

            return Ok(service);
        }

        [HttpPost]
        [Authorize(Roles = "Administrador")]
        public async Task<IActionResult> CreateService([FromBody] CreateServiceDTO dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var service = await _serviceService.CreateServiceAsync(dto);
            if (service == null)
                return BadRequest(new { message = "Error al crear el servicio" });

            return CreatedAtAction(nameof(GetServiceById), new { id = service.ServiceId }, service);
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Administrador")]
        public async Task<IActionResult> UpdateService(int id, [FromBody] CreateServiceDTO dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var service = await _serviceService.UpdateServiceAsync(id, dto);
            if (service == null)
                return NotFound(new { message = "Servicio no encontrado" });

            return Ok(service);
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Administrador")]
        public async Task<IActionResult> DeleteService(int id)
        {
            var result = await _serviceService.DeleteServiceAsync(id);
            if (!result)
                return NotFound(new { message = "Servicio no encontrado" });

            return Ok(new { message = "Servicio eliminado exitosamente" });
        }
    }
}
