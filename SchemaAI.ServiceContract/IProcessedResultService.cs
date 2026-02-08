using CommonEntities;
using SchemaAI.Entities;

namespace SchemaAI.ServiceContract
{
    public interface IProcessedResultService
    {
        Task<ProcessedResult?> GetByRequestAsync(Guid processingRequestGuid);
        Task<ProcessedResult?> GetByProcessedGuidAsync(Guid processingRequestGuid);
        Task<ProcessedResult?> GetByReferenceNoAsync(string referenceNo, string apiKey);
        Task<ProcessedResult> CreateAsync(ProcessedResult result);
        Task<List<ProcessedResult>> GetAllAsync();

        #region Paging

        Task<PagedResult<ProcessedResult>> GetPagedAsync(int pageNumber, int pageSize);

        Task<List<ProcessedResult>> GetScrollAsync(int lastId, int batchSize);

        #endregion Paging
    };
}
