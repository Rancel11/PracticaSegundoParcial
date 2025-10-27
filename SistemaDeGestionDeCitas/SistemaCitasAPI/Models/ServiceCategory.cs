using System.ComponentModel.DataAnnotations;

namespace SistemaCitasAPI.Models
{
    public class ServiceCategory
    {
        [Key]
        public int CategoryId { get; set; }

        [Required]
        [MaxLength(100)]
        public string CategoryName { get; set; } = string.Empty;

        public string? Description { get; set; }

        public bool IsActive { get; set; } = true;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // Navigation Properties
        public virtual ICollection<Service> Services { get; set; } = new List<Service>();
    }
}
