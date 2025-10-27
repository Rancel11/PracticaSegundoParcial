using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SistemaCitasAPI.Models
{
    public class Payment
    {
        [Key]
        public int PaymentId { get; set; }

        [Required]
        public int AppointmentId { get; set; }

        [Required]
        [Column(TypeName = "decimal(10,2)")]
        public decimal Amount { get; set; }

        [Required]
        [MaxLength(50)]
        public string PaymentMethod { get; set; } = string.Empty;

        [Required]
        [MaxLength(20)]
        public string Status { get; set; } = "Pendiente";

        public string? TransactionId { get; set; }

        public DateTime PaymentDate { get; set; } = DateTime.UtcNow;

        // Navigation Properties
        [ForeignKey("AppointmentId")]
        public virtual Appointment Appointment { get; set; } = null!;
    }
}
