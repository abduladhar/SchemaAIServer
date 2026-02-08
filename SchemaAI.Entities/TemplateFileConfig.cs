namespace SchemaAI.Entities
{
    public class TemplateFileConfig : BaseEntity
    {
        public Guid TemplateFileConfigGuid { get; set; }
        public Guid TemplateGuid { get; set; }

        public string AllowedFileTypesCsv { get; set; } = string.Empty;
        public long MaxFileSizeInBytes { get; set; }
    }
}
