using System;
using System.Collections.Generic;
using System.Text;

//namespace SchemaAI.Entities
//{
//    public interface ISoftDelete
//    {
//        bool IsDeleted { get; set; }
//    }
//    public interface ITenantEntity
//    {
//        Guid TenantGuid { get; set; }
//    }

//    public interface IUserAudit
//    {
//        Guid CreatedByUserGuid { get; set; }
//        string CreatedByUserName { get; set; }
//        DateTime CreatedOn { get; set; }

//        Guid? ModifiedByUserGuid { get; set; }
//        string? ModifiedByUserName { get; set; }
//        DateTime? ModifiedOn { get; set; }
//    }

//    public class Application : BaseEntity
//    {
//        public Guid ApplicationGuid { get; set; }
//        public string Name { get; set; } = string.Empty;
//        public string Description { get; set; } = string.Empty;

//        public List<Module> Modules { get; set; } = new();
//    }

//    public abstract class BaseEntity : ITenantEntity, IUserAudit, ISoftDelete
//    {
//        public Guid TenantGuid { get; set; }

//        public Guid CreatedByUserGuid { get; set; }
//        public string CreatedByUserName { get; set; } = string.Empty;
//        public DateTime CreatedOn { get; set; }

//        public Guid? ModifiedByUserGuid { get; set; }
//        public string? ModifiedByUserName { get; set; }
//        public DateTime? ModifiedOn { get; set; }

//        public bool IsDeleted { get; set; }
//    }

//    public class Module : BaseEntity
//    {
//        public Guid ModuleGuid { get; set; }
//        public Guid ApplicationGuid { get; set; }

//        public string Name { get; set; } = string.Empty;
//        public string Description { get; set; } = string.Empty;
//        public bool IsEnabled { get; set; }

//        public List<Template> Templates { get; set; } = new();
//    }

//    public class ProcessedResult : BaseEntity
//    {
//        public Guid ProcessedResultGuid { get; set; }
//        public Guid ProcessingRequestGuid { get; set; }

//        public string OutputJson { get; set; } = string.Empty;
//    }

//    public class ProcessingRequest : BaseEntity
//    {
//        public Guid ProcessingRequestGuid { get; set; }

//        public Guid UploadedFileGuid { get; set; }
//        public string ReferenceNumber { get; set; } = string.Empty;
//        public Guid TemplateGuid { get; set; }

//        public ProcessingStatus Status { get; set; }
//    }
//    public enum ProcessingStatus
//    {
//        Pending = 0,
//        Processing = 1,
//        Completed = 2,
//        Failed = 3
//    }

//    public static class ReferenceNumberGenerator
//    {
//        public static string Generate()
//        {
//            return $"REF-{DateTime.UtcNow:yyyyMMdd}-{Guid.NewGuid():N}".Substring(0, 28).ToUpper();
//        }
//    }

//    public class Template : BaseEntity
//    {
//        public Guid TemplateGuid { get; set; }
//        public Guid ModuleGuid { get; set; }

//        public string Name { get; set; } = string.Empty;
//        public string Description { get; set; } = string.Empty;

//        public TemplatePrompt Prompt { get; set; }
//        public TemplateFileConfig FileConfig { get; set; }
//    }

//    public class TemplateFileConfig : BaseEntity
//    {
//        public Guid TemplateFileConfigGuid { get; set; }
//        public Guid TemplateGuid { get; set; }

//        public string AllowedFileTypesCsv { get; set; } = string.Empty;
//        public long MaxFileSizeInBytes { get; set; }
//    }

//    public class TemplatePrompt : BaseEntity
//    {
//        public Guid TemplatePromptGuid { get; set; }
//        public Guid TemplateGuid { get; set; }

//        public string PromptText { get; set; } = string.Empty;
//        public string OutputSchemaJson { get; set; } = string.Empty;
//    }

//    public class UploadedFile : BaseEntity
//    {
//        public Guid UploadedFileGuid { get; set; }
//        public string ReferenceNumber { get; set; } = string.Empty;

//        public Guid TemplateGuid { get; set; }

//        public string OriginalFileName { get; set; } = string.Empty;
//        public string StoredFilePath { get; set; } = string.Empty;
//    }
//}
