using CommonEntities;
using SchemaAI.Entities;

namespace SchemaAI.ServiceContract
{
    public interface ITemplatePromptService
    {
        Task<TemplatePrompt?> GetByTemplateAsync(Guid templateGuid);
        Task<TemplatePrompt> CreateOrUpdateAsync(TemplatePrompt prompt);
        #region Paging

        Task<PagedResult<TemplatePrompt>> GetPagedAsync(int pageNumber, int pageSize);

        Task<List<TemplatePrompt>> GetScrollAsync(int lastId, int batchSize);

        #endregion Paging
    }
}
