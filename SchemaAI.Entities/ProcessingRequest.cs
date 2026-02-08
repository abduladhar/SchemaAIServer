namespace SchemaAI.Entities
{
    public class ProcessingRequest : BaseEntity
    {
        public Guid ProcessingRequestGuid { get; set; }

        public Guid UploadedFileGuid { get; set; }
        public string ReferenceNumber { get; set; } = string.Empty;
        public Guid TemplateGuid { get; set; }
        public string Status { get; set; } = string.Empty;
        public string TemplateName { get; set; } = string.Empty;
    }
}
