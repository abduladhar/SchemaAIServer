using SchemaAI.Entities;

namespace SchemaAI.DALContracts
{
    public interface IProcessedResultDal : IBaseDal<ProcessedResult>
    {
        Task<ProcessedResult?> GetByProcessingRequestGuidAsync(Guid processingRequestGuid);
        Task<ProcessedResult?> GetByProcessedGuidAsync(Guid processedRequestGuid);
        Task<List<ProcessedResult>> GetAllByProcessingRequestGuidAsync(Guid processingRequestGuid);
        Task<ProcessedResult?> GetByReferanceNoAsync(string p_szReferanceNo);
        
    }
}
