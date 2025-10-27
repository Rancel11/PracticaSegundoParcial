namespace SistemaCitasAPI.Models
{
    public class NotificationType
    {
        public int TypeId { get; set; }
        public string TypeName { get; set; } = string.Empty;
        public string? Description { get; set; }
        public string? Template { get; set; }
        public bool IsActive { get; set; } = true;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // Navigation properties
        public virtual ICollection<Notification> Notifications { get; set; } = new List<Notification>();
    }
}
