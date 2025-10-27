using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SistemaCitasAPI.Models
{
    public class Schedule
    {
        [Key]
        public int ScheduleId { get; set; }

        [Required]
        public int ServiceId { get; set; }

        [Required]
        [MaxLength(20)]
        public string DayOfWeek { get; set; } = string.Empty;

        [Required]
        public TimeSpan StartTime { get; set; }

        [Required]
        public TimeSpan EndTime { get; set; }

        public bool IsActive { get; set; } = true;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // Navigation Properties
        [ForeignKey("ServiceId")]
        public virtual Service Service { get; set; } = null!;
    }
}
