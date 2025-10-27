namespace SistemaCitasAPI.DTOs
{
    public class SystemSettingDto
    {
        public int SettingId { get; set; }
        public string SettingKey { get; set; } = string.Empty;
        public string SettingValue { get; set; } = string.Empty;
        public string? Description { get; set; }
        public string DataType { get; set; } = "string";
        public string Category { get; set; } = "General";
        public bool IsEditable { get; set; }
        public DateTime UpdatedAt { get; set; }
    }

    public class UpdateSystemSettingDto
    {
        public string SettingValue { get; set; } = string.Empty;
    }

    public class CreateSystemSettingDto
    {
        public string SettingKey { get; set; } = string.Empty;
        public string SettingValue { get; set; } = string.Empty;
        public string? Description { get; set; }
        public string DataType { get; set; } = "string";
        public string Category { get; set; } = "General";
    }
}
