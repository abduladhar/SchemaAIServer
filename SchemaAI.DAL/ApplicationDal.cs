using CommonEntities;
using Microsoft.EntityFrameworkCore;
using SchemaAI.DALContracts;
using SchemaAI.Entities;
using SchemaAI.Persistence;

namespace SchemaAI.DAL
{
    public sealed class ApplicationDal
    : BaseDal<Application>, IApplicationDal
    {
        public ApplicationDal(SchemaAIDbContext db) : base(db) { }

        public async Task<Application?> GetWithModulesAsync(Guid applicationGuid)
        {
            return await _db.Applications
                .Include(a => a.Modules)
                .FirstOrDefaultAsync(a => a.ApplicationGuid == applicationGuid);
        }

        public async Task<PagedResult<Application>> GetPagedWithModulesAsync(
        int pageNumber,
        int pageSize)
        {
            var query = _db.Applications
                .Include(x => x.Modules)
                .AsNoTracking();

            return await GetPagedAsync(query, pageNumber, pageSize);
        }
    }
}
