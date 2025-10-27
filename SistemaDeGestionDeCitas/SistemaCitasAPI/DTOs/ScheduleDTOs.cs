using System.ComponentModel.DataAnnotations;

namespace SistemaCitasAPI.DTOs
{
    public class ScheduleDTO
    {
        public int ScheduleId { get; set; }
        public int ServiceId { get; set; }
        public string ServiceName { get; set; } = string.Empty;
        public string DayOfWeek { get; set; } = string.Empty;
        public TimeSpan StartTime { get; set; }
        public TimeSpan EndTime { get; set; }
        public bool IsActive { get; set; }
    }

    public class CreateScheduleDTO
    {
        [Required(ErrorMessage = "El servicio es requerido")]
        public int ServiceId { get; set; }

        [Required(ErrorMessage = "El día de la semana es requerido")]
        public string DayOfWeek { get; set; } = string.Empty;

        [Required(ErrorMessage = "La hora de inicio es requerida")]
        public TimeSpan StartTime { get; set; }

        [Required(ErrorMessage = "La hora de fin es requerida")]
        public TimeSpan EndTime { get; set; }
    }

    public class AvailableSlotDTO
    {
        public DateTime Date { get; set; }
        public TimeSpan Time { get; set; }
        public bool IsAvailable { get; set; }
    }
}
