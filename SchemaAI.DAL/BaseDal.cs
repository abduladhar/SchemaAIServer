using CommonEntities;
using Microsoft.EntityFrameworkCore;
using SchemaAI.DALContracts;
using SchemaAI.Entities;
using SchemaAI.Persistence;

namespace SchemaAI.DAL
{
    public abstract class BaseDal<TEntity> : IBaseDal<TEntity>
        where TEntity : BaseEntity
    {
        protected readonly SchemaAIDbContext _db;
        protected readonly DbSet<TEntity> _set;

        protected BaseDal(SchemaAIDbContext db)
        {
            _db = db;
            _set = db.Set<TEntity>();
        }

        public virtual async Task<TEntity?> GetByGuidAsync(Guid guid)
        {
            return await _set.FirstOrDefaultAsync(e =>
                EF.Property<Guid>(e, $"{typeof(TEntity).Name}Guid") == guid);
        }

        public virtual async Task<List<TEntity>> GetAllAsync()
        {
            return await _set.ToListAsync();
        }

        public virtual async Task AddAsync(TEntity entity)
        {
            await _set.AddAsync(entity);
        }

        public virtual Task UpdateAsync(TEntity entity)
        {
            _set.Update(entity);
            return Task.CompletedTask;
        }

        public virtual Task SoftDeleteAsync(TEntity entity)
        {
            entity.IsDeleted = true;
            _set.Update(entity);
            return Task.CompletedTask;
        }

        public virtual async Task<PagedResult<TEntity>> GetPagedAsync(
        int pageNumber,
        int pageSize)
        {
            if (pageNumber <= 0) pageNumber = 1;
            if (pageSize <= 0) pageSize = 25;

            var query = _set.AsNoTracking();

            var total = await query.CountAsync();

            var items = await query
                .OrderBy(x => x.Id) // uses your new identity column 👍
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return new PagedResult<TEntity>
            {
                Items = items,
                TotalCount = total,
                PageNumber = pageNumber,
                PageSize = pageSize
            };
        }

        public virtual async Task<List<TEntity>> GetScrollAsync(
        int lastId,
        int batchSize)
        {
            return await _set
                .Where(x => x.Id > lastId)
                .OrderBy(x => x.Id)
                .Take(batchSize)
                .ToListAsync();
        }

        public virtual async Task<PagedResult<TEntity>> GetPagedAsync(
        IQueryable<TEntity> query,
        int pageNumber,
        int pageSize)
        {
            if (pageNumber <= 0) pageNumber = 1;
            if (pageSize <= 0) pageSize = 25;

            var total = await query.CountAsync();

            var items = await query
                .OrderBy(x => x.Id)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return new PagedResult<TEntity>
            {
                Items = items,
                TotalCount = total,
                PageNumber = pageNumber,
                PageSize = pageSize
            };
        }


        public IQueryable<TEntity> Query() => _set.AsQueryable();
    }
}
