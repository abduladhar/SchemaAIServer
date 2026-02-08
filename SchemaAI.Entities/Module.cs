using System.Text.Json.Serialization;

namespace SchemaAI.Entities
{
    public class Module : BaseEntity
    {
        public Guid ModuleGuid { get; set; }
        public Guid ApplicationGuid { get; set; }

        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public bool IsEnabled { get; set; }

        public List<Template> Templates { get; set; } = new();
    }
}
