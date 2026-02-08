using CommonEntities;
using SchemaAI.Entities;

namespace SchemaAI.ServiceContract
{
    public interface IModuleService
    {
        Task<List<Module>> GetAllAsync();
        Task<Module?> GetAsync(Guid moduleGuid);
        Task<List<Module>> GetByApplicationAsync(Guid applicationGuid);
        Task<Module> CreateAsync(Module module);
        Task UpdateAsync(Module module);
        Task DeleteAsync(Guid moduleGuid);

        #region Paging

        Task<PagedResult<Module>> GetPagedAsync(int pageNumber, int pageSize);

        Task<List<Module>> GetScrollAsync(int lastId, int batchSize);

        #endregion Paging
    }
}
