using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SistemaCitasAPI.DTOs;
using SistemaCitasAPI.Services;
using System.Security.Claims;

namespace SistemaCitasAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class UsersController : ControllerBase
    {
        private readonly IUserService _userService;

        public UsersController(IUserService userService)
        {
            _userService = userService;
        }

        [HttpGet]
        [Authorize(Roles = "Administrador")]
        public async Task<IActionResult> GetAllUsers()
        {
            var users = await _userService.GetAllUsersAsync();
            return Ok(users);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetUserById(int id)
        {
            var currentUserId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
            var userRole = User.FindFirst(ClaimTypes.Role)?.Value;

            // Solo admin o el mismo usuario pueden ver el perfil
            if (userRole != "Administrador" && currentUserId != id)
                return Forbid();

            var user = await _userService.GetUserByIdAsync(id);
            if (user == null)
                return NotFound(new { message = "Usuario no encontrado" });

            return Ok(user);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateUser(int id, [FromBody] UpdateUserDTO dto)
        {
            var currentUserId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
            var userRole = User.FindFirst(ClaimTypes.Role)?.Value;

            // Solo admin o el mismo usuario pueden actualizar
            if (userRole != "Administrador" && currentUserId != id)
                return Forbid();

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var user = await _userService.UpdateUserAsync(id, dto);
            if (user == null)
                return NotFound(new { message = "Usuario no encontrado" });

            return Ok(user);
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Administrador")]
        public async Task<IActionResult> DeleteUser(int id)
        {
            var result = await _userService.DeleteUserAsync(id);
            if (!result)
                return NotFound(new { message = "Usuario no encontrado" });

            return Ok(new { message = "Usuario eliminado exitosamente" });
        }
    }
}
