namespace SistemaCitasAPI.Models
{
    public class AppointmentStatus
    {
        public int StatusId { get; set; }
        public string StatusName { get; set; } = string.Empty;
        public string? Description { get; set; }
        public string? ColorCode { get; set; }
        public bool IsActive { get; set; } = true;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // Navigation properties
        public virtual ICollection<Appointment> Appointments { get; set; } = new List<Appointment>();
    }
}
