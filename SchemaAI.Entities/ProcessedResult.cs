namespace SchemaAI.Entities
{
    public class ProcessedResult : BaseEntity
    {
        public Guid ProcessedResultGuid { get; set; }
        public Guid ProcessingRequestGuid { get; set; }

        public string OutputJson { get; set; } = string.Empty;
        public string TemplateName { get; set; } = string.Empty;
        public string ReferenceNumber { get; set; } = string.Empty;
    }

    public class WebhookResult
    {
        public string ReferenceNumber { get; set; }
        public string OutputJson { get; set; } = string.Empty;
    }
}
