namespace SchemaAI.Entities
{
    public class TemplatePrompt : BaseEntity
    {
        public Guid TemplatePromptGuid { get; set; }
        public Guid TemplateGuid { get; set; }

        public string PromptText { get; set; } = string.Empty;
        public string OutputSchemaJson { get; set; } = string.Empty;
    }

}
