using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SistemaCitasAPI.Models
{
    public class Appointment
    {
        [Key]
        public int AppointmentId { get; set; }

        [Required]
        public int UserId { get; set; }

        [Required]
        public int ServiceId { get; set; }

        [Required]
        public int StatusId { get; set; }

        [Required]
        public DateTime AppointmentDate { get; set; }

        [Required]
        public TimeSpan AppointmentTime { get; set; }

        public string? Notes { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public DateTime? UpdatedAt { get; set; }

        // Navigation Properties
        [ForeignKey("UserId")]
        public virtual User User { get; set; } = null!;

        [ForeignKey("ServiceId")]
        public virtual Service Service { get; set; } = null!;

        [ForeignKey("StatusId")]
        public virtual AppointmentStatus Status { get; set; } = null!;

        public virtual ICollection<Payment> Payments { get; set; } = new List<Payment>();
        public virtual ICollection<Notification> Notifications { get; set; } = new List<Notification>();
    }
}
