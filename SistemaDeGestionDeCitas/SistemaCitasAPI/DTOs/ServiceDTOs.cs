using System.ComponentModel.DataAnnotations;

namespace SistemaCitasAPI.DTOs
{
    public class ServiceDTO
    {
        public int ServiceId { get; set; }
        public string ServiceName { get; set; } = string.Empty;
        public string? Description { get; set; }
        public int DurationMinutes { get; set; }
        public decimal Price { get; set; }
        public string CategoryName { get; set; } = string.Empty;
        public bool IsActive { get; set; }
    }

    public class CreateServiceDTO
    {
        [Required(ErrorMessage = "El nombre del servicio es requerido")]
        public string ServiceName { get; set; } = string.Empty;

        public string? Description { get; set; }

        [Required(ErrorMessage = "La duración es requerida")]
        [Range(15, 480, ErrorMessage = "La duración debe estar entre 15 y 480 minutos")]
        public int DurationMinutes { get; set; }

        [Required(ErrorMessage = "El precio es requerido")]
        [Range(0.01, 999999.99, ErrorMessage = "El precio debe ser mayor a 0")]
        public decimal Price { get; set; }

        [Required(ErrorMessage = "La categoría es requerida")]
        public int CategoryId { get; set; }
    }
}
