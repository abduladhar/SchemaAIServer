using Microsoft.EntityFrameworkCore;
using SchemaAI.DALContracts;
using SchemaAI.Entities;
using SchemaAI.Persistence;

namespace SchemaAI.DAL
{
    public sealed class SystemConfigDal
   : BaseDal<SystemConfig>, ISystemConfigDal
    {
        public SystemConfigDal(SchemaAIDbContext db) : base(db) { }

        public async Task<SystemConfig?> GetByApiKey(string apiKey)
        {
            return await _db.SystemConfig.IgnoreQueryFilters()
                 .FirstOrDefaultAsync(x => x.ApiKey == apiKey);
        }
    }
}
