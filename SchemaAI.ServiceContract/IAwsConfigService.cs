using CommonEntities;
using SchemaAI.Entities;

namespace SchemaAI.ServiceContract
{
    public interface IAwsConfigService
    {
        Task<List<AwsConfig>> GetAllAsync();
        Task<AwsConfig?> GetAsync(Guid awsConfigGuid);
        Task<AwsConfig> CreateAsync(AwsConfig awsConfig);
        Task UpdateAsync(AwsConfig awsConfig);
        Task DeleteAsync(Guid awsConfigGuid);

        Task<AwsConfig> GetCurrentAsync();
        Task InvalidateCacheAsync();

        #region Paging

        Task<PagedResult<AwsConfig>> GetPagedAsync(int pageNumber, int pageSize);

        Task<List<AwsConfig>> GetScrollAsync(int lastId, int batchSize);

        #endregion Paging
    }
}
