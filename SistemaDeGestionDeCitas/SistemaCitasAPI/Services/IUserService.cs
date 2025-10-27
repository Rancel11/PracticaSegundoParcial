using SistemaCitasAPI.DTOs;

namespace SistemaCitasAPI.Services
{
    public interface IUserService
    {
        Task<IEnumerable<UserDTO>> GetAllUsersAsync();
        Task<UserDTO?> GetUserByIdAsync(int userId);
        Task<UserDTO?> UpdateUserAsync(int userId, UpdateUserDTO dto);
        Task<bool> DeleteUserAsync(int userId);
    }
}
