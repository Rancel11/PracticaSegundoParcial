namespace SistemaCitasAPI.DTOs
{
    public class NotificationTypeDto
    {
        public int TypeId { get; set; }
        public string TypeName { get; set; } = string.Empty;
        public string? Description { get; set; }
        public string? Template { get; set; }
        public bool IsActive { get; set; }
    }

    public class CreateNotificationTypeDto
    {
        public string TypeName { get; set; } = string.Empty;
        public string? Description { get; set; }
        public string? Template { get; set; }
    }

    public class UpdateNotificationTypeDto
    {
        public string? TypeName { get; set; }
        public string? Description { get; set; }
        public string? Template { get; set; }
        public bool? IsActive { get; set; }
    }
}
