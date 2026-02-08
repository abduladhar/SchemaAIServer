namespace SchemaAI.Entities
{
    public class Template : BaseEntity
    {
        public Guid TemplateGuid { get; set; }
        public Guid ModuleGuid { get; set; }

        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;

        public TemplatePrompt? Prompt { get; set; }
        public TemplateFileConfig? FileConfig { get; set; }
    }
}
