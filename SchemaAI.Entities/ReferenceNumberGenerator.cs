namespace SchemaAI.Entities
{
    public static class ReferenceNumberGenerator
    {
        public static string Generate()
        {
            return $"REF-{DateTime.UtcNow:yyyyMMdd}-{Guid.NewGuid():N}".Substring(0, 28).ToUpper();
        }
    }
}
