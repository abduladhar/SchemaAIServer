namespace SchemaAI.Entities
{
    public class GenAIProvider : BaseEntity
    {
        public Guid GenAIProviderGuid { get; set; }   // ✅ FIXED
        public string Name { get; set; } = string.Empty;
        public string SecretAccessKey { get; set; } = string.Empty;
        public string ModelName { get; set; } = string.Empty;
        public string BaseUrl { get; set; } = string.Empty;
    }

    public static class ModelProviders
    {
        public const string OpenAI = "OpenAI";
        public const string Bedrock = "Bedrock";
        public const string AWS = "AWS";          // optional alias
        public const string AzureOpenAI = "AzureOpenAI";

        public static readonly IReadOnlyList<string> All = new[]
        {
            OpenAI,
            Bedrock,
            AzureOpenAI
        };
    }
}
