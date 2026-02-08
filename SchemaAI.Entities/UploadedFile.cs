namespace SchemaAI.Entities
{
    public class UploadedFile : BaseEntity
    {
        public Guid UploadedFileGuid { get; set; }
        public string ReferenceNumber { get; set; } = string.Empty;
        public Guid TemplateGuid { get; set; }
        public string OriginalFileName { get; set; } = string.Empty;
        public string StoredFilePath { get; set; } = string.Empty;
        public string SignedUrl { get; set; } = string.Empty;
        public string ApplicationName { get; set; } = string.Empty;
        public string ModuleName { get; set; } = string.Empty;
        public string TemplateName { get; set; } = string.Empty;
        public string ContantType { get; set; } = string.Empty;
    }

    public class DocumentExtractionResult
    {
        public string Json { get; set; } = string.Empty;
        public string RawResponse { get; set; } = string.Empty;
    }

}
