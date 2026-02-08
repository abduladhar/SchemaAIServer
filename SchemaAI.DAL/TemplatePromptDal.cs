using Microsoft.EntityFrameworkCore;
using SchemaAI.DALContracts;
using SchemaAI.Entities;
using SchemaAI.Persistence;

namespace SchemaAI.DAL
{
    public sealed class TemplatePromptDal
       : BaseDal<TemplatePrompt>, ITemplatePromptDal
    {
        public TemplatePromptDal(SchemaAIDbContext db)
            : base(db)
        {
        }

        public async Task<TemplatePrompt?> GetByTemplateGuidAsync(Guid templateGuid)
        {
            return await _db.TemplatePrompts
                .FirstOrDefaultAsync(x => x.TemplateGuid == templateGuid);
        }
    }
}
