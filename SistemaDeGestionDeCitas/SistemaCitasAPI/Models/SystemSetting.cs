namespace SistemaCitasAPI.Models
{
    public class SystemSetting
    {
        public int SettingId { get; set; }
        public string SettingKey { get; set; } = string.Empty;
        public string SettingValue { get; set; } = string.Empty;
        public string? Description { get; set; }
        public string DataType { get; set; } = "string"; // string, int, bool, json
        public string Category { get; set; } = "General";
        public bool IsEditable { get; set; } = true;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
        public int? UpdatedBy { get; set; }

        // Navigation properties
        public virtual User? Updater { get; set; }
    }
}
