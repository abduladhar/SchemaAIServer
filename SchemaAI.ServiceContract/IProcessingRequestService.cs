using CommonEntities;
using SchemaAI.Entities;

namespace SchemaAI.ServiceContract
{
    public interface IProcessingRequestService
    {
        Task<ProcessingRequest?> GetByReferenceAsync(string referenceNumber);
        Task<ProcessingRequest> CreateAsync(ProcessingRequest request);
        Task UpdateStatusAsync(Guid requestGuid, string status);
        Task<List<ProcessingRequest>> GetAllAsync();

        #region Paging

        Task<PagedResult<ProcessingRequest>> GetPagedAsync(int pageNumber, int pageSize);

        Task<List<ProcessingRequest>> GetScrollAsync(int lastId, int batchSize);

        #endregion Paging
    }
}
