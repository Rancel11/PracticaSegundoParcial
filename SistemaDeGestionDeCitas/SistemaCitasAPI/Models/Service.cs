using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SistemaCitasAPI.Models
{
    public class Service
    {
        [Key]
        public int ServiceId { get; set; }

        [Required]
        [MaxLength(100)]
        public string ServiceName { get; set; } = string.Empty;

        public string? Description { get; set; }

        [Required]
        public int DurationMinutes { get; set; }

        [Required]
        [Column(TypeName = "decimal(10,2)")]
        public decimal Price { get; set; }

        [Required]
        public int CategoryId { get; set; }

        public bool IsActive { get; set; } = true;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public DateTime? UpdatedAt { get; set; }

        // Navigation Properties
        [ForeignKey("CategoryId")]
        public virtual ServiceCategory Category { get; set; } = null!;

        public virtual ICollection<Schedule> Schedules { get; set; } = new List<Schedule>();
        public virtual ICollection<Appointment> Appointments { get; set; } = new List<Appointment>();

        public virtual ICollection<ScheduleException> ScheduleExceptions { get; set; } = new List<ScheduleException>();
    }
}
