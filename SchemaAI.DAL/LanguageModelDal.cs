using Microsoft.EntityFrameworkCore;
using SchemaAI.DALContracts;
using SchemaAI.Entities;
using SchemaAI.Persistence;

namespace SchemaAI.DAL
{
    public sealed class LanguageModelDal
    : BaseDal<LanguageModel>, ILanguageModelDal
    {
        public LanguageModelDal(SchemaAIDbContext db) : base(db) { }

        public async Task<List<LanguageModel>> GetAllLanguageModelAsync(string szProviderName)
        {

            return await _db.LanguageModels
                .Where(x => x.Provider == szProviderName)
                .ToListAsync();
        
        }

        public async Task<List<LanguageModel>> GetAllLanguageModelAsync(string szProviderName,bool isActive)
        {

            return await _db.LanguageModels
                .Where(x => x.Provider == szProviderName && x.IsActive == isActive)
                .ToListAsync();

        }
    }
}
