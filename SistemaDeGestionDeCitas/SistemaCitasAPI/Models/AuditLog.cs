using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SistemaCitasAPI.Models
{
    public class AuditLog
    {
        [Key]
        public int AuditId { get; set; }

        [Required]
        [MaxLength(50)]
        public string TableName { get; set; } = string.Empty;

        [Required]
        public int RecordId { get; set; }

        [Required]
        [MaxLength(50)]
        public string Action { get; set; } = string.Empty;

        public string? OldValues { get; set; }

        public string? NewValues { get; set; }

        [Required]
        public int UserId { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // Navigation Properties
        [ForeignKey("UserId")]
        public virtual User User { get; set; } = null!;
    }
}
