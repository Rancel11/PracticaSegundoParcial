using System.ComponentModel.DataAnnotations;

namespace SistemaCitasAPI.DTOs
{
    public class AppointmentDTO
    {
        public int AppointmentId { get; set; }
        public int UserId { get; set; }
        public string UserName { get; set; } = string.Empty;
        public string UserEmail { get; set; } = string.Empty;
        public int ServiceId { get; set; }
        public string ServiceName { get; set; } = string.Empty;
        public DateTime AppointmentDate { get; set; }
        public TimeSpan AppointmentTime { get; set; }
        public string Status { get; set; } = string.Empty;
        public string? Notes { get; set; }
        public decimal ServicePrice { get; set; }
        public DateTime CreatedAt { get; set; }
    }

    public class CreateAppointmentDTO
    {
        [Required(ErrorMessage = "El servicio es requerido")]
        public int ServiceId { get; set; }

        [Required(ErrorMessage = "La fecha es requerida")]
        public DateTime AppointmentDate { get; set; }

        [Required(ErrorMessage = "La hora es requerida")]
        public TimeSpan AppointmentTime { get; set; }

        public string? Notes { get; set; }
    }

    public class UpdateAppointmentStatusDTO
    {
        [Required(ErrorMessage = "El estado es requerido")]
        public string Status { get; set; } = string.Empty;
    }
}
