using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SistemaCitasAPI.DTOs;
using SistemaCitasAPI.Services;
using System.Security.Claims;

namespace SistemaCitasAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequestDTO request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _authService.LoginAsync(request);
            if (result == null)
                return Unauthorized(new { message = "Credenciales inválidas" });

            // Crear claims para la cookie
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, result.UserId.ToString()),
                new Claim(ClaimTypes.Email, result.Email),
                new Claim(ClaimTypes.Name, $"{result.FirstName} {result.LastName}"),
                new Claim(ClaimTypes.Role, result.Role)
            };

            var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            var authProperties = new AuthenticationProperties
            {
                IsPersistent = true,
                ExpiresUtc = DateTimeOffset.UtcNow.AddHours(8)
            };

            await HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                new ClaimsPrincipal(claimsIdentity),
                authProperties);

            return Ok(result);
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterRequestDTO request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _authService.RegisterAsync(request);
            if (result == null)
                return BadRequest(new { message = "El email ya está registrado" });

            return Ok(result);
        }

        [HttpPost("logout")]
        [Authorize]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return Ok(new { message = "Sesión cerrada exitosamente" });
        }

        [HttpGet("me")]
        [Authorize]
        public IActionResult GetCurrentUser()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var email = User.FindFirst(ClaimTypes.Email)?.Value;
            var name = User.FindFirst(ClaimTypes.Name)?.Value;
            var role = User.FindFirst(ClaimTypes.Role)?.Value;

            return Ok(new
            {
                UserId = userId,
                Email = email,
                Name = name,
                Role = role
            });
        }
    }
}
