using CommonEntities;
using SchemaAI.Entities;

namespace SchemaAI.ServiceContract
{
    public interface IGenAIProviderService
    {
        Task<List<GenAIProvider>> GetAllAsync();
        Task<GenAIProvider?> GetAsync(Guid genAIProviderGuid);
        Task<GenAIProvider> CreateAsync(GenAIProvider genAIProvider);
        Task UpdateAsync(GenAIProvider genAIProvider);
        Task DeleteAsync(Guid genAIProviderGuid);

        #region Paging

        Task<PagedResult<GenAIProvider>> GetPagedAsync(int pageNumber, int pageSize);

        Task<List<GenAIProvider>> GetScrollAsync(int lastId, int batchSize);

        #endregion Paging
    }
}
