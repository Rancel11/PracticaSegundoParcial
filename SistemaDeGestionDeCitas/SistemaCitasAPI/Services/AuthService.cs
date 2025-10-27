using Microsoft.EntityFrameworkCore;
using SistemaCitasAPI.Data;
using SistemaCitasAPI.DTOs;
using SistemaCitasAPI.Models;
using System.Security.Cryptography;
using System.Text;

namespace SistemaCitasAPI.Services
{
    public class AuthService : IAuthService
    {
        private readonly AppDbContext _context;

        public AuthService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<AuthResponseDTO?> LoginAsync(LoginRequestDTO request)
        {
            var user = await _context.Users
                .Include(u => u.Role)
                .FirstOrDefaultAsync(u => u.Email == request.Email && u.IsActive);

            if (user == null)
                return null;

            if (!VerifyPassword(request.Password, user.PasswordHash))
                return null;

            return new AuthResponseDTO
            {
                UserId = user.UserId,
                Email = user.Email,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Role = user.Role.RoleName,
                Message = "Login exitoso"
            };
        }

        public async Task<AuthResponseDTO?> RegisterAsync(RegisterRequestDTO request)
        {
            // Verificar si el email ya existe
            if (await _context.Users.AnyAsync(u => u.Email == request.Email))
                return null;

            // Obtener el rol de Usuario (RoleId = 2)
            var userRole = await _context.Roles.FirstOrDefaultAsync(r => r.RoleName == "Usuario");
            if (userRole == null)
                return null;

            var user = new User
            {
                Email = request.Email,
                PasswordHash = HashPassword(request.Password),
                FirstName = request.FirstName,
                LastName = request.LastName,
                PhoneNumber = request.PhoneNumber,
                RoleId = userRole.RoleId,
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            return new AuthResponseDTO
            {
                UserId = user.UserId,
                Email = user.Email,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Role = userRole.RoleName,
                Message = "Registro exitoso"
            };
        }

        public async Task<bool> ValidateUserAsync(string email, string password)
        {
            var user = await _context.Users
                .FirstOrDefaultAsync(u => u.Email == email && u.IsActive);

            if (user == null)
                return false;

            return VerifyPassword(password, user.PasswordHash);
        }

        private string HashPassword(string password)
        {
            using var sha256 = SHA256.Create();
            var hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
            return Convert.ToBase64String(hashedBytes);
        }

        private bool VerifyPassword(string password, string hash)
        {
            var hashOfInput = HashPassword(password);
            return hashOfInput == hash;
        }
    }
}
