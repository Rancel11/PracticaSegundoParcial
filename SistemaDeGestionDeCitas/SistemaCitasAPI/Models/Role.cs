using System.ComponentModel.DataAnnotations;

namespace SistemaCitasAPI.Models
{
    public class Role
    {
        [Key]
        public int RoleId { get; set; }

        [Required]
        [MaxLength(50)]
        public string RoleName { get; set; } = string.Empty;

        public string? Description { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // Navigation Properties
        public virtual ICollection<User> Users { get; set; } = new List<User>();
    }
}
