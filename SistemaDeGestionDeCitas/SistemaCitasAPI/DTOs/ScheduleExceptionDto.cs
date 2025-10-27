namespace SistemaCitasAPI.DTOs
{
    public class ScheduleExceptionDto
    {
        public int ExceptionId { get; set; }
        public int? ServiceId { get; set; }
        public string? ServiceName { get; set; }
        public DateTime ExceptionDate { get; set; }
        public TimeSpan? StartTime { get; set; }
        public TimeSpan? EndTime { get; set; }
        public string Reason { get; set; } = string.Empty;
        public string ExceptionType { get; set; } = string.Empty;
        public bool IsRecurring { get; set; }
        public DateTime CreatedAt { get; set; }
    }

    public class CreateScheduleExceptionDto
    {
        public int? ServiceId { get; set; }
        public DateTime ExceptionDate { get; set; }
        public TimeSpan? StartTime { get; set; }
        public TimeSpan? EndTime { get; set; }
        public string Reason { get; set; } = string.Empty;
        public string ExceptionType { get; set; } = string.Empty;
        public bool IsRecurring { get; set; } = false;
    }

    public class UpdateScheduleExceptionDto
    {
        public DateTime? ExceptionDate { get; set; }
        public TimeSpan? StartTime { get; set; }
        public TimeSpan? EndTime { get; set; }
        public string? Reason { get; set; }
        public string? ExceptionType { get; set; }
        public bool? IsRecurring { get; set; }
    }
}
