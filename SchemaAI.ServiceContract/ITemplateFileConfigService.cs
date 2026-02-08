using CommonEntities;
using SchemaAI.Entities;

namespace SchemaAI.ServiceContract
{
    public interface ITemplateFileConfigService
    {
        Task<TemplateFileConfig?> GetByTemplateAsync(Guid templateGuid);
        Task<TemplateFileConfig> CreateOrUpdateAsync(TemplateFileConfig config);
        #region Paging

        Task<PagedResult<TemplateFileConfig>> GetPagedAsync(int pageNumber, int pageSize);
        Task<List<TemplateFileConfig>> GetScrollAsync(int lastId, int batchSize);

        #endregion Paging
    }
}
