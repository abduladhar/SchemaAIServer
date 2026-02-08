using CommonEntities;
using Microsoft.AspNetCore.StaticFiles;
using NUlid;
using SchemaAI.DAL;
using SchemaAI.DALContracts;
using SchemaAI.Entities;
using SchemaAI.Persistence;
using SchemaAI.ServiceContract;
using System.IO.Enumeration;

namespace SchemaAI.Services
{
    public sealed class UploadedFileService : IUploadedFileService
    {
        private readonly IUploadedFileDal _dal;
        private readonly ITemplateDal _templatedal;
        private readonly ITemplatePromptDal _TemplatePromptDal;
        private readonly SchemaAIDbContext _db;
        private readonly IAWSService _AWSService;
        private readonly ICurrentUser _currentUser;

        public UploadedFileService(
            IUploadedFileDal dal,
            ITemplateDal templateDal,
            ITemplatePromptDal templatePromptDal,
            IAWSService AWSService,
            SchemaAIDbContext db,
            ICurrentUser currentUser)
        {
            _AWSService = AWSService;
            _dal = dal;
            _templatedal = templateDal;
            _TemplatePromptDal = templatePromptDal;
            _db = db;
            _currentUser = currentUser;
        }

        public async Task<UploadedFile?> GetByReferenceAsync(string referenceNumber)
            => await _dal.GetByReferenceAsync(referenceNumber);

        public async Task<UploadedFile?> GetUploadedFileByGuid(Guid fileGuid)
           => await _dal.GetUploadedFilebyGuidAsync(fileGuid);



        public async Task<UploadedFile> CreateAsync(UploadedFile file)
        {
            var prefix = GenerateUniquePrefix();

            var pathParts = new List<string>();
            file.CreateSetupConfiguration(_currentUser);
            file.UploadedFileGuid= Guid.NewGuid();
            AddIfNotEmpty(pathParts, file.ApplicationName);
            AddIfNotEmpty(pathParts, file.ModuleName);
            AddIfNotEmpty(pathParts, file.TemplateName);

            // Always include date folders
            pathParts.Add(DateTime.UtcNow.ToString("yyyy"));
            pathParts.Add(DateTime.UtcNow.ToString("MM"));
            pathParts.Add(DateTime.UtcNow.ToString("dd"));

            // Final file name
            pathParts.Add($"{prefix}_{file.OriginalFileName}");

            file.StoredFilePath = string.Join("/", pathParts);
            file.ContantType = GetContentType(file.OriginalFileName);

            await _dal.AddAsync(file);
            await _db.SaveChangesAsync();


            file.SignedUrl = await _AWSService.GeneratePutPreSignedUrl(file.StoredFilePath, file.ContantType, 45);

            return file;
        }

        public string GetContentType(string fileName)
        {
            var provider = new FileExtensionContentTypeProvider();

            if (!provider.TryGetContentType(fileName, out var contentType))
            {
                contentType = "application/octet-stream"; // fallback
            }

            return contentType;
        }

        private string Sanitize(string value)
        {
            return string.IsNullOrWhiteSpace(value)
                ? "unknown"
                : value.Replace(" ", "_")
                       .Replace("/", "_")
                       .Replace("\\", "_");
        }

        private void AddIfNotEmpty(List<string> parts, string? value)
        {
            if (!string.IsNullOrWhiteSpace(value))
            {
                parts.Add(Sanitize(value));
            }
        }

        public static string GenerateUniquePrefix()
        {
            return Ulid.NewUlid().ToString();
        }

        public async Task<bool> GenerateAIObjects(Guid fileGuid)
        {
            var objUploadedFile = await _dal.GetUploadedFilebyGuidAsync(fileGuid);
            if (objUploadedFile != null)
            {
                var objTemplate =await _templatedal.GetByGuidAsync(objUploadedFile.TemplateGuid);
                if (objTemplate != null)
                {
                    var szTemplatePromt = await _TemplatePromptDal.GetByTemplateGuidAsync(objTemplate.TemplateGuid);
                    string szPromt = szTemplatePromt.PromptText;

                }
            }

            return true;
        }

        #region Paging

        public async Task<PagedResult<UploadedFile>> GetPagedAsync(
        int pageNumber,
        int pageSize)
        {
            return await _dal.GetPagedAsync(pageNumber, pageSize);
        }

        public async Task<List<UploadedFile>> GetScrollAsync(
        int lastId,
        int batchSize)
        {
            return await _dal.GetScrollAsync(lastId, batchSize);
        }

        #endregion
    }
}
