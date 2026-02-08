namespace SchemaAI.Entities
{
    public class SystemConfig : BaseEntity
    {
        public Guid SystemConfigGuid { get; set; }   // ✅ FIXED
        public bool UseAws { get; set; } = false;
        public string WebhookUrl { get; set; } = string.Empty;
        public string ApiKey { get; set; } = string.Empty;
    }
}
