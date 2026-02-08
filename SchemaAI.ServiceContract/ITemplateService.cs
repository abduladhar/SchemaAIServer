using CommonEntities;
using SchemaAI.Entities;

namespace SchemaAI.ServiceContract
{
    public interface ITemplateService
    {
        Task<List<Template>> GetAllAsync();
        Task<Template?> GetFullAsync(Guid templateGuid);
        Task<List<Template>> GetByModuleAsync(Guid moduleGuid);
        Task<Template> CreateAsync(Template template);
        Task UpdateAsync(Template template);
        Task DeleteAsync(Guid templateGuid);
        #region Paging

        Task<PagedResult<Template>> GetPagedAsync(int pageNumber, int pageSize);

        Task<List<Template>> GetScrollAsync(int lastId, int batchSize);

        #endregion Paging
    }
}
