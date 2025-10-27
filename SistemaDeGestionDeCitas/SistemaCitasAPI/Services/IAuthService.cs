using SistemaCitasAPI.DTOs;

namespace SistemaCitasAPI.Services
{
    public interface IAuthService
    {
        Task<AuthResponseDTO?> LoginAsync(LoginRequestDTO request);
        Task<AuthResponseDTO?> RegisterAsync(RegisterRequestDTO request);
        Task<bool> ValidateUserAsync(string email, string password);
    }
}
