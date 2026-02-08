using CommonEntities;
using SchemaAI.Entities;

namespace SchemaAI.DALContracts
{
    public interface IBaseDal<TEntity>
       where TEntity : BaseEntity
    {
        Task<TEntity?> GetByGuidAsync(Guid guid);
        Task<List<TEntity>> GetAllAsync();

        Task AddAsync(TEntity entity);
        Task UpdateAsync(TEntity entity);
        Task SoftDeleteAsync(TEntity entity);

        IQueryable<TEntity> Query();

        Task<PagedResult<TEntity>> GetPagedAsync(
        int pageNumber,
        int pageSize);

        Task<PagedResult<TEntity>> GetPagedAsync(
            IQueryable<TEntity> query,
            int pageNumber,
            int pageSize);

        Task<List<TEntity>> GetScrollAsync(int lastId, int batchSize);

    }
}
