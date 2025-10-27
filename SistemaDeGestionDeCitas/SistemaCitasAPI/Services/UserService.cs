using Microsoft.EntityFrameworkCore;
using SistemaCitasAPI.Data;
using SistemaCitasAPI.DTOs;

namespace SistemaCitasAPI.Services
{
    public class UserService : IUserService
    {
        private readonly AppDbContext _context;

        public UserService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<UserDTO>> GetAllUsersAsync()
        {
            return await _context.Users
                .Include(u => u.Role)
                .Where(u => u.IsActive)
                .Select(u => new UserDTO
                {
                    UserId = u.UserId,
                    Email = u.Email,
                    FirstName = u.FirstName,
                    LastName = u.LastName,
                    PhoneNumber = u.PhoneNumber,
                    Role = u.Role.RoleName,
                    IsActive = u.IsActive,
                    CreatedAt = u.CreatedAt
                })
                .ToListAsync();
        }

        public async Task<UserDTO?> GetUserByIdAsync(int userId)
        {
            var user = await _context.Users
                .Include(u => u.Role)
                .FirstOrDefaultAsync(u => u.UserId == userId && u.IsActive);

            if (user == null)
                return null;

            return new UserDTO
            {
                UserId = user.UserId,
                Email = user.Email,
                FirstName = user.FirstName,
                LastName = user.LastName,
                PhoneNumber = user.PhoneNumber,
                Role = user.Role.RoleName,
                IsActive = user.IsActive,
                CreatedAt = user.CreatedAt
            };
        }

        public async Task<UserDTO?> UpdateUserAsync(int userId, UpdateUserDTO dto)
        {
            var user = await _context.Users
                .Include(u => u.Role)
                .FirstOrDefaultAsync(u => u.UserId == userId);

            if (user == null)
                return null;

            if (!string.IsNullOrEmpty(dto.FirstName))
                user.FirstName = dto.FirstName;

            if (!string.IsNullOrEmpty(dto.LastName))
                user.LastName = dto.LastName;

            if (dto.PhoneNumber != null)
                user.PhoneNumber = dto.PhoneNumber;

            user.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            return new UserDTO
            {
                UserId = user.UserId,
                Email = user.Email,
                FirstName = user.FirstName,
                LastName = user.LastName,
                PhoneNumber = user.PhoneNumber,
                Role = user.Role.RoleName,
                IsActive = user.IsActive,
                CreatedAt = user.CreatedAt
            };
        }

        public async Task<bool> DeleteUserAsync(int userId)
        {
            var user = await _context.Users.FindAsync(userId);
            if (user == null)
                return false;

            user.IsActive = false;
            user.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();
            return true;
        }
    }
}
