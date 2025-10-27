namespace SistemaCitasAPI.DTOs
{
    public class AppointmentStatusDto
    {
        public int StatusId { get; set; }
        public string StatusName { get; set; } = string.Empty;
        public string? Description { get; set; }
        public string? ColorCode { get; set; }
        public bool IsActive { get; set; }
    }

    public class CreateAppointmentStatusDto
    {
        public string StatusName { get; set; } = string.Empty;
        public string? Description { get; set; }
        public string? ColorCode { get; set; }
    }

    public class UpdateAppointmentStatusDto
    {
        public string? StatusName { get; set; }
        public string? Description { get; set; }
        public string? ColorCode { get; set; }
        public bool? IsActive { get; set; }
    }
}
