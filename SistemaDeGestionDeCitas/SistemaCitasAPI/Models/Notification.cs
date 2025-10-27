using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SistemaCitasAPI.Models
{
    public class Notification
    {
        [Key]
        public int NotificationId { get; set; }

        [Required]
        public int UserId { get; set; }

        [Required]
        public int AppointmentId { get; set; }

        [Required]
        public int TypeId { get; set; }

        [Required]
        public string Message { get; set; } = string.Empty;

        [Required]
        [MaxLength(20)]
        public string Status { get; set; } = "Pendiente";

        public DateTime? SentAt { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // Navigation Properties
        [ForeignKey("UserId")]
        public virtual User User { get; set; } = null!;

        [ForeignKey("AppointmentId")]
        public virtual Appointment Appointment { get; set; } = null!;

        [ForeignKey("TypeId")]
        public virtual NotificationType Type { get; set; } = null!;
    }
}
