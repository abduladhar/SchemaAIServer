namespace SchemaAI.Entities
{
    public class AwsConfig : BaseEntity
    {
        public Guid AwsConfigGuid { get; set; }   // ✅ FIXED
        public string AccessKeyID { get; set; } = string.Empty;
        public string SecretAccessKey { get; set; } = string.Empty;
        public string Region { get; set; } = string.Empty;
        public string S3BucketName { get; set; } = string.Empty;
        public bool IsCurrentItem { get; set; } = true;
    }
}
