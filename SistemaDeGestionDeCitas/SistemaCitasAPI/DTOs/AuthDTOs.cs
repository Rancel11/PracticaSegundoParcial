using System.ComponentModel.DataAnnotations;

namespace SistemaCitasAPI.DTOs
{
    public class LoginRequestDTO
    {
        [Required(ErrorMessage = "El email es requerido")]
        [EmailAddress(ErrorMessage = "Email inválido")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "La contraseña es requerida")]
        public string Password { get; set; } = string.Empty;
    }

    public class RegisterRequestDTO
    {
        [Required(ErrorMessage = "El email es requerido")]
        [EmailAddress(ErrorMessage = "Email inválido")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "La contraseña es requerida")]
        [MinLength(6, ErrorMessage = "La contraseña debe tener al menos 6 caracteres")]
        public string Password { get; set; } = string.Empty;

        [Required(ErrorMessage = "El nombre es requerido")]
        public string FirstName { get; set; } = string.Empty;

        [Required(ErrorMessage = "El apellido es requerido")]
        public string LastName { get; set; } = string.Empty;

        [Phone(ErrorMessage = "Número de teléfono inválido")]
        public string? PhoneNumber { get; set; }
    }

    public class AuthResponseDTO
    {
        public int UserId { get; set; }
        public string Email { get; set; } = string.Empty;
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string Role { get; set; } = string.Empty;
        public string Message { get; set; } = string.Empty;
    }
}
