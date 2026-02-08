using Microsoft.EntityFrameworkCore;
using SchemaAI.DALContracts;
using SchemaAI.Entities;
using SchemaAI.Persistence;

namespace SchemaAI.DAL
{
    public sealed class ModuleDal
    : BaseDal<Module>, IModuleDal
    {
        public ModuleDal(SchemaAIDbContext db) : base(db) { }

        public async Task<List<Module>> GetByApplicationGuidAsync(Guid applicationGuid)
        {
            return await _db.Modules
                .Where(m => m.ApplicationGuid == applicationGuid)
                .ToListAsync();
        }
    }
}
