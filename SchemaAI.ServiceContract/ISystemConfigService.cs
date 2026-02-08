using CommonEntities;
using SchemaAI.Entities;

namespace SchemaAI.ServiceContract
{
    public interface ISystemConfigService
    {
        Task<List<SystemConfig>> GetAllAsync();
        Task<SystemConfig?> GetAsync(Guid SystemConfigGuid);
        Task<SystemConfig> CreateAsync(SystemConfig SystemConfig);
        Task UpdateAsync(SystemConfig SystemConfig);
        Task DeleteAsync(Guid SystemConfigGuid);
        #region Paging

        Task<PagedResult<SystemConfig>> GetPagedAsync(int pageNumber, int pageSize);

        Task<List<SystemConfig>> GetScrollAsync(int lastId, int batchSize);

        Task<SystemConfig?> GetAsync(string p_szApiKey);
        #endregion Paging
    }
}
