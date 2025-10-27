namespace SistemaCitasAPI.Models
{
    public class ScheduleException
    {
        public int ExceptionId { get; set; }
        public int? ServiceId { get; set; }
        public DateTime ExceptionDate { get; set; }
        public TimeSpan? StartTime { get; set; }
        public TimeSpan? EndTime { get; set; }
        public string Reason { get; set; } = string.Empty;
        public string ExceptionType { get; set; } = string.Empty; // Holiday, Vacation, Maintenance, etc.
        public bool IsRecurring { get; set; } = false;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public int? CreatedBy { get; set; }

        // Navigation properties
        public virtual Service? Service { get; set; }
        public virtual User? Creator { get; set; }
    }
}
