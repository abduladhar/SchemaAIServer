namespace SchemaAI.Entities
{
    public class Application : BaseEntity
    {
        public Guid ApplicationGuid { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;

        public List<Module> Modules { get; set; } = new();
    }
}
