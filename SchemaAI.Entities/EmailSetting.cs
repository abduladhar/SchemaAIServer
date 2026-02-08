namespace SchemaAI.Entities
{
    public class EmailSetting : BaseEntity
    {

        public Guid SmtpGuid { get; set; }

        public string SmtpHost { get; set; } = string.Empty;

        public int SmtpPort { get; set; }

        public string SenderName { get; set; } = string.Empty;

        public string SenderEmail { get; set; } = string.Empty;

        public string Password { get; set; } = string.Empty;
    }
}
