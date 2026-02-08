using CommonEntities;
using SchemaAI.Entities;

namespace SchemaAI.ServiceContract
{
    public interface IUploadedFileService
    {
        Task<UploadedFile?> GetByReferenceAsync(string referenceNumber);
        Task<UploadedFile?> GetUploadedFileByGuid(Guid uploadFileGuid);
        Task<UploadedFile> CreateAsync(UploadedFile file);
        Task<bool> GenerateAIObjects(Guid fileGuid);
        #region Paging

        Task<PagedResult<UploadedFile>> GetPagedAsync(int pageNumber, int pageSize);

        Task<List<UploadedFile>> GetScrollAsync(int lastId, int batchSize);

        #endregion Paging
    }
}
